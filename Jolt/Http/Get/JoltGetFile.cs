using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace JoltHttp.Http.Get
{
    /// Use this for getting file from url.
    public class JoltGetFile
    {
        private string url;
        private string filepath;
        private List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>();
        private string oAuthKey;
        private string oAuthValue;
        private int timeOut;

        private Action OnComplete;
        public Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltGetFile(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Adds a custom cookie to request header.
        /// </summary>
        /// <param name="CookieName">Name of the cookie.</param>
        /// <param name="CookieValue">Value of the cookie.</param>
        public JoltGetFile SetCookies(string CookieName, string CookieValue)
        {
            Cookies.Add(new KeyValuePair<string, string>(CookieName, CookieValue));
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="key">OAuth name.</param>
        /// <param name="value">OAuth value.</param>
        public JoltGetFile SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        /// <summary>
        /// Saves file to the given path.
        /// </summary>
        /// <param name="filepath">File path for the file.</param>
        public JoltGetFile SaveTo(string filepath)
        {
            this.filepath = filepath;
            return this;
        }

        /// <summary>
        /// Gets file from the url.
        /// </summary>
        /// <param name="OnComplete">Called when request is completed.</param>
        /// <param name="OnFail">Called when request fails.</param>
        /// <param name="OnStart">Called when request starts.</param>
        /// <param name="OnProgress">Returns progress report of download process.</param>
        public void MakeRequest(Action OnComplete, Action<string> OnFail = null,
                                Action OnStart = null,
                                Action<long, long, long> OnProgress = null)
        {

            this.OnComplete = OnComplete;
            this.OnProgress = OnProgress;
            this.OnFail = OnFail;

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
            //        var cancellationToken = new CancellationToken();

            //        //using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            //        //using (var result = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            //        using (var result = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            //        {
            //            long totalBytes = (long)result.Content.Headers.ContentLength;
            //            if (result.IsSuccessStatusCode)
            //            {

            //                using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
            //                {

            //                    using (var filestream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            //                    //using (var ms = new MemoryStream())
            //                    {

            //                        long bytesRead = 0;
            //                        byte[] buffer = new byte[1024];


            //                        //while (bytesRead != stream.Length)
            //                        while (true)
            //                        {
            //                            int num = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            //                            bytesRead += num;

            //                            if (OnProgress != null)
            //                                OnProgress(bytesRead, totalBytes, bytesRead * 100 / totalBytes);

            //                            int bytesToWrite;
            //                            if ((bytesToWrite = num) != 0)
            //                                await filestream.WriteAsync(buffer, 0, bytesToWrite, cancellationToken).ConfigureAwait(false);
            //                            else
            //                                break;
            //                        }

            //                        //stream.CopyTo(ms);
            //                        //r = ms.ToArray();
            //                        //File.WriteAllBytes(filepath, r);
            //                    }
            //                }

            //                OnSuccess();
            //            }
            //            else
            //            {
            //                if (OnFail != null)
            //                    OnFail(result.StatusCode.ToString());
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
                    client.DownloadFileAsync(new Uri(url), filepath);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                }
                catch (Exception e)
                {
                    if (OnFail != null)
                        OnFail(e.ToString());
                }

            }

        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            if (OnProgress != null)
                OnProgress(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
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
