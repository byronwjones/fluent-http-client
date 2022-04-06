using BWJ.Net.Http.RequestObject;
using BWJ.Net.Http.Validation;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BWJ.Net.Http.RequestBuilder
{
    public sealed class HttpRequestWithContentBuilder
    {
        private readonly HttpRequestConfiguration _config;

        internal HttpRequestWithContentBuilder(
            HttpRequestConfiguration config)
        {
            _config = config;
        }

        public HttpRequestWithQueryBuilder Form<T>(T form)
            where T: class
        {
            if(form.GetType().GetProperties().Any(p => p.PropertyType == typeof(FileContent)))
            {
                return MultipartForm(form);
            }
            else
            {
                return UrlEncodedForm(form);
            }
        }

        public HttpRequestWithQueryBuilder UrlEncodedForm<T>(T form)
            where T : class
        {
            BuilderUtils.NotNullOrEnumerable(form, nameof(form));

            var dict = BuilderUtils.ToKeyValuePairCollection(form);
            _config.Content = new FormUrlEncodedContent(dict);

            return new HttpRequestWithQueryBuilder(_config);
        }

        public HttpRequestWithQueryBuilder MultipartForm<T>(T form)
            where T : class
        {
            BuilderUtils.NotNullOrEnumerable(form, nameof(form));

            var formContent = new MultipartFormDataContent();
            var formType = form.GetType();
            var formProps = formType.GetProperties();
            foreach (var prop in formProps)
            {
                var name = BuilderUtils.GetFormName(prop);
                if(typeof(IFileContent).IsAssignableFrom(prop.PropertyType))
                {
                    var value = prop.GetValue(form) as IFileContent;
                    FileContentValidator.AssertIsValid(value, prop.Name);

                    var data = new ByteArrayContent(value.Content);
                    data.Headers.ContentType = MediaTypeHeaderValue.Parse(value.ContentType);
                    
                    if(value.AdditionalHeaders is not null)
                    {
                        foreach (var header in value.AdditionalHeaders)
                        {
                            data.Headers.Add(header.Key, header.Value);
                        }
                    }
                    
                    if(!string.IsNullOrWhiteSpace(value.FileName))
                    {
                        formContent.Add(data, name, value.FileName);
                    }
                    else
                    {
                        formContent.Add(data, name);
                    }
                }
                else
                {
                    formContent.Add(new StringContent(prop.GetValue(form).ToString()), name);
                }
            }

            _config.Content = formContent;
            return new HttpRequestWithQueryBuilder(_config);
        }
    }
}
