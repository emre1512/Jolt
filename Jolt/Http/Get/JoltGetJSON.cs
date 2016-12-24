using JoltHttp.Utils;
using System;
using System.Net;
using System.Net.Http;


namespace JoltHttp.Http.Get
{
    public class JoltGetJSON
    {
        private string url;
        private CookieContainer cookieContainer = new CookieContainer();

        public JoltGetJSON(string url)
        {
            this.url = url;
        }

        public JoltGetJSON SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        public async void MakeRequest(Action<object> OnSuccess, Action<string> OnFail = null,
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
                
                // Call OnStart() at the beginning
                if (OnStart != null)
                    OnStart();

                try
                {               
                    var result = await client.GetStringAsync(url);
                    var jsonObject = JSONConverter.JSONToObject(result);
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
