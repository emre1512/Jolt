using System;

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
