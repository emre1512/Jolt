using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Get
{
    public class JoltGetRequest
    {
        private string url;

        public JoltGetRequest(string url)
        {
            this.url = url;
        }

        public JoltGetText AsText()
        {
            return new JoltGetText(url);
        }

        public JoltGetJSON AsJSON()
        {
            return new JoltGetJSON(url);
        }

        public JoltGetFile AsFile()
        {
            return new JoltGetFile(url);
        }

    }
}
