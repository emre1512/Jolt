using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace JoltHttp.Http.Post
{
    /// Use this for posting multipart/form data.
    public class JoltPostMultipart
    {

        private string url;
        private NameValueCollection FormFields = new NameValueCollection();
        private byte[] fileToSend;
        private List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>();
        private string oAuthKey;
        private string oAuthValue;

        private Action<string> OnComplete;
        private Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltPostMultipart(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Adds a form field.
        /// </summary>
        /// <param name="key">Name of the field.</param>
        /// <param name="value">Value of the field.</param>
        public JoltPostMultipart AddField(string key, string value)
        {
            FormFields.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds a custom cookie to request header.
        /// </summary>
        /// <param name="CookieName">Name of the cookie.</param>
        /// <param name="CookieValue">Value of the cookie.</param>
        public JoltPostMultipart SetCookies(string CookieName, string CookieValue)
        {
            Cookies.Add(new KeyValuePair<string, string>(CookieName, CookieValue));
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="key">OAuth name.</param>
        /// <param name="value">OAuth value.</param>
        public JoltPostMultipart SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        /// <summary>
        /// Adds a file from file path. This method reads file all at once.
        /// This can cause memory problems for large files. If file is too large,
        /// you can read it in your own way and pass it as byte array.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        public JoltPostMultipart AddFile(string filePath)
        {
            fileToSend = File.ReadAllBytes(filePath);
            return this;
        }

        /// <summary>
        /// Adds a file as byte array.
        /// </summary>
        /// <param name="file">File to send as byte array.</param>
        public JoltPostMultipart AddFile(byte[] file)
        {
            fileToSend = file;
            return this;
        }

        /// <summary>
        /// Posts multipart/form data.
        /// </summary>
        /// <param name="OnComplete">Called when request is completed.</param>
        /// <param name="OnFail">Called when request fails.</param>
        /// <param name="OnStart">Called when request starts.</param>
        /// <param name="OnProgress">Returns progress report of upload process.</param>
        public void MakeRequest(Action<string> OnComplete, Action<string> OnFail = null,
                                      Action OnStart = null,
                                      Action<long, long, long> OnProgress = null)
        {
            this.OnComplete = OnComplete;
            this.OnFail = OnFail;
            this.OnProgress = OnProgress;

            using (var client = new WebClient())
            {

                if (FormFields != null) client.QueryString = FormFields;

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
                    client.UploadDataAsync(new Uri(url), "POST", fileToSend);
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
                OnComplete(e.Result.ToString());
            }
            else
            {
                if (OnFail != null)
                    OnFail(e.Error.ToString());
            }
        }

    }
}
