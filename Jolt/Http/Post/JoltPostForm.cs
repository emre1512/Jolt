using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JoltHttp.Http.Post
{

    /// Use this for posting x-www-form-urlencoded data.
    public class JoltPostForm
    {
        private string url;
        private CookieContainer cookieContainer = new CookieContainer();
        private Dictionary<string, string> FormFields = new Dictionary<string, string>();
        private string oAuthKey;
        private string oAuthValue;

        public JoltPostForm(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Adds a form field.
        /// </summary>
        /// <param name="key">Name of the field.</param>
        /// <param name="value">Value of the field.</param>
        public JoltPostForm AddField(string key, string value)
        {
            FormFields.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds a custom cookie to request header.
        /// </summary>
        /// <param name="CookieName">Name of the cookie.</param>
        /// <param name="CookieValue">Value of the cookie.</param>
        public JoltPostForm SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="key">OAuth name.</param>
        /// <param name="value">OAuth value.</param>
        public JoltPostForm SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        /// <summary>
        /// Posts x-www-form-urlencoded data.
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
                    var content = new FormUrlEncodedContent(FormFields);

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

            }

        }

    }
}
