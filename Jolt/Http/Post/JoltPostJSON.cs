using Jolt.Utils;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JoltHttp.Http.Post
{
    public class JoltPostJSON
    {
        private string url;
        private string json;

        public JoltPostJSON(string url, string json)
        {
            this.url = url;
            this.json = json;
        }

        public async void MakeRequest(Action<object> OnSuccess, Action<string> OnFail = null,
                                      Action OnStart = null, Action OnFinish = null)
        {

            using (var client = new HttpClient())
            {

                // Call OnStart() at the beginning
                if (OnStart != null)
                    OnStart();

                try
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new StringContent(json);

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var jsonObject = JSONConverter.JSONToObject(result);
                        OnSuccess(result);
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
