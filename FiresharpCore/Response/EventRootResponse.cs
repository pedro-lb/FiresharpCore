using FiresharpCore.EventStreaming;
using FiresharpCore.Extensions;
using FiresharpCore.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiresharpCore.Response
{
    public class EventRootResponse<T> : IDisposable
    {
        private ValueRootAddedEventHandler<T> ValueRootEventHandler { get; }
        private CancellationTokenSource CancellationToken { get; }
        private IRequestManager RequestManager { get; }
        private Task PollingTask { get; }
        private string Path { get; }

        internal EventRootResponse(HttpResponseMessage httpResponse, ValueRootAddedEventHandler<T> added,
            IRequestManager requestManager, string path)
        {
            RequestManager = requestManager;
            ValueRootEventHandler = added;
            Path = path;

            CancellationToken = new CancellationTokenSource();
            PollingTask = ReadLoop(httpResponse, CancellationToken.Token);
        }

        ~EventRootResponse()
        {
            Dispose(false);
        }

        private async Task ReadLoop(HttpResponseMessage httpResponse, CancellationToken token)
        {
            await Task.Factory.StartNew(async () =>
            {
                using (httpResponse)
                {
                    using (var content = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (var streamReader = new StreamReader(content))
                        {
                            string eventName = null;

                            while (true)
                            {
                                CancellationToken.Token.ThrowIfCancellationRequested();

                                var read = await streamReader.ReadLineAsync().ConfigureAwait(false);

                                Debug.WriteLine(read);

                                if (read.StartsWith("event: "))
                                {
                                    eventName = read.Substring(7);
                                    continue;
                                }

                                if (read.StartsWith("data: "))
                                {
                                    if (string.IsNullOrEmpty(eventName))
                                    {
                                        throw new InvalidOperationException("Payload data was received but an event did not preceed it.");
                                    }

                                    // Every change on child, will get entire object again.
                                    var request = await RequestManager.RequestAsync(HttpMethod.Get, Path);
                                    var jsonStr = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

                                    ValueRootEventHandler(this, jsonStr.ReadAs<T>());
                                }

                                // start over
                                eventName = null;
                            }
                        }
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        public void Cancel()
        {
            CancellationToken.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Cancel();

            if (disposing)
            {
                CancellationToken.Dispose();
            }
        }
    }
}