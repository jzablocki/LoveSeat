using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using LoveSeat.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoveSeat
{
    public class ViewResult<T> : ViewResult
    {
        private readonly IObjectSerializer<T> objectSerializer = null;
        public ViewResult(string responseString, int statusCode, string eTag, IObjectSerializer<T> objectSerializer)
            : base(responseString, statusCode, eTag)
        {
            this.objectSerializer = objectSerializer;
        }

        public IEnumerable<T> Items
        {
            get
            {
                if (objectSerializer == null)
                {
                    throw new InvalidOperationException("ObjectSerializer must be set in order to use the generic view.");
                }
                return this.RawValues.Select(item => objectSerializer.Deserialize(item));
            }
        }
    }



    public class ViewResult
    {
        public string ETag { get; set; }
        private JObject json = null;
        private readonly string responseString;
        private string eTag;
        private HttpStatusCode statusCode;

        public JObject Json { get { return json ?? (json = JObject.Parse(responseString)); } }
        public ViewResult(string responseString, int statusCode, string eTag)
        {
            this.eTag = eTag;
            this.responseString = responseString;
            this.statusCode = (HttpStatusCode) statusCode;
        }

        public HttpStatusCode StatusCode { get { return statusCode; } }

        public string Etag { get { return eTag; } }
        public int TotalRows { get { return Json["total_rows"].Value<int>(); } }
        public int OffSet { get { return Json["offset"].Value<int>(); } }
        public IEnumerable<JToken> Rows { get { return (JArray)Json["rows"]; } }
        /// <summary>
        /// Only populated when IncludeDocs is true
        /// </summary>
        public IEnumerable<JToken> Docs
        {
            get
            {
                return (JArray)Json["doc"];
            }
        }
        /// <summary>
        /// An IEnumerable of strings insteda of the IEnumerable of JTokens
        /// </summary>
        public IEnumerable<string> RawRows
        {
            get
            {
                var arry = (JArray)Json["rows"];
                return arry.Select(item => item.ToString());
            }
        }

        public IEnumerable<string> RawValues
        {
            get
            {
                var arry = (JArray)Json["rows"];
                return arry.Select(item => item["value"].ToString());
            }
        }
        public IEnumerable<string> RawDocs
        {
            get
            {
                var arry = (JArray)Json["rows"];
                return arry.Select(item => item["doc"].ToString());
            }
        }
        public string RawString
        {
            get { return responseString; }
        }
        public override string ToString()
        {
            return responseString;
        }
        /// <summary>
        /// Provides a formatted version of the json returned from this Result.  (Avoid this method in favor of RawString as it's much more performant)
        /// </summary>
        public string FormattedResponse { get { return Json.ToString(Formatting.Indented); } }
    }
}