using System;

namespace JoltHttp.Http.Put
{
    public class JoltPutRequest
    {
        private string url;

        public JoltPutRequest(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Adds a file from file path. This method reads file all at once.
        /// This can cause memory problems for large files. If file is too large,
        /// you can read it in your own way and pass it as byte array.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        public JoltPutFile AddFile(string filePath)
        {
            return new JoltPutFile(filePath, url);
        }

        /// <summary>
        /// Adds a file as byte array.
        /// </summary>
        /// <param name="file">File to send as byte array.</param>
        public JoltPutFile AddFile(byte[] file)
        {
            return new JoltPutFile(file, url);
        }

    }
}
