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
    class JoltFtpDownload
    {

        private string url;
        private string filepath;
        private string oAuthKey;
        private string oAuthValue;
        private int timeOut;

        private Action OnComplete;
        public Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltFtpDownload(string url)
        {
            this.url = url;
        }

        public JoltFtpDownload SetCredentials(string key, string value)
        {
            oAuthKey = key;
            oAuthValue = value;
            return this;
        }

        public JoltFtpDownload SaveTo(string filepath)
        {
            this.filepath = filepath;
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

                if (oAuthKey != null && oAuthValue != null)
                {
                    client.Credentials = new NetworkCredential(oAuthKey, oAuthValue);
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
