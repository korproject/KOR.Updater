using System;
using System.IO;
using System.Net;
using System.Text;

namespace KOR.Updater.Core.Web
{
    public class Internet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="recurlyResponse"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="responseStatusCode"></param>
        public void processResponse(WebResponse response, out string recurlyResponse, out string responseHeaders, out int responseStatusCode)
        {
            recurlyResponse = responseHeaders = String.Empty;
            responseStatusCode = 0;

            using (var responseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    recurlyResponse = sr.ReadToEnd();
                }
            }

            responseHeaders = response.Headers.ToString();
            var httpWebResponse = response as HttpWebResponse;
            if (httpWebResponse != null)
            {
                responseStatusCode = (int)httpWebResponse.StatusCode;
            }
        }


    }
}
