using System;
using System.Web.Script.Serialization;

namespace JoltHttp.Utils
{
    class JSONConverter
    {

        public static string ObjectToJSON(object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public static object JSONToObject(string json)
        {
            return new JavaScriptSerializer().DeserializeObject(json);
        }

    }
}
