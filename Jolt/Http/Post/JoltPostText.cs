using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


namespace JoltHttp.Http.Post
{
    /// Use this for posting string/text.
    public class JoltPostText
    {

        private string url;
        private string text;
        private CookieContainer cookieContainer = new CookieContainer();
        private string oAuthKey;
        private string oAuthValue;

        public JoltPostText(string url, string text)
        {
            this.url = url;
            this.text = text;
        }

        /// <summary>
        /// Adds a custom cookie to request header.
        /// </summary>
        /// <param name="CookieName">Name of the cookie.</param>
        /// <param name="CookieValue">Value of the cookie.</param>
        public JoltPostText SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="key">OAuth name.</param>
        /// <param name="value">OAuth value.</param>
        public JoltPostText SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        /// <summary>
        /// Posts string/text data.
        /// </summary>
        /// <param name="OnComplete">Called when request is completed.</param>
        /// <param name="OnFail">Called when request fails.</param>
        /// <param name="OnStart">Called when request starts.</param>
        public async void MakeRequest(Action<string> OnComplete, Action<string> OnFail = null,
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
                        OnComplete(result.ToString());
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
