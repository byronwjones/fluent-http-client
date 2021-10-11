using System;
using System.Collections.Generic;
using System.Linq;

namespace BWJ.Net.Http.RequestObject
{
    public class FileContent : IFileContent
    {
        public const string DEFAULT_CONTENT_TYPE = "multipart/form-data";
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; } = DEFAULT_CONTENT_TYPE;
        public Dictionary<string, string> AdditionalHeaders { get; } = new Dictionary<string, string>();
    }
}
