using System;
using System.Collections.Generic;
using System.Linq;
using Facebook;
using Facebook.Web;

namespace Ak.Facebook
{
    ///<summary>
    /// To Get Data from Facebook
    ///</summary>
    [Obsolete]
    public class Communication
    {
        ///<summary>
        /// Get just one Data Node
        ///</summary>
        ///<param name="page">The path of the page of data</param>
        public static JsonObject GetFBData(String page)
        {
            var client = new FacebookWebClient();

            return client.Get(page) as JsonObject;
        }

        ///<summary>
        /// Get a list of Data Nodes
        ///</summary>
        ///<param name="page">The path of the page of data</param>
        public static IEnumerable<JsonObject> GetFBListOfData(String page)
        {
            var result = GetFBData(page);

            var jsonArray = result["data"] as JsonArray;
            
            // ReSharper disable AssignNullToNotNullAttribute
            var jsonObjectList = jsonArray.Select(a => (JsonObject)a);
            // ReSharper restore AssignNullToNotNullAttribute

            return jsonObjectList;
        }
    }
}
