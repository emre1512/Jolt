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
    public class JoltPostForm
    {
        private string url;
        private CookieContainer cookieContainer = new CookieContainer();
        private Dictionary<string, string> FormFields = new Dictionary<string, string>();
        private string oAuthKey;
        private string oAuthValue;
        private int timeOut;

        public JoltPostForm(string url)
        {
            this.url = url;
        }

        public JoltPostForm AddField(string key, string value)
        {
            FormFields.Add(key, value);
            return this;
        }

        public JoltPostForm SetCookies(string CookieName, string CookieValue)
        {
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        public JoltPostForm SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        //public JoltPostForm SetTimeOut(int TimeOut)
        //{
        //    timeOut = TimeOut;
        //    return this;
        //}

        /// <summary>
        /// Posting x-www-urlencoded form data
        /// </summary>
        /// <param name="OnComplete">Called when post request is completed</param>
        /// <param name="OnFail">Called when post request fails</param>
        /// <param name="OnStart">Called when post request starts</param>
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
