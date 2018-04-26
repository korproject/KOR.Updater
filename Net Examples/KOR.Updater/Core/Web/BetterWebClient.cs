using System;
using System.Net;

namespace KOR.Updater.Core.Web
{
    public class BetterWebClient : WebClient
    {
        /// <summary>
        /// See <see cref="System.Net.WebRequest.GetWebRequest"/>
        /// </summary>
        /// <param name="address">See <see cref="System.Net.WebRequest.GetWebRequest"/></param>
        /// <returns>See <see cref="System.Net.WebRequest.GetWebRequest"/></returns>
        public virtual WebRequest GetWebRequestX(Uri address)
        {
            return this.GetWebRequest(address);
        }
    }
}
