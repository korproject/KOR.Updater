using KOR.Updater.Controllers;
using KOR.Updater.Core.Helpers;
using KOR.Updater.JSON;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using static KOR.Updater.Core.Api;

namespace KOR.Updater.Core.Feed
{
    public class Reporter
    {
        #region Reporter Constructors

        /// <summary>
        /// Report value: file downloaded
        /// </summary>
        public bool Downloaded { get; set; }

        /// <summary>
        /// Report value: success operation
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Reprot value: fail operation
        /// </summary>
        public bool Fail { get; set; }

        /// <summary>
        /// Error response
        /// </summary>
        public ApiResponse ReporterResponse { get; set; }

        /// <summary>
        /// JSON deseialized response data
        /// </summary>
        public RootobjectforReports RespJsonObj { get; set; }

        #endregion

        #region Reporter

        /// <summary>
        /// Check and parse updater json response data
        /// </summary>
        /// <returns></returns>
        public bool SendReport(string updateid)
        {
            if (InternetController.InternetCheck())
            {
                ReporterResponse = new ApiResponse();

                #region API Request

                // request host
                var server = new RestClient("http://api.kor.onl/");
                // request relative url
                var request = new RestRequest("apps/updater/1.0/", Method.POST);

                // add important parameters
                request.AddOrUpdateParameter("request", "report");
                request.AddOrUpdateParameter("type", OutputType);
                request.AddOrUpdateParameter("updateid", updateid);
                request.AddOrUpdateParameter("api_key", API_KEY);
                request.AddOrUpdateParameter("api_secret", API_SECRET);
                request.AddOrUpdateParameter("download", Downloaded);
                request.AddOrUpdateParameter("success", Success);
                request.AddOrUpdateParameter("fail", Fail);
                request.AddOrUpdateParameter("client", Client.Username);
                request.AddOrUpdateParameter("machine", Client.CPUId);

                // get query result
                var response = server.Execute(request);

                // get response status code
                ReporterResponse.ResponseStatus = response.StatusCode;
                ReporterResponse.ResponseData = response.Content;

                #endregion

                #region JSON Parsing

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                };
                RespJsonObj = JsonConvert.DeserializeObject<RootobjectforReports>(response.Content, settings);

                #endregion

                #region Updates JSON Parsing

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // if API code is 1
                    if (RespJsonObj.code == 1)
                    {
                        if (RespJsonObj.result == "OK")
                        {
                            return true;
                        }
                    }

                    ReporterResponse.ResponseResultString = RespJsonObj.result;
                }
                else
                {
                    // error message
                    ReporterResponse.ResponseAPIErrorMessage = (string)RespJsonObj.messages.error_message ?? "";
                    // warning message
                    ReporterResponse.ResponseAPIWarningMessage = (string)RespJsonObj.messages.warning_message ?? "";
                }

                #endregion
            }

            return false;
        }

        #endregion
    }
}
