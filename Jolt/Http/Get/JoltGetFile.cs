using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Get
{
    public class JoltGetFile
    {
        private string url;
        private string filepath;
        private List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>();

        private Action OnSuccess;
        public Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltGetFile(string url)
        {
            this.url = url;
        }

        public JoltGetFile SetCookies(string CookieName, string CookieValue)
        {
            Cookies.Add(new KeyValuePair<string, string>(CookieName, CookieValue));
            return this;
        }

        public JoltGetFile SaveTo(string filepath)
        {
            this.filepath = filepath;
            return this;
        }

        public void MakeRequest(Action OnSuccess, Action<string> OnFail = null,
                                Action OnStart = null, Action OnFinish = null,
                                Action<long, long, long> OnProgress = null)
        {

            this.OnSuccess = OnSuccess;
            this.OnProgress = OnProgress;
            this.OnFail = OnFail;

            using (var client = new WebClient())
            {
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
                OnStart();

                try
                {
                    client.DownloadFileAsync(new Uri(url), filepath);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);

                    //client.DownloadDataAsync(new Uri(url), filepath);
                    //client.DownloadDataCompleted += Completed;
                    //client.DownloadProgressChanged += DownloadProgress;
                }
                catch (Exception e)
                {
                    OnFail(e.ToString());
                }

                // Call OnFinish() at the end
                OnFinish();
            }

        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            OnProgress(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
        }

        //private void Completed(object sender, DownloadDataCompletedEventArgs e)
        //{
        //    OnSuccess(e.Result.ToString());
        //}

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
                OnSuccess();
            else
                OnFail(e.Error.ToString());
        }

    }
}
