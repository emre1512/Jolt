using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Get
{
    public class JoltGetText
    {
        private string url;
        private CookieContainer cookieContainer = new CookieContainer();
        private string oAuthKey;
        private string oAuthValue;

        public JoltGetText(string url)
        {
            this.url = url;
        }

        public JoltGetText SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        public JoltGetText SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        public async void MakeRequest(Action<string> OnSuccess, Action<string> OnFail = null,
                                      Action OnStart = null, Action OnFinish = null)
        {

            var handler = new HttpClientHandler();

            if (cookieContainer.Count != 0)
            {
                handler.UseCookies = false;
                handler.CookieContainer = cookieContainer;
            }

            using (var client = new HttpClient(handler))
            {

                if (oAuthKey != null && oAuthValue != null)
                {
                    client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue(oAuthKey, oAuthValue);
                }

                // Call OnStart() at the beginning
                if (OnStart != null)
                    OnStart();

                try
                {
                    var result = await client.GetStringAsync(url);
                    OnSuccess(result);
                }
                catch (Exception e)
                {
                    OnFail(e.ToString());
                }

                // Call OnFinish() at the end
                if (OnFinish != null)
                    OnFinish();

            }

        }

    }
}
