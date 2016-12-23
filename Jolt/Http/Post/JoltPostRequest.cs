using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Post
{
    public class JoltPostRequest
    {
        private string url;

        public JoltPostRequest(string url)
        {
            this.url = url;
        }

        public JoltPostText AsText(string text)
        {
            return new JoltPostText(url, text);
        }

        public JoltPostJSON AsJSON(object obj)
        {
            var json = JSONConverter.ObjectToJSON(obj);
            return new JoltPostJSON(url, json);
        }

        public JoltPostForm AsForm()
        {
            return new JoltPostForm(url);
        }

        public JoltPostMultipart AsMultipart()
        {
            return new JoltPostMultipart(url);
        }

    }
}
