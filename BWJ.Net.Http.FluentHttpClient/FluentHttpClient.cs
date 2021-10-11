using BWJ.Net.Http.RequestBuilder;
using System.Net.Http;

namespace BWJ.Net.Http
{
    public class FluentHttpClient
    {
        private readonly HttpClient _client;

        public FluentHttpClient()
        {
            _client = new HttpClient();
        }

        public FluentHttpClient(HttpClient client)
        {
            _client = client ?? new HttpClient();
        }

        public virtual HttpRequestWithQueryBuilder Get(string url)
        {
            return new HttpRequestWithQueryBuilder(
                new HttpRequestConfiguration(HttpMethod.Get, url, _client)
                );
        }

        public virtual HttpRequestWithContentBuilder Post(string url)
        {
            return new HttpRequestWithContentBuilder(
                new HttpRequestConfiguration(HttpMethod.Post, url, _client)
                );
        }
    }
}
