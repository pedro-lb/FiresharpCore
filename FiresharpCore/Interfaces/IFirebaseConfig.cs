using System;

namespace FiresharpCore.Interfaces
{
    public interface IFirebaseConfig
    {
        string BasePath { get; set; }
        string Host { get; set; }
        string AuthSecret { get; set; }
        string AccessToken { get; set; }
        TimeSpan? RequestTimeout { get; set; }
        ISerializer Serializer { get; set; }
    }
}