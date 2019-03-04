using FiresharpCore.EventStreaming;
using FiresharpCore.Exceptions;
using FiresharpCore.Interfaces;
using FiresharpCore.Response;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FiresharpCore
{
    public class FirebaseClient : IFirebaseClient
    {
        public FirebaseClient(IFirebaseConfig config)
            : this(new RequestManager(config))
        {
        }

        internal FirebaseClient(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }

        ~FirebaseClient()
        {
            Dispose(false);
        }

        private readonly IRequestManager RequestManager;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                RequestManager.Dispose();
            }
        }

        private readonly Action<HttpStatusCode, string> DefaultErrorHandler = (statusCode, body) =>
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
                throw new FirebaseException(statusCode, body);
            }
        };

        public FirebaseResponse Get(string path)
        {
            try
            {
                using (var response = RequestManager.RequestAsync(HttpMethod.Get, path).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public FirebaseResponse Get(string path, QueryBuilder queryBuilder)
        {
            try
            {
                using (var response = RequestManager.RequestAsync(HttpMethod.Get, path, queryBuilder).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public SetResponse Set<T>(string path, T data)
        {
            try
            {
                using (var response = RequestManager.RequestAsync(HttpMethod.Put, path, data).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new SetResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public SetResponse Set<T>(string path, T data, string print)
        {
            try
            {
                var queryBuilder = QueryBuilder.New().Print(print);

                using (var response = RequestManager.RequestAsync(HttpMethod.Put, path, queryBuilder, data).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new SetResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public PushResponse Push<T>(string path, T data)
        {
            try
            {
                using (var response = RequestManager.RequestAsync(HttpMethod.Post, path, data).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);
                    return new PushResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public FirebaseResponse Delete(string path)
        {
            try
            {
                using (var response = RequestManager.RequestAsync(HttpMethod.Delete, path).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public FirebaseResponse Update<T>(string path, T data)
        {
            try
            {
                using (var response = RequestManager.RequestAsync(FiresharpCore.RequestManager.PatchMethod, path, data).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public FirebaseResponse Update<T>(string path, T data, string print)
        {
            try
            {
                var queryBuilder = QueryBuilder.New().Print(print);

                using (var response = RequestManager.RequestAsync(FiresharpCore.RequestManager.PatchMethod, path, queryBuilder, data).Result)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<FirebaseResponse> GetAsync(string path, QueryBuilder queryBuilder)
        {
            try
            {
                using (var response = await RequestManager.RequestAsync(HttpMethod.Get, path, queryBuilder).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<FirebaseResponse> GetAsync(string path)
        {
            try
            {
                using (var response = await RequestManager.RequestAsync(HttpMethod.Get, path).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<SetResponse> SetAsync<T>(string path, T data)
        {
            try
            {
                using (var response = await RequestManager.RequestAsync(HttpMethod.Put, path, data).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new SetResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<SetResponse> SetAsync<T>(string path, T data, string print)
        {
            try
            {
                var queryBuilder = QueryBuilder.New().Print(print);

                using (var response = await RequestManager.RequestAsync(HttpMethod.Put, path, queryBuilder, data).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new SetResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<PushResponse> PushAsync<T>(string path, T data)
        {
            try
            {
                using (var response = await RequestManager.RequestAsync(HttpMethod.Post, path, data).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new PushResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<FirebaseResponse> DeleteAsync(string path)
        {
            try
            {
                using (var response = await RequestManager.RequestAsync(HttpMethod.Delete, path).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<FirebaseResponse> UpdateAsync<T>(string path, T data)
        {
            try
            {
                using (var response = await RequestManager.RequestAsync(FiresharpCore.RequestManager.PatchMethod, path, data).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<FirebaseResponse> UpdateAsync<T>(string path, T data, string print)
        {
            try
            {
                var queryBuilder = QueryBuilder.New().Print(print);

                using (var response = await RequestManager.RequestAsync(FiresharpCore.RequestManager.PatchMethod, path, queryBuilder, data).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    HandleIfErrorResponse(response.StatusCode, content);

                    return new FirebaseResponse(content, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new FirebaseException(ex);
            }
        }

        public async Task<EventRootResponse<T>> OnChangeGetAsync<T>(string path,
            ValueRootAddedEventHandler<T> added = null)
        {
            return new EventRootResponse<T>(
                await RequestManager
                    .ListenAsync(path)
                    .ConfigureAwait(false),
                added,
                RequestManager,
                path
            );
        }

        public async Task<EventStreamResponse> OnAsync(string path, ValueAddedEventHandler added = null,
            ValueChangedEventHandler changed = null,
            ValueRemovedEventHandler removed = null, object context = null)
        {
            return new EventStreamResponse(
                await RequestManager
                    .ListenAsync(path)
                    .ConfigureAwait(false),
                added,
                changed,
                removed,
                context
            );
        }

        private void HandleIfErrorResponse(HttpStatusCode statusCode, string content,
            Action<HttpStatusCode, string> errorHandler = null)
        {
            if (errorHandler != null)
            {
                errorHandler(statusCode, content);
            }
            else
            {
                DefaultErrorHandler(statusCode, content);
            }
        }
    }
}