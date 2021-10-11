using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BWJ.Net.Http.RequestBuilder
{
    internal class HttpRequestConfiguration
    {
        internal HttpRequestConfiguration(HttpMethod method, string url, HttpClient client)
        {
            Method = method;
            Url = url;
            Client = client;
        }

        public HttpClient Client { get; }
        public HttpMethod Method { get; }
        public string Url { get; }
        public HttpContent Content { get; set; }
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> QueryParameters { get; set; }
        public List<Action<HttpRequestMessage>> OnConfiguringRequest { get; }
            = new List<Action<HttpRequestMessage>>();
        public List<Action<HttpRequestMessage>> OnRequestConfigured { get; }
            = new List<Action<HttpRequestMessage>>();
    }
}
