using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
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

        public HttpRequestBuilder AcceptStatusCodes(params object[] codes)
        {
            _config.AcceptStatusCodes.Clear();
            _config.AcceptStatusCodeSeries.Clear();

            foreach(var code in codes)
            {
                if(code is null)
                {
                    throw new ArgumentNullException("Null parameter encountered");
                }
                else if (code is HttpStatusCode)
                {
                    _config.AcceptStatusCodes.Add((int)code);
                }
                else if (code is string)
                {
                    var series = code.ToString().Trim();
                    if(Regex.IsMatch(series, @"^[1-5]xx$", RegexOptions.IgnoreCase))
                    {
                        _config.AcceptStatusCodeSeries.Add(Convert.ToInt32(series[0]));
                    }
                    else
                    {
                        throw new ArgumentException($"String parameter '{code}' is invalid");
                    }
                }
                else if (IsInteger(code))
                {
                    var errMessage = $"Invalid status code {code}. Acceptable codes are 100-599";
                    try
                    {
                        var intValue = (int)code;
                        if (intValue > 99 && intValue < 600)
                        {
                            _config.AcceptStatusCodes.Add(intValue);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(codes), errMessage);
                        }
                    }
                    catch
                    {
                        throw new ArgumentOutOfRangeException(nameof(codes), errMessage);
                    }
                }
                else
                {
                    throw new ArgumentException($"Status code parameter invalid: {code}");
                }
            }

            return this;
        }

        public HttpRequestBuilder Retry(int numberOfTries, int delayBetweenTriesMs = 500)
        {
            if(numberOfTries < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfTries));
            }
            if(delayBetweenTriesMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delayBetweenTriesMs));
            }

            _config.RetryCount = numberOfTries;
            _config.RetryDelay = delayBetweenTriesMs;
            return this;
        }

        public HttpRequestBuilder IncludeHeaders<T>(T headers)
            where T : class
        {
            var dict = BuilderUtils.ToDictionary(headers);

            foreach (var pair in dict)
            {
                _config.Headers.Add(pair.Key, pair.Value);
            }
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
