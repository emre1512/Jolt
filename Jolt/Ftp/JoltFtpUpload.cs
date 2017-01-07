using System;
using System.IO;
using System.Net;

namespace JoltHttp.Ftp
{
    public class JoltFtpUpload
    {

        private string url;
        private string filePath;
        byte[] file;
        private string username;
        private string password;

        private Action OnComplete;
        private Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltFtpUpload(string url)
        {
            this.url = url;
        }

        public JoltFtpUpload AddFile(byte[] file)
        {
            this.file = file;
            return this;
        }

        public JoltFtpUpload AddFile(string filePath)
        {
            this.filePath = filePath;
            return this;
        }

        public JoltFtpUpload SetCredentials(string username, string password)
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
                    client.UploadDataAsync(new Uri(url), "STOR", fileToSend);
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
