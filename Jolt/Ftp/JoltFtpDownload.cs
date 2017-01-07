using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Ftp
{
    public class JoltFtpDownload
    {

        private string url;
        private string filepath;
        private string username;
        private string password;
        private int timeOut;

        private Action OnComplete;
        public Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltFtpDownload(string url)
        {
            this.url = url;
        }

        public JoltFtpDownload SaveTo(string filepath)
        {
            this.filepath = filepath;
            return this;
        }

        public JoltFtpDownload SetCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;
            return this;
        }

        public void MakeRequest(Action OnComplete, Action<string> OnFail = null,
                                Action OnStart = null,
                                Action<long, long, long> OnProgress = null)
        {

            this.OnComplete = OnComplete;
            this.OnProgress = OnProgress;
            this.OnFail = OnFail;        

            using (var client = new WebClient())
            {

                if (username != null && password != null)
                {
                    client.Credentials = new NetworkCredential(username, password);
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
