using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using SeasideResearch.LibCurlNet;

namespace LoveSeat
{
    public static class Fetcher
    {
        private static StringWriter headerWriter = null;
        private static StringWriter dataWriter = null;
        /// <summary>
        /// Are you sure you don't need any headers?  
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Result Fetch(Uri uri)
        {
            return Fetch(uri, new string[]{});
        }
        public static Result Fetch(Uri uri, string[] headers)
        {
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

            var easy = new Easy();
            headerWriter = new StringWriter();
            dataWriter = new StringWriter();
            var wf = new Easy.WriteFunction(OnWriteData);
            var hf = new Easy.HeaderFunction(OnHeaderData);
            easy.SetOpt(CURLoption.CURLOPT_URL,  uri.AbsoluteUri);
            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
          //  easy.SetOpt(CURLoption.CURLOPT_WRITEDATA, uri.Query);
            easy.SetOpt(CURLoption.CURLOPT_HEADERFUNCTION, hf);
            var slist = new Slist();
            foreach (var header in headers)
                slist.Append(header);
            easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, slist);
            easy.Perform();
            easy.Cleanup();

            Curl.GlobalCleanup();
            var result = new Result();
            result.Response = dataWriter.ToString();
            var rows = headerWriter.ToString().Split('\n');
            var nvc = new NameValueCollection();
            foreach (var row in rows)
            {
                if (row.Contains(":"))
                {
                    var tmp = row.Split(':');
                    nvc.Add(tmp[0].Trim(), tmp[1].Trim());
                }
            }
            result.Headers = nvc;
            return result;
        }

        public static Int32 OnHeaderData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            headerWriter.Write(System.Text.Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        private static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            dataWriter.Write(System.Text.Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

    }

    public class Result
    {
        public int StatusCode { get; set; }
        public NameValueCollection Headers { get; set; }
        public string Etag
        {
            get
            {
                if (Headers.Get("Etag") != null)
                    return Headers["Etag"];
                return null;
            }
        }
        public string Response { get; set; }
    }
}