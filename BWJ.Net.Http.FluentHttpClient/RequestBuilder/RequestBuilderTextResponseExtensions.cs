using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BWJ.Net.Http.RequestBuilder
{
    public static class RequestBuilderTextResponseExtensions
    {
        public static async Task<string> SendForTextAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Task<string>> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStringAsync(),
                onError: onError,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<string> SendForTextAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Task<string>> onError,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStringAsync(),
                onError: onError,
                cancellationToken: cancellationToken);

        public static async Task<string> SendForTextAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, string> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStringAsync(),
                onError: onError,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<string> SendForTextAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, string> onError,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStringAsync(),
                onError: onError,
                cancellationToken: cancellationToken);

        public static async Task<string> SendForTextAsync(
            this HttpRequestBuilder builder,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStringAsync(),
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<string> SendForTextAsync(
            this HttpRequestBuilder builder,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStringAsync(),
                cancellationToken: cancellationToken);
    }
}
