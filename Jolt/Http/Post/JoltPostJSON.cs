using JoltHttp.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JoltHttp.Http.Post
{
    public class JoltPostJSON
    {
        private string url;
        private string json;
        private CookieContainer cookieContainer = new CookieContainer();
        private string oAuthKey;
        private string oAuthValue;

        public JoltPostJSON(string url, string json)
        {
            this.url = url;
            this.json = json;
        }

        public JoltPostJSON SetCookies(string CookieName, string CookieValue)
        {        
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        public JoltPostJSON SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
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
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new StringContent(json);

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var jsonObject = JSONConverter.JSONToObject(result);
                        OnSuccess(result);
                    }
                    else
                    {
                        OnFail(response.StatusCode.ToString());
                    }
                }
                catch (Exception e)
                {
                    OnFail(e.ToString());
                }

                // Call OnFinish() at the beginning
                if (OnFinish != null)
                    OnFinish();

            }

        }

    }
}
