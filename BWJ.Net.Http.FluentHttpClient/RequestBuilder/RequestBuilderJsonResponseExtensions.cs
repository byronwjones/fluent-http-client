using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BWJ.Net.Http.RequestBuilder
{
    public static class RequestBuilderJsonResponseExtensions
    {
        public static async Task<T> SendForJsonAsync<T>(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Task<T>> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: GetJsonContent<T>,
                onError: onError,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<T> SendForJsonAsync<T>(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Task<T>> onError,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: GetJsonContent<T>,
                onError: onError,
                cancellationToken: cancellationToken);

        public static async Task<T> SendForJsonAsync<T>(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, T> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: GetJsonContent<T>,
                onError: onError,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<T> SendForJsonAsync<T>(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, T> onError,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: GetJsonContent<T>,
                onError: onError,
                cancellationToken: cancellationToken);

        public static async Task<T> SendForJsonAsync<T>(
            this HttpRequestBuilder builder,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: GetJsonContent<T>,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<T> SendForJsonAsync<T>(
            this HttpRequestBuilder builder,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: GetJsonContent<T>,
                cancellationToken: cancellationToken);

        private static async Task<T> GetJsonContent<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
