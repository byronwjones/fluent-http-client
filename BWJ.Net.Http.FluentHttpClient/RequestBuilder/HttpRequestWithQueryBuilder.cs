using System;
using System.Collections.Generic;
using System.Text;

namespace BWJ.Net.Http.RequestBuilder
{
    public sealed class HttpRequestWithQueryBuilder : HttpRequestBuilder
    {
        internal HttpRequestWithQueryBuilder(HttpRequestConfiguration config) : base(config) { }

        public HttpRequestBuilder IncludeQuery<T>(T queryParameters)
            where T : class
        {
            BuilderUtils.NotNullOrEnumerable(queryParameters, nameof(queryParameters));

            _config.QueryParameters = BuilderUtils.ToDictionary(queryParameters);

            return new HttpRequestBuilder(_config);
        }
    }
}
