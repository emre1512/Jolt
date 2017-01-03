using JoltHttp.Ftp;
using JoltHttp.Http.Delete;
using JoltHttp.Http.Get;
using JoltHttp.Http.Head;
using JoltHttp.Http.Post;
using JoltHttp.Http.Put;
using System;

namespace JoltHttp
{

    public class Jolt
    {       
        public static JoltGetRequest GET(string url)
        {
            return new JoltGetRequest(url);
        }

        public static JoltPostRequest POST(string url)
        {
            return new JoltPostRequest(url);
        }

        public static JoltPutRequest PUT(string url)
        {
            return new JoltPutRequest(url);
        }

        public static JoltDeleteRequest DELETE(string url)
        {
            return new JoltDeleteRequest(url);
        }

        public static JoltHeadRequest HEAD(string url)
        {
            return new JoltHeadRequest(url);
        }

        public static JoltFtpUpload Upload(string filePath, string url)
        {
            return new JoltFtpUpload(filePath, url);
        }

        public static JoltFtpUpload Upload(byte[] file, string url)
        {
            return new JoltFtpUpload(file, url);
        }

        public static JoltFtpDownload Download(string filePath, string url)
        {
            return new JoltFtpDownload(filePath, url);
        }

    }   


}
