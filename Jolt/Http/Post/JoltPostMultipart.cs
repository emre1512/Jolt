using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Post
{
    public class JoltPostMultipart
    {

        private string url;
        //private MultipartFormDataContent MultipartContent = new MultipartFormDataContent();
        private NameValueCollection FormFields = new NameValueCollection();
        private byte[] fileToSend;
        //private CookieContainer cookieContainer = new CookieContainer();
        private List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>();
        private string oAuthKey;
        private string oAuthValue;
        private int timeOut;

        private Action<string> OnComplete;
        private Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltPostMultipart(string url)
        {
            this.url = url;
        }

        public JoltPostMultipart AddField(string value, string name)
        {
            FormFields.Add(value, name);
            //MultipartContent.Add(new StringContent(content), name);
            return this;
        }

        public JoltPostMultipart SetCookies(string CookieName, string CookieValue)
        {
            Cookies.Add(new KeyValuePair<string, string>(CookieName, CookieValue));
            return this;
        }

        public JoltPostMultipart SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        //public JoltPostMultipart SetTimeOut(int TimeOut)
        //{
        //    timeOut = TimeOut;
        //    return this;
        //}

        // Here, we are reading a file from its path all at once.
        // Reading a large file all at once can cause memory problems.
        public JoltPostMultipart AddFile(string filePath)
        {
            fileToSend = File.ReadAllBytes(filePath);
            return this;
        }

        // If you have to upload a large file, then you should read it in your own way.
        // Then send it as a byte array.
        public JoltPostMultipart AddFile(byte[] file)
        {
            fileToSend = file;
            return this;
        }

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
