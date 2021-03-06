﻿using JoltHttp.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JoltHttp.Http.Post
{
    /// Use this for posting JSON.
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

        /// <summary>
        /// Adds a custom cookie to request header.
        /// </summary>
        /// <param name="CookieName">Name of the cookie.</param>
        /// <param name="CookieValue">Value of the cookie.</param>
        public JoltPostJSON SetCookies(string CookieName, string CookieValue)
        {        
            cookieContainer.Add(new Cookie(CookieName, CookieValue));
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="key">OAuth name.</param>
        /// <param name="value">OAuth value.</param>
        public JoltPostJSON SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        /// <summary>
        /// Posts JSON data.
        /// </summary>
        /// <param name="OnComplete">Called when request is completed.</param>
        /// <param name="OnFail">Called when request fails.</param>
        /// <param name="OnStart">Called when request starts.</param>
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
                        OnComplete(result);
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
