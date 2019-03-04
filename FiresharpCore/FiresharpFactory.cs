using FiresharpCore.Config;
using FiresharpCore.Interfaces;
using System;
using System.Security;

namespace FiresharpCore
{
    public static class FiresharpFactory
    {
        private static bool HasConfigured { get; set; } = false;

        private static SecureString AuthSecret { get; set; }
        private static SecureString BasePath { get; set; }

        public static void Configure(string basePath, string authSecret)
        {
            authSecret = authSecret ?? throw new ArgumentNullException(nameof(authSecret));
            basePath = basePath ?? throw new ArgumentException(nameof(basePath));

            foreach (var toAdd in basePath)
            {
                BasePath.AppendChar(toAdd);
            }

            foreach (var toAdd in authSecret)
            {
                AuthSecret.AppendChar(toAdd);
            }

            AuthSecret.MakeReadOnly();
            BasePath.MakeReadOnly();

            HasConfigured = true;
        }

        public static IFirebaseClient Create(string basePath, string authSecret)
        {
            if (!HasConfigured)
            {
                Configure(basePath, authSecret);
            }

            return Create();
        }

        public static IFirebaseClient Create()
        {
            AuthSecret = AuthSecret ?? throw new ArgumentException($"AuthSecret must be informed before creating connections.");
            BasePath = BasePath ?? throw new ArgumentException($"BasePath must be informed before creating connections.");

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = AuthSecret.ToString(),
                BasePath = BasePath.ToString(),
            };

            return new FirebaseClient(config);
        }
    }
}