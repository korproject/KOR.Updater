using KOR.Updater.Controllers;
using KOR.Updater.Core.Helpers;
using KOR.Updater.JSON;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using static KOR.Updater.Core.Api;

namespace KOR.Updater.Core.Feed
{
    public class Feedback
    {
        #region Feedback Constructors

        /// <summary>
        /// Which update will send feedback
        /// </summary>
        public string UpdateId { get; set; }

        /// <summary>
        /// User comment about this update
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// User point
        /// </summary>
        public double Point { get; set; }

        /// <summary>
        /// Feedback response
        /// </summary>
        public ApiResponse FeedbackResponse { get; set; }

        /// <summary>
        /// JSON deseialized response data
        /// </summary>
        public RootobjectforFeedback RespJsonObj { get; set; }

        #endregion

        #region Feedback Sender

        /// <summary>
        /// Send request for feedback
        /// </summary>
        /// <returns></returns>
        public bool SendFeedback()
        {
            if (InternetController.InternetCheck())
            {
                FeedbackResponse = new ApiResponse();

                #region API Request

                // request host
                var server = new RestClient("http://api.kor.onl/");
                // request relative url
                var request = new RestRequest("apps/updater/1.0/", Method.POST);

                // add important parameters
                request.AddOrUpdateParameter("request", "feedback");
                request.AddOrUpdateParameter("type", OutputType);
                request.AddOrUpdateParameter("updateid", UpdateId);
                request.AddOrUpdateParameter("api_key", API_KEY);
                request.AddOrUpdateParameter("api_secret", API_SECRET);
                request.AddOrUpdateParameter("comment", Comment);
                request.AddOrUpdateParameter("point", Point);
                request.AddOrUpdateParameter("client", Client.Username);
                request.AddOrUpdateParameter("machine", Client.CPUId);

                // get query result
                var response = server.Execute(request);

                // get response status code
                FeedbackResponse.ResponseStatus = response.StatusCode;
                FeedbackResponse.ResponseData = response.Content;

                #endregion

                #region JSON Parsing

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                };
                RespJsonObj = JsonConvert.DeserializeObject<RootobjectforFeedback>(response.Content, settings);

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

                    FeedbackResponse.ResponseResultString = RespJsonObj.result;
                }
                else
                {
                    // error message
                    FeedbackResponse.ResponseAPIErrorMessage = (string)RespJsonObj.messages.error_message ?? "";
                    // warning message
                    FeedbackResponse.ResponseAPIWarningMessage = (string)RespJsonObj.messages.warning_message ?? "";
                }

                #endregion
            }

            return false;
        }

        #endregion
    }
}
