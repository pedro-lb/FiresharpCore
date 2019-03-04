using FiresharpCore.Exceptions;
using FiresharpCore.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FiresharpCore
{
    internal class RequestManager : IRequestManager
    {
        internal static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");

        private readonly IFirebaseConfig FirebaseConfig;

        private readonly HttpClient HttpClient;

        internal RequestManager(IFirebaseConfig config)
        {
            FirebaseConfig = config ?? throw new ArgumentNullException(nameof(config));

            HttpClient = new HttpClient(new AutoRedirectHttpClientHandler());

            var basePath = FirebaseConfig.BasePath.EndsWith("/") ? FirebaseConfig.BasePath : FirebaseConfig.BasePath + "/";

            HttpClient.BaseAddress = new Uri(basePath);

            if (FirebaseConfig.RequestTimeout.HasValue)
            {
                HttpClient.Timeout = FirebaseConfig.RequestTimeout.Value;
            }
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public async Task<HttpResponseMessage> ListenAsync(string path)
        {
            var client = PrepareEventStreamRequest(path, null, out HttpRequestMessage request);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> ListenAsync(string path, QueryBuilder queryBuilder)
        {
            var client = PrepareEventStreamRequest(path, queryBuilder, out HttpRequestMessage request);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, object payload)
        {
            return RequestAsync(method, path, null, payload);
        }

        public Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, QueryBuilder queryBuilder, object payload = null)
        {
            try
            {
                var uri = PrepareUri(path, queryBuilder);
                var request = PrepareRequest(method, uri, payload);

                return GetClient().SendAsync(request, HttpCompletionOption.ResponseContentRead);
            }
            catch (Exception ex)
            {
                throw new FirebaseException($"An error occured while execute request. Path : {path} , Method : {method}", ex);
            }
        }

        public Task<HttpResponseMessage> RequestApiAsync(HttpMethod method, string path, QueryBuilder queryBuilder, object payload = null)
        {
            try
            {
                var uri = PrepareApiUri(path, queryBuilder);
                var request = PrepareRequest(method, uri, payload);

                return GetClient().SendAsync(request, HttpCompletionOption.ResponseContentRead);
            }
            catch (Exception ex)
            {
                throw new FirebaseException($"An error occured while execute request. Path : {path} , Method : {method}", ex);
            }
        }

        private HttpClient GetClient()
        {
            return HttpClient;
        }

        private HttpClient PrepareEventStreamRequest(string path, QueryBuilder queryBuilder, out HttpRequestMessage request)
        {
            var client = GetClient();
            var uri = PrepareUri(path, queryBuilder);

            request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

            if (!string.IsNullOrEmpty(FirebaseConfig.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", FirebaseConfig.AccessToken);
            }

            return client;
        }

        private HttpRequestMessage PrepareRequest(HttpMethod method, Uri uri, object payload)
        {
            var request = new HttpRequestMessage(method, uri);

            if (payload != null)
            {
                request.Content = new StringContent(payload as string ?? FirebaseConfig.Serializer.Serialize(payload));
            }

            if (!string.IsNullOrEmpty(FirebaseConfig.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", FirebaseConfig.AccessToken);
            }

            return request;
        }

        private Uri PrepareUri(string path, QueryBuilder queryBuilder)
        {
            var authToken = !string.IsNullOrWhiteSpace(FirebaseConfig.AuthSecret)
                ? $"{path}.json?auth={FirebaseConfig.AuthSecret}"
                : $"{path}.json?";

            var queryStr = string.Empty;

            if (queryBuilder != null)
            {
                queryStr = $"&{queryBuilder.ToQueryString()}";
            }

            var url = $"{FirebaseConfig.BasePath}{authToken}{queryStr}";

            return new Uri(url);
        }

        private Uri PrepareApiUri(string path, QueryBuilder queryBuilder)
        {
            string uriString = $"https://auth.firebase.com/v2/{FirebaseConfig.Host}/{path}?{queryBuilder.ToQueryString()}";
            return new Uri(uriString);
        }
    }
}