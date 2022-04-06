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
            var dict = BuilderUtils.ToKeyValuePairCollection(headers);

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

        public async Task<TResponse> SendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            Func<Exception, HttpResponseMessage, Task<TResponse>> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        {
            if (onSuccess is null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            if (onError is null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            return await ExecuteSendAsync(
                onSuccess: onSuccess,
                onError: onError,
                cancellationToken: cancellationToken,
                completionOption: completionOption);
        }
        public async Task<TResponse> SendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            Func<Exception, HttpResponseMessage, Task<TResponse>> onError,
            CancellationToken? cancellationToken = null)
        {
            if (onSuccess is null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            if (onError is null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            return await ExecuteSendAsync(
                onSuccess: onSuccess,
                onError: onError,
                cancellationToken: cancellationToken);
        }

        public async Task<TResponse> SendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            Func<Exception, HttpResponseMessage, TResponse> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await SendAsync(
            onSuccess,
            async (exception, response) => await Task.FromResult(onError(exception, response)),
            completionOption,
            cancellationToken);
        public async Task<TResponse> SendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            Func<Exception, HttpResponseMessage, TResponse> onError,
            CancellationToken? cancellationToken = null)
        => await SendAsync(
            onSuccess,
            async (exception, response) => await Task.FromResult(onError(exception, response)),
            cancellationToken);

        public async Task<TResponse> SendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        {
            if (onSuccess is null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            return await ExecuteSendAsync(
                onSuccess: onSuccess,
                onError: null,
                cancellationToken: cancellationToken,
                completionOption: completionOption);
        }
        public async Task<TResponse> SendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            CancellationToken? cancellationToken = null)
        {
            if (onSuccess is null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            return await ExecuteSendAsync(
                onSuccess: onSuccess,
                onError: null,
                cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(
            Func<HttpResponseMessage, Task<HttpResponseMessage>> onResponse,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        {
            if (onResponse is null)
            {
                throw new ArgumentNullException(nameof(onResponse));
            }

            return await ExecuteSendAsync(
                onSuccess: onResponse,
                onError: async (exception, response) => {
                    if(response is not null)
                    {
                        return await onResponse(response);
                    }
                    else
                    {
                        throw exception;
                    }
                },
                cancellationToken: cancellationToken,
                completionOption);
        }
        public async Task<HttpResponseMessage> SendAsync(
            Func<HttpResponseMessage, Task<HttpResponseMessage>> onResponse,
            CancellationToken? cancellationToken = null)
        {
            if (onResponse is null)
            {
                throw new ArgumentNullException(nameof(onResponse));
            }

            return await ExecuteSendAsync(
                onSuccess: onResponse,
                onError: async (exception, response) => {
                    if (response is not null)
                    {
                        return await onResponse(response);
                    }
                    else
                    {
                        throw exception;
                    }
                },
                cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(
            Action<Exception, HttpResponseMessage> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        {
            if (onError is null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            return await ExecuteSendAsync(
                async resp => await Task.FromResult(resp),
                async (exception, response) =>
                {
                    onError(exception, response);
                    return await Task.FromResult(response);
                },
                cancellationToken,
                completionOption);
        }
        public async Task<HttpResponseMessage> SendAsync(
            Action<Exception, HttpResponseMessage> onError,
            CancellationToken? cancellationToken = null)
        {
            if (onError is null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            return await ExecuteSendAsync(
                async resp => await Task.FromResult(resp),
                async (exception, response) =>
                {
                    onError(exception, response);
                    return await Task.FromResult(response);
                },
                cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(
            Func<Exception, HttpResponseMessage, Task> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        {
            if (onError is null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            return await ExecuteSendAsync(
                async resp => await Task.FromResult(resp),
                async (exception, response) =>
                {
                    await onError(exception, response);
                    return response;
                },
                cancellationToken,
                completionOption);
        }
        public async Task<HttpResponseMessage> SendAsync(
            Func<Exception, HttpResponseMessage, Task> onError,
            CancellationToken? cancellationToken = null)
        {
            if (onError is null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            return await ExecuteSendAsync(
                async resp => await Task.FromResult(resp),
                async (exception, response) =>
                {
                    await onError(exception, response);
                    return response;
                },
                cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
            => await ExecuteSendAsync(
                onSuccess: async resp => await Task.FromResult(resp),
                onError: null,
                cancellationToken: cancellationToken,
                completionOption: completionOption);

        public async Task<HttpResponseMessage> SendAsync(
            CancellationToken? cancellationToken = null)
            => await ExecuteSendAsync(
                onSuccess: async resp => await Task.FromResult(resp),
                onError: null,
                cancellationToken);


        private async Task<TResponse> ExecuteSendAsync<TResponse>(
            Func<HttpResponseMessage, Task<TResponse>> onSuccess,
            Func<Exception, HttpResponseMessage, Task<TResponse>> onError,
            CancellationToken? cancellationToken,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
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

                HttpResponseMessage resp = null;
                Exception exception = null;
                for(var retry = 0; retry < (_config.RetryCount + 1); retry++)
                {
                    resp = null;
                    exception = null;
                    try
                    {
                        resp = cancellationToken.HasValue ?
                            await _config.Client.SendAsync(req, completionOption, cancellationToken.Value) :
                            await _config.Client.SendAsync(req, completionOption);

                        var statusCodeSeries = (int)resp.StatusCode / 100;
                        // if acceptable status code(s) were never explicitly defined by the developer,
                        // the default is 2xx
                        if(_config.AcceptStatusCodes.Count == 0 &&
                            _config.AcceptStatusCodeSeries.Count == 0)
                        {
                            _config.AcceptStatusCodeSeries.Add(2);
                        }

                        if(false == _config.AcceptStatusCodeSeries.Any(s=>s == statusCodeSeries) &&
                            false == _config.AcceptStatusCodes.Any(s=>s == (int)resp.StatusCode))
                        {
                            var requestException = new HttpRequestException($"Unacceptable status code {(int)resp.StatusCode} ({resp.StatusCode})");
                            resp.StatusCode = resp.StatusCode;

                            // unless the status code is 500 or 503, which could happen with a temporary
                            // glitch on the host, there's no point in retrying invalid status codes
                            // e.g. a bad request won't become a good one if you just retry
                            if((new HttpStatusCode[] { HttpStatusCode.InternalServerError, HttpStatusCode.ServiceUnavailable})
                                .Any(code=> code == resp.StatusCode))
                            {
                                throw requestException;
                            }
                            else
                            {
                                exception = requestException;
                                break;
                            }
                        }

                        return await onSuccess(resp);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        Thread.Sleep(_config.RetryDelay);
                    }
                }

                if (onError is not null)
                {
                    return await onError(exception, resp);
                }
                else
                {
                    throw exception;
                }
            }
        }

        private static bool IsInteger(object obj)
        {
            return obj is int ||
                obj is uint ||
                obj is byte ||
                obj is sbyte ||
                obj is short ||
                obj is ushort ||
                obj is long ||
                obj is ulong;
        }
    }
}
