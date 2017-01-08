using System;
using System.ComponentModel;
using System.Net;

namespace JoltHttp.Ftp
{
    /// Use this for downloading file from ftp.
    public class JoltFtpDownload
    {

        private string url;
        private string filepath;
        private string username;
        private string password;

        private Action OnComplete;
        public Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltFtpDownload(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Saves file to the given path.
        /// </summary>
        /// <param name="filepath">File path for the file.</param>
        public JoltFtpDownload SaveTo(string filepath)
        {
            this.filepath = filepath;
            return this;
        }

        /// <summary>
        /// Adds authentication info to request header.
        /// </summary>
        /// <param name="username">Username for ftp.</param>
        /// <param name="password">Password for ftp.</param>
        public JoltFtpDownload SetCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;
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
