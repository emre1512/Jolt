using JoltHttp.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JoltHttp.Http.Get
{
    public class JoltGetJSON
    {
        private string url;
        private CookieContainer cookieContainer = new CookieContainer();
        private string oAuthKey;
        private string oAuthValue;
        private int timeOut;

        public JoltGetJSON(string url)
        {
            this.url = url;
        }

        public JoltGetJSON SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        public JoltGetJSON SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        //public JoltGetJSON SetTimeOut(int TimeOut)
        //{
        //    timeOut = TimeOut;
        //    return this;
        //}

        public async void MakeRequest(Action<object> OnComplete, Action<string> OnFail = null,
                                      Action OnStart = null)
        {

            var handler = new HttpClientHandler();

            if (cookieContainer.Count != 0)
            {
                handler.UseCookies = false;
                handler.CookieContainer = cookieContainer;
            }

            using (var client = new HttpClient(handler))
            {

                if (timeOut != 0)
                {
                    client.Timeout = new TimeSpan(0, 0, 0, timeOut);
                }

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
                    var jsonObject = JSONConverter.JSONToObject(result);
                    OnComplete(result);                                          
                }
                catch (Exception e)
                {
                    if (OnFail != null)
                        OnFail(e.ToString());
                }

                // Call OnFinish() at the end
                //if (OnFinish != null)
                //    OnFinish();

            }

        }

    }
}
