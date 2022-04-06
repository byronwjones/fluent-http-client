using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BWJ.Net.Http.RequestBuilder
{
    public static class RequestBuilderStreamResponseExtensions
    {
        public static async Task<Stream> SendForStreamAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Task<Stream>> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStreamAsync(),
                onError: onError,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<Stream> SendForStreamAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Task<Stream>> onError,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStreamAsync(),
                onError: onError,
                cancellationToken: cancellationToken);

        public static async Task<Stream> SendForStreamAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Stream> onError,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStreamAsync(),
                onError: onError,
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<Stream> SendForStreamAsync(
            this HttpRequestBuilder builder,
            Func<Exception, HttpResponseMessage, Stream> onError,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStreamAsync(),
                onError: onError,
                cancellationToken: cancellationToken);

        public static async Task<Stream> SendForStreamAsync(
            this HttpRequestBuilder builder,
            HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStreamAsync(),
                completionOption: completionOption,
                cancellationToken: cancellationToken);

        public static async Task<Stream> SendForStreamAsync(
            this HttpRequestBuilder builder,
            CancellationToken? cancellationToken = null)
        => await builder.SendAsync(
                onSuccess: async (resp) => await resp.Content.ReadAsStreamAsync(),
                cancellationToken: cancellationToken);
    }
}
