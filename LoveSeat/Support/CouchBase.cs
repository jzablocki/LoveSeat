using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace LoveSeat.Support
{
    public abstract class CouchBase
    {
        protected readonly string username;
        protected readonly string password;
        protected string baseUri;

        protected CouchBase()
        {
            throw new Exception("Should not be used.");
        }
        protected CouchBase(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        public static bool Authenticate(string baseUri, string userName, string password)
        {
            if (!baseUri.Contains("http://"))
                baseUri = "http://" + baseUri;
            var request = new CouchRequest(baseUri + "/_session");
            request.Timeout = 3000;
            var response = request.Post()
                .ContentType("application/x-www-form-urlencoded")
                .Data("name=" + userName + "&password=" + password)
                .GetResponse();
            return response.StatusCode == HttpStatusCode.OK;
        }

        protected Cookie GetSession()
        {
            if (string.IsNullOrEmpty(username)) return null;
            var request = new CouchRequest(baseUri + "_session");
            var response = request.Post()
                .ContentType("application/x-www-form-urlencoded")
                .Data("name=" + username + "&password=" + password)
                .GetResponse();

            var header = response.Headers.Get("Set-Cookie");
            if (header != null)
            {
                var parts = header.Split(';')[0].Split('=');
                var authCookie = new Cookie(parts[0], parts[1]);
                authCookie.Domain = response.Server;
                return authCookie;
            }
            return null;
        }

        protected string[] GetHeaders(params string[] headersToAdd)
        {
            var headers = new List<string>();
            headers.Add("Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
            headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3");
            headers.Add("Accept-Language: en-us");
            headers.Add("Connection: keep-alive");
            var cookie = GetSession();
            if (cookie != null)
                headers.Add("Cookie: " + cookie.Name + "=" + cookie.Value);
            /*
             *   request.Headers.Add("Accept-Charset", "utf-8");
            request.Headers.Add("Accept-Language", "en-us");
			request.Referer = uri;
			request.ContentType = "application/json";
			request.KeepAlive = true;
			if (authCookie != null)
				request.Headers.Add("Cookie", "AuthSession=" + authCookie.Value);
		    */
            headers.AddRange(headersToAdd);
            headers.Sort();
            return headers.ToArray();
        }

        protected CouchRequest GetRequest(string uri)
        {
            return GetRequest(uri, null);
        }
        protected CouchRequest GetRequest(string uri, string etag)
        {
            var request = new CouchRequest(uri, GetSession(), etag);
            return request;
        }
    }
}