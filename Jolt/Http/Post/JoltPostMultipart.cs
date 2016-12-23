using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Post
{
    public class JoltPostMultipart
    {

        private string url;
        private MultipartFormDataContent MultipartContent = new MultipartFormDataContent();

        public JoltPostMultipart(string url)
        {
            this.url = url;
        }

        public JoltPostMultipart AddField(string content, string name)
        {
            MultipartContent.Add(new StringContent(content), name);
            return this;
        }

        // Here, we are reading a file from its path all at once.
        // Reading a large file all at once can cause memory problems.
        public JoltPostMultipart AddFile(string filePath, string name, string fileName)
        {
            byte[] file = File.ReadAllBytes(filePath);
            MultipartContent.Add(new StreamContent(new MemoryStream(file)), name, fileName);
            return this;
        }

        // If you have to upload a large file, then you should read it in your own way.
        // Then send it as a byte array.
        public JoltPostMultipart AddFile(byte[] file, string name, string fileName)
        {
            MultipartContent.Add(new StreamContent(new MemoryStream(file)), name, fileName);
            return this;
        }

        public async void MakeRequest(Action<string> OnSuccess, Action<string> OnFail = null,
                                      Action OnStart = null, Action OnFinish = null)
        {
            using (var client = new HttpClient())
            {
                // Call OnStart() at the beginning
                if (OnStart != null)
                    OnStart();

                try
                {
                    var response = await client.PostAsync(this.url, MultipartContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        OnSuccess(result.ToString());
                    }
                    else
                    {
                        OnFail(response.StatusCode.ToString());
                    }
                }
                catch (Exception e)
                {
                    OnFail(e.ToString());
                }

                // Call OnFinish() at the beginning
                if (OnFinish != null)
                    OnFinish();

            }
        }

    }
}
