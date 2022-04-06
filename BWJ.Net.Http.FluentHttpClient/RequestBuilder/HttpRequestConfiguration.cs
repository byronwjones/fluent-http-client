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
        public int RetryCount { get; set; }
        public int RetryDelay { get; set; }
        public List<int> AcceptStatusCodes { get; set; } = new List<int>();
        public List<int> AcceptStatusCodeSeries { get; } = new List<int>();
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public IEnumerable<KeyValuePair<string, string>> QueryParameters { get; set; } = new Dictionary<string, string>();
        public List<Action<HttpRequestMessage>> OnConfiguringRequest { get; }
            = new List<Action<HttpRequestMessage>>();
        public List<Action<HttpRequestMessage>> OnRequestConfigured { get; }
            = new List<Action<HttpRequestMessage>>();
    }
}
