using BWJ.Net.Http.RequestObject;
using System;
using System.Linq;

namespace BWJ.Net.Http.Validation
{
    internal static class FileContentValidator
    {
        public static void AssertIsValid(IFileContent fileContent, string propertyName)
        {
            if (fileContent is null)
            {
                throw new ArgumentNullException(propertyName);
            }

            if (fileContent.Content is null)
            {
                throw new ArgumentException("File content cannot be null", propertyName);
            }

            if (string.IsNullOrWhiteSpace(fileContent.ContentType))
            {
                throw new ArgumentException("Content type property cannot be null or empty", propertyName);
            }

            if(fileContent.AdditionalHeaders is not null)
            {
                if (fileContent.AdditionalHeaders.Keys.Any(k => k.Equals("content-disposition", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException("Header Content-Disposition can not be explicitly set", propertyName);
                }
                if (fileContent.AdditionalHeaders.Keys.Any(k => k.Equals("content-type", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException("Header Content-Type can not be explicitly set", propertyName);
                }
            }
        }
    }
}
