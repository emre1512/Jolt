using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Get
{
    public class JoltGetText
    {
        private string url;

        public JoltGetText(string url)
        {
            this.url = url;
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
                    var result = await client.GetStringAsync(url);
                    OnSuccess(result);
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

    }
}
