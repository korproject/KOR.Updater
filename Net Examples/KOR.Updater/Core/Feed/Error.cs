using KOR.Updater.Controllers;
using KOR.Updater.Core.Helpers;
using KOR.Updater.JSON;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using static KOR.Updater.Core.Api;

namespace KOR.Updater.Core.Feed
{
    public class Error
    {
        #region Error Constructors

        /// <summary>
        /// Which update will send feedback
        /// </summary>
        public string UpdateId { get; set; }

        /// <summary>
        /// Error Title
        /// </summary>
        public string ErrorTitle { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Error time
        /// </summary>
        public string ErrorTime { get; set; }

        /// <summary>
        /// Error response
        /// </summary>
        public ApiResponse ErrorResponse { get; set; }

        /// <summary>
        /// JSON deseialized response data
        /// </summary>
        public RootobjectforError RespJsonObj { get; set; }

        #endregion

        #region Error Sender

        /// <summary>
        /// Send request for error info
        /// </summary>
        /// <returns></returns>
        public bool SendErrorInfo()
        {
            if (InternetController.InternetCheck())
            {
                ErrorResponse = new ApiResponse();

                #region API Request

                // request host
                var server = new RestClient("http://api.kor.onl/");
                // request relative url
                var request = new RestRequest("apps/updater/1.0/", Method.POST);

                // add important parameters
                request.AddOrUpdateParameter("request", "error");
                request.AddOrUpdateParameter("type", OutputType);
                request.AddOrUpdateParameter("updateid", UpdateId);
                request.AddOrUpdateParameter("api_key", API_KEY);
                request.AddOrUpdateParameter("api_secret", API_SECRET);
                request.AddOrUpdateParameter("title", ErrorTitle);
                request.AddOrUpdateParameter("message", ErrorMessage);
                request.AddOrUpdateParameter("client_date", ErrorTime);
                request.AddOrUpdateParameter("client", Client.Username);
                request.AddOrUpdateParameter("machine", Client.CPUId);

                // get query result
                var response = server.Execute(request);

                // get response status code
                ErrorResponse.ResponseStatus = response.StatusCode;
                ErrorResponse.ResponseData = response.Content;

                #endregion

                #region JSON Parsing

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                };
                RespJsonObj = JsonConvert.DeserializeObject<RootobjectforError>(response.Content, settings);

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

                    ErrorResponse.ResponseResultString = RespJsonObj.result;
                }
                else
                {
                    // error message
                    ErrorResponse.ResponseAPIErrorMessage = (string)RespJsonObj.messages.error_message ?? "";
                    // warning message
                    ErrorResponse.ResponseAPIWarningMessage = (string)RespJsonObj.messages.warning_message ?? "";
                }

                #endregion
            }

            return false;
        }

        #endregion
    }
}
