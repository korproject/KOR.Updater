using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Collections.Generic;
using KOR.Updater.Controllers;
using static KOR.Updater.Core.Api;
using KOR.Updater.JSON;
using KOR.Updater.Core.Helpers;

namespace KOR.Updater.Core
{
    /// <summary>
    /// Updater Class
    /// </summary>
    public class Updater
    {
        #region Updater Constructors

        /// <summary>
        /// Current app version
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Updates of list
        /// </summary>
        public List<Update> Updates { get; set; }

        /// <summary>
        /// Multi result for listing multi updates
        /// </summary>
        public bool MultiResult { get; set; }

        /// <summary>
        /// Updater response
        /// </summary>
        public ApiResponse UpdaterResponse { get; set; }

        /// <summary>
        /// JSON deseialized response data
        /// </summary>
        public RootobjectforUpdater RespJsonObj { get; set; }

        #endregion

        #region Updater

        /// <summary>
        /// Check and parse updater json response data
        /// </summary>
        /// <returns></returns>
        public object CheckUpdate()
        {
            if (InternetController.InternetCheck())
            {
                UpdaterResponse = new ApiResponse();

                #region API Request

                // request host
                var server = new RestClient("http://api.kor.onl/");
                // request relative url
                var request = new RestRequest("apps/updater/1.0/", Method.POST);

                // add important parameters
                request.AddOrUpdateParameter("request", "updatecheck");
                request.AddOrUpdateParameter("type", OutputType);
                request.AddOrUpdateParameter("version", AppVersion);
                request.AddOrUpdateParameter("api_key", API_KEY);
                request.AddOrUpdateParameter("api_secret", API_SECRET);
                request.AddOrUpdateParameter("multiresult", MultiResult);

                // get query result
                var response = server.Execute(request);

                // get response status code
                UpdaterResponse.ResponseStatus = response.StatusCode;
                UpdaterResponse.ResponseData = response.Content;

                #endregion

                #region JSON Parsing

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                };

                RespJsonObj = JsonConvert.DeserializeObject<RootobjectforUpdater>(response.Content, settings);

                #endregion

                #region Updates JSON Parsing

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // if API code is 1
                    if (RespJsonObj.code == 1)
                    {
                        // possible there is/are update/s
                        if (RespJsonObj.result is Array && RespJsonObj.result != null)
                        {
                            // create new updates list
                            Updates = new List<Update>();

                            // get updates
                            foreach (var update in RespJsonObj.result)
                            {
                                // parse update
                                var newupdate = new Update()
                                {
                                    AppId = update.appid,
                                    AppVersion = update.app_version,
                                    UpdateId = update.updateid,
                                    ClientVersion = update.client_version,
                                    ReasonCode = update.reason_code,
                                    ReasonTitle = update.reason_title,
                                    AddedDate = update.added_date,
                                    MessageTitle = update.message_title,
                                    MessageContent = update.message_content,
                                    DownloadUrl = update.download_url,
                                    AddedFeatures = update.added_features,
                                    RemovedFeatures = update.removed_features
                                };

                                // add to list
                                Updates.Add(newupdate);
                            }

                            // and then return true
                            return Updates.Count > 0 ? true : false;
                        }
                    }
                }
                else
                {
                    // error message
                    UpdaterResponse.ResponseAPIErrorMessage = (string)RespJsonObj.messages.error_message;
                    // warning message
                    UpdaterResponse.ResponseAPIWarningMessage = (string)RespJsonObj.messages.warning_message;
                }

                #endregion

                return false;
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Updates base class
    /// </summary>
    public class Update
    {
        public string AppId { get; set; }
        public string AppVersion { get; set; }
        public string UpdateId { get; set; }
        public string ClientVersion { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonTitle { get; set; }
        public string AddedDate { get; set; }
        public string MessageTitle { get; set; }
        public string MessageContent { get; set; }
        public string DownloadUrl { get; set; }
        public string AddedFeatures { get; set; }
        public string RemovedFeatures { get; set; }
    }
}
