using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Get
{
    public class JoltGetFile
    {
        private string url;
        private string filepath;

        private Action OnSuccess;
        public Action<string> OnFail;
        private Action<long, long, long> OnProgress;

        public JoltGetFile(string url)
        {
            this.url = url;
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

            //using (var httpClient = new HttpClient())
            //{ 
            //    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            //    {                 
            //        using (Stream contentStream = await(await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
            //               stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            //        {
            //            await contentStream.CopyToAsync(stream);
            //        }
            //    }
            //}

            using (var client = new WebClient())
            {
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
