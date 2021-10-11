using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BWJ.Net.Http.RequestBuilder
{
    public class HttpRequestBuilder
    {
        internal readonly HttpRequestConfiguration _config;

        internal HttpRequestBuilder(
            HttpRequestConfiguration config)
        {
            _config = config;
        }

        public HttpRequestBuilder IncludeHeader(string name, string value)
        {
            value = value ?? string.Empty;
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            _config.Headers.Add(name, value);
            return this;
        }

        public HttpRequestBuilder OnConfiguringRequest(Action<HttpRequestMessage> requestConfigurator)
        {
            if(requestConfigurator is null)
            {
                throw new ArgumentNullException(nameof(requestConfigurator));
            }

            _config.OnConfiguringRequest.Add(requestConfigurator);
            return this;
        }

        public HttpRequestBuilder OnRequestConfigured(Action<HttpRequestMessage> requestConfigurator)
        {
            if (requestConfigurator is null)
            {
                throw new ArgumentNullException(nameof(requestConfigurator));
            }

            _config.OnRequestConfigured.Add(requestConfigurator);
            return this;
        }

        public async Task<HttpResponseMessage> SendAsync()
        {
            Uri url;
            if(Uri.TryCreate(_config.Url, UriKind.Absolute, out url) == false)
            {
                url = new Uri(_config.Client.BaseAddress, _config.Url);
            }
            
            if(_config.QueryParameters is not null)
            {
                var builder = new UriBuilder(url);
                var query = string.Empty;
                using (var content = new FormUrlEncodedContent(_config.QueryParameters))
                {
                    query = await content.ReadAsStringAsync();
                }

                builder.Query = query;
                url = builder.Uri;
            }
            
            using (var req = new HttpRequestMessage(_config.Method, url))
            using (var content = _config.Content)
            {
                _config.OnConfiguringRequest.ForEach(action => action(req));

                foreach (var header in _config.Headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }

                req.Content = content;

                _config.OnRequestConfigured.ForEach(action => action(req));

                return await _config.Client.SendAsync(req);
            }
        }
    }
}
