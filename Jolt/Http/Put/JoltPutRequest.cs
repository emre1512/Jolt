using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoltHttp.Http.Put
{
    public class JoltPutRequest
    {
        private string url;

        public JoltPutRequest(string url)
        {
            this.url = url;
        }

        public JoltPutFile AddFile(string filePath)
        {
            return new JoltPutFile(filePath, url);
        }

        public JoltPutFile AddFile(byte[] file)
        {
            return new JoltPutFile(file, url);
        }

    }
}
