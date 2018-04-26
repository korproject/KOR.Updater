using KOR.Updater.Core.Security;
using KOR.Updater.Core.System;
using KOR.Updater.Core.Web;
using KOR.Updater.JSON;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace KOR.Updater.Core
{
    public class Feedback
    {
        Info Info = new Info();
        ResponseWatcher internet = new ResponseWatcher();
        Common common = new Common();
        AES256 aes256 = new AES256();

        Stopwatch stopwatch = new Stopwatch();

        // API_KEY for app authorization
        public string API_KEY = null;
        // API_SECRET for client authorization
        public string API_SECRET = null;


        /// <summary>
        /// Feecback sender
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SendAppFeedback(string feedback_type, string message, string messageid, string update_id)
        {
            UpdateCheck updatecheck = new UpdateCheck();

            stopwatch.Start();

            bool ret = false;

            DateTime startedOnUtc = DateTime.UtcNow;
            string responseData = null;
            string responseHeaders = null;
            int responseStatusCode = 0;

            try
            {
                BetterWebClient webClient = new BetterWebClient();
                webClient.Proxy = null;
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/json charset=utf-8");

                var webRequest = webClient.GetWebRequestX(new Uri(CreateCheckURL(feedback_type, message, messageid, update_id)));
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

                // save errorlog for feedback
                common.saveErrorLog(errorlog, errorid, updatecheck.update_id);
            }

            Debug.WriteLine("respoınse---->>>>>>>>>>>>>>>>>>" + responseStatusCode);
            // All 2xx HTTP statuses are OK
            if (responseStatusCode >= 200 && responseStatusCode < 300)
            {
                // response json data
                RootobjectforFeedback jobj = JsonConvert.DeserializeObject<RootobjectforFeedback>(responseData);

                if (jobj.status && jobj.feed_status)
                    ret = true;
                else
                    ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Returns update check URL with API definitions
        /// </summary>
        /// <returns>Update check URL</returns>
        private string CreateCheckURL(string feedback_type, string message, string messageid, string update_id)
        {
            UpdateCheck updatecheck = new UpdateCheck();

            //string outputformat = updatecheck.OutputFormat.ToLower() != null ? updatecheck.OutputFormat.ToLower() : string.Empty;
            string outputformat = "json";

            feedback_type = "feedback_type=" + feedback_type;
            string uniqsuerid = "&unique_userid=" + common.getUnique_User_Identifier();
            messageid = "&message_id=" + messageid;
            message = "&message=" + common.base64Engine(message);
            update_id = "&update_id=" + update_id;

            string url = "http://api.kor.onl/apps/updater/" + outputformat + "/feedback/" + feedback_type + uniqsuerid + message + messageid + update_id;

            // last one add api definitions
            url = url + "&api_key=22B6EB8F159F1B209F1CC982AC8A3EA0B30C5B9E3DCCEEC167FCFCA06866BCB7" + "&api_secret=e6842c6e809e4bee48d4b0a6815231b7f21042e4d119b465681541b683f8dd565214200470f8b3b46841ee7a98adae19";
            Debug.WriteLine(Uri.EscapeUriString(url).ToString());
            return Uri.EscapeUriString(url).ToString();
        }

        // Installed feedback
        public bool sendFeedbackInstalled(string message, string update_id)
        {
            string messageid = aes256.AES256Engine(Info.current_username + "+" + DateTime.UtcNow.ToString(), Info.current_username);

            return SendAppFeedback("installed", message, messageid, update_id);
        }

        // Download feedback
        public bool sendFeedbackDownloaded(string message, string update_id)
        {
            string messageid = aes256.AES256Engine(Info.current_username + "+" + DateTime.UtcNow.ToString(), Info.current_username);

            return SendAppFeedback("downloaded", message, messageid, update_id);
        }

        // Updated feedback
        public bool sendFeedbackUpdated(string message, string update_id)
        {
            string messageid = aes256.AES256Engine(Info.current_username + "+" + DateTime.UtcNow.ToString(), Info.current_username);

            return SendAppFeedback("updated", message, messageid, update_id);
        }

        // Error feedback
        public bool sendFeedbackError(string message, string update_id)
        {
            string messageid = aes256.AES256Engine(Info.current_username + "+" + DateTime.UtcNow.ToString(), Info.current_username);

            return SendAppFeedback("error", message, messageid, update_id);
        }

        // Abort Mission feedback
        public bool sendFeedbackAbortMission(string message, string update_id)
        {
            string messageid = aes256.AES256Engine(Info.current_username + "+" + DateTime.UtcNow.ToString(), Info.current_username);

            return SendAppFeedback("abort", message, messageid, update_id);
        }
    }
}
