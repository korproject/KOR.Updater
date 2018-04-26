using KOR.Updater.Core.Security;
using KOR.Updater.Core.System;
using KOR.Updater.Core.Web;
using KOR.Updater.JSON;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;

namespace KOR.Updater.Core
{
    public class UpdateCheck
    {

        #region Definitions

        Info Info = new Info();
        string username = WindowsIdentity.GetCurrent().Name;

        public UpdateCheck()
        {
            Info.current_username = username;
        }
        Internet internet = new Internet();
        AES256 aes256 = new AES256();
        Feedback.Feedback feedback = new Feedback.Feedback();
        Common common = new Common();

        // API_KEY for app authorization
        public string API_KEY = null;
        // API_SECRET for client authorization
        public string API_SECRET = null;
        // JSON or XML 
        public string OutputFormat = null;
        // 
        public string Version = null;

        #endregion

        Stopwatch stopwatch = new Stopwatch();

        #region Update Definitions
        public bool updatestatus = false;

        public string download_url = string.Empty;
        public string update_id = string.Empty;
        public string current_version = string.Empty;
        public string client_version = string.Empty;
        public string severity = string.Empty;
        public string message_title = string.Empty;
        public string message_content = string.Empty;
        public string added_features = string.Empty;
        public string removed_features = string.Empty;
        #endregion

        /// <summary>
        /// Check and parse update
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            bool ret = false;

            var response = GetUpdaterResponse();

            if (response != null)
            {
                try
                {
                    RootobjectforUpdater jobj = JsonConvert.DeserializeObject<RootobjectforUpdater>(response);
                    
                    if (jobj.status && jobj.update_status) {
                        ret = jobj.status;

                        update_id = jobj.result.update_id;
                        current_version = jobj.result.version;
                        client_version = jobj.result.client_version;
                        download_url = jobj.result.download_url;
                        severity = jobj.result.severity;
                        message_title = jobj.result.message_title;
                        message_content = jobj.result.message_content;
                        added_features = jobj.result.added_features;
                        removed_features = jobj.result.removed_features;
                    } else {
                        updatestatus = false;
                    }
                }
                catch { updatestatus = false;}
            }

            return ret;
        }

        /// <summary>
        /// Returns update check response
        /// </summary>
        /// <returns></returns>
        public string GetUpdaterResponse()
        {
            stopwatch.Start();

            DateTime startedOnUtc = DateTime.UtcNow;
            string responseData = null;
            string responseHeaders = null;
            int responseStatusCode = 0;

            try
            {
                BetterWebClient webClient = new BetterWebClient();
                webClient.Proxy = null;
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/" + OutputFormat.ToLower() + "  charset=utf-8");

                var webRequest = webClient.GetWebRequestX(new Uri(CreateCheckURL()));
                webRequest.Method = WebRequestMethods.Http.Get;

                using (WebResponse response = webRequest.GetResponse())
                {
                    internet.processResponse(response, out responseData, out responseHeaders, out responseStatusCode);
                }
            }
            catch (WebException ex)
            {
                using (ex.Response)
                {
                    internet.processResponse(ex.Response, out responseData, out responseHeaders, out responseStatusCode);
                }

                // stop stopwatch
                stopwatch.Stop();

                #region Prepare Error Log

                // get error log contains
                string errorlog = ":Date: " + DateTime.UtcNow.ToString() + Environment.NewLine;
                errorlog = errorlog + ":ExMessage: " + ex.Message + Environment.NewLine + Environment.NewLine;
                errorlog = errorlog + ":ExtentedExMEssage: " + ex.ToString() + Environment.NewLine + Environment.NewLine;
                errorlog = errorlog + ":HttpStatus: " + responseStatusCode + Environment.NewLine;
                errorlog = errorlog + ":ResponseData: " + responseData + Environment.NewLine;
                errorlog = errorlog + ":TimeElapsed: " + stopwatch.Elapsed;

                // get error log id
                string errorid = aes256.AES256Engine(Info.current_username + "+" + DateTime.UtcNow.ToString(), Info.current_username);

                #endregion

                // send errorlog feedback
                if (!feedback.SendAppFeedback("error", errorlog, errorid, update_id))
                {
                    // save error log for next time sending try
                    common.saveErrorLog(errorlog, errorid, update_id);
                }
            }

            // All 2xx HTTP statuses are OK
            if (responseStatusCode >= 200 && responseStatusCode < 300)
            {
                return responseData;
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns update check URL with API definitions
        /// </summary>
        /// <returns>Update check URL</returns>
        private string CreateCheckURL() {
            string outputformat = OutputFormat.ToLower() != null ? OutputFormat.ToLower() : "json";
            string uniqmachineid = "&uniqmachineid=" + Info.getCPUID();

            string url = $"http://api.kor.onl/apps/updater/{outputformat}/{Version}/" + uniqmachineid;

            // add api definitions
            url = url + "&api_key=" + API_KEY + "&api;_secret=" + API_SECRET;

            return url;
        }
    }
}

