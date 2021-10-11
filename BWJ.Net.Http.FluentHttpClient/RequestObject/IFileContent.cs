using System.Collections.Generic;

namespace BWJ.Net.Http.RequestObject
{
    public interface IFileContent
    {
        Dictionary<string, string> AdditionalHeaders { get; }
        byte[] Content { get; set; }
        string ContentType { get; set; }
        string FileName { get; set; }
    }
}