using FiresharpCore.Interfaces;
using System;

namespace FiresharpCore.Config
{
    public class FirebaseConfig : IFirebaseConfig
    {
        public FirebaseConfig()
        {
            Serializer = new JsonNetSerializer();
        }

        public string BasePath
        {
            get
            {
                return RawBasePath.EndsWith("/") ? RawBasePath : $"{RawBasePath}/";
            }
            set
            {
                RawBasePath = value;
            }
        }

        private string RawBasePath { get; set; }

        public string Host { get; set; }
        public string AuthSecret { get; set; }
        public string AccessToken { get; set; }

        public TimeSpan? RequestTimeout { get; set; }

        public ISerializer Serializer { get; set; }
    }
}