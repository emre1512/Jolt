using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Put
{
    public class JoltPutFile
    {
        private string url;
        private string filePath;
        byte[] file;
        private List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>();
        private string oAuthKey;
        private string oAuthValue;

        private Action OnSuccess;
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

        public JoltPutFile SetCookies(string CookieName, string CookieValue)
        {
            Cookies.Add(new KeyValuePair<string, string>(CookieName, CookieValue));
            return this;
        }

        public JoltPutFile SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        public void MakeRequest(Action OnSuccess, Action<string> OnFail = null,
                                Action OnStart = null, Action OnFinish = null,
                                Action<long, long, long> OnProgress = null)
        {

            this.OnSuccess = OnSuccess;
            this.OnProgress = OnProgress;
            this.OnFail = OnFail;

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
                    client.UploadDataAsync(new Uri(url), fileToSend);
                    client.UploadDataCompleted += Completed;
                    client.UploadProgressChanged += UploadProgress;
                }
                catch (Exception e)
                {
                    OnFail(e.ToString());
                }

                // Call OnFinish() at the end
                if (OnFinish != null)
                    OnFinish();

            }             
        }

        private void UploadProgress(object sender, UploadProgressChangedEventArgs e)
        {
            if(OnProgress != null)
                OnProgress(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
        }

        private void Completed(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
                OnSuccess();
            else
                OnFail(e.Error.ToString());
        }

    }
}
