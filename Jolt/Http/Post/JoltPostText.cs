using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Post
{
    public class JoltPostText
    {

        private string url;
        private string text;
        private CookieContainer cookieContainer = new CookieContainer();
        private string oAuthKey;
        private string oAuthValue;
        private int timeOut;

        public JoltPostText(string url, string text)
        {
            this.url = url;
            this.text = text;
        }

        public JoltPostText SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        public JoltPostText SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        public JoltPostText SetTimeOut(int TimeOut)
        {
            timeOut = TimeOut;
            return this;
        }

        public async void MakeRequest(Action<string> OnSuccess, Action<string> OnFail = null,
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
                    client.BaseAddress = new Uri(url);               
                    var content = new StringContent(text);

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        OnSuccess(result.ToString());
                    }
                    else
                    {
                        if (OnFail != null)
                            OnFail(response.StatusCode.ToString());
                    }
                }
                catch (Exception e)
                {
                    if (OnFail != null)
                        OnFail(e.ToString());
                }

                // Call OnFinish() at the beginning
                //if (OnFinish != null)
                //    OnFinish();

            }

        }

    }
}
