using System;
using System.Collections.Generic;
using System.IO;
using System.Net;


namespace JoltHttp.Http.Put
{
    /// Use this to upload file via PUT request.
    public class JoltPutFile
    {
        private string url;
        private string filePath;
        byte[] file;
        private List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>();
        private string oAuthKey;
        private string oAuthValue;

        private Action OnComplete;
        private Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltPutFile(string filePath, string url)
        {
            this.filePath = filePath;
            this.url = url;
        }

        public JoltPutFile(byte[] file, string url)
        {
            this.file = file;
            this.url = url;
        }

        /// <summary>
        /// Adds a custom cookie to request header.
        /// </summary>
        /// <param name="CookieName">Name of the cookie.</param>
        /// <param name="CookieValue">Value of the cookie.</param>
        public JoltPutFile SetCookies(string CookieName, string CookieValue)
        {
            Cookies.Add(new KeyValuePair<string, string>(CookieName, CookieValue));
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="key">OAuth name.</param>
        /// <param name="value">OAuth value.</param>
        public JoltPutFile SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }


        /// <summary>
        /// Posts multipart/form data.
        /// </summary>
        /// <param name="OnComplete">Called when request is completed.</param>
        /// <param name="OnFail">Called when request fails.</param>
        /// <param name="OnStart">Called when request starts.</param>
        /// <param name="OnProgress">Returns progress report of upload process.</param>
        public void MakeRequest(Action OnComplete, Action<string> OnFail = null,
                                Action OnStart = null,
                                Action<long, long, long> OnProgress = null)
        {

            this.OnComplete = OnComplete;
            this.OnFail = OnFail;
            this.OnProgress = OnProgress;

            // If you didn't read the file in your own way, this reads it automatically.
            // Automatic read can cause memory problems.
            byte[] fileToSend;

            if (file == null)
            {
                fileToSend = File.ReadAllBytes(filePath);
            }
            else
            {
                fileToSend = file;
            }

            //var handler = new HttpClientHandler();

            //if (cookieContainer.Count != 0)
            //{
            //    handler.UseCookies = false;
            //    handler.CookieContainer = cookieContainer;
            //}

            //using (var client = new HttpClient(handler))
            //{

            //    if (timeOut != 0)
            //    {
            //        client.Timeout = new TimeSpan(0, 0, 0, timeOut);
            //    }

            //    if (oAuthKey != null && oAuthValue != null)
            //    {
            //        client.DefaultRequestHeaders.Authorization =
            //                    new AuthenticationHeaderValue(oAuthKey, oAuthValue);
            //    }

            //    // Call OnStart() at the beginning
            //    if (OnStart != null)
            //        OnStart();

            //    try
            //    {
            //        using (var ms = new MemoryStream(fileToSend))
            //        {
            //            HttpContent content = new StreamContent(ms);
            //            var response = await client.PutAsync(url, content);

            //            if (response.IsSuccessStatusCode)
            //            {
            //                var result = await response.Content.ReadAsStringAsync();
            //                OnSuccess(result.ToString());
            //            }
            //            else
            //            {
            //                if (OnFail != null)
            //                    OnFail(response.StatusCode.ToString());
            //            }
            //        }

            //    }
            //    catch (Exception e)
            //    {
            //        if (OnFail != null)
            //            OnFail(e.ToString());
            //    }

            //}

            using (var client = new WebClient())
            {
                
                if (oAuthKey != null && oAuthValue != null)
                {
                    client.Credentials = new NetworkCredential(oAuthKey, oAuthValue);
                }

                if (Cookies.Count != 0)
                {
                    string cookies = "";

                    foreach (var element in Cookies)
                    {
                        cookies += element.Key + "=" + element.Value + "; ";
                    }

                    cookies = cookies.Remove(cookies.Length - 2);
                    client.Headers.Add(HttpRequestHeader.Cookie, cookies);
                }

                // Call OnStart() at the beginning
                if (OnStart != null)
                    OnStart();

                try
                {
                    client.UploadDataAsync(new Uri(url), "PUT", fileToSend);
                    client.UploadDataCompleted += Completed;
                    client.UploadProgressChanged += UploadProgress;                   
                }
                catch (Exception e)
                {
                    if (OnFail != null)
                        OnFail(e.ToString());
                }

            }
        }

        private void UploadProgress(object sender, UploadProgressChangedEventArgs e)
        {
            if (OnProgress != null)
                OnProgress(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
        }

        private void Completed(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                OnComplete();
            }
            else
            {
                if (OnFail != null)
                    OnFail(e.Error.ToString());
            }
        }

    }

}
