using FiresharpCore.EventStreaming;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiresharpCore.Response
{
    public class EventStreamResponse : IDisposable
    {
        private CancellationTokenSource CancellationToken { get; }
        private TemporaryCache TemporaryCache { get; }
        public Task PollingTask { get; }

        internal EventStreamResponse(
            HttpResponseMessage httpResponse,
            ValueAddedEventHandler added = null,
            ValueChangedEventHandler changed = null,
            ValueRemovedEventHandler removed = null,
            object context = null
        )
        {
            CancellationToken = new CancellationTokenSource();

            TemporaryCache = new TemporaryCache();

            if (added != null)
            {
                TemporaryCache.Added += added;
            }
            if (changed != null)
            {
                TemporaryCache.Changed += changed;
            }
            if (removed != null)
            {
                TemporaryCache.Removed += removed;
            }
            if (context != null)
            {
                TemporaryCache.Context = context;
            }

            PollingTask = ReadLoop(httpResponse, CancellationToken.Token);
        }

        ~EventStreamResponse()
        {
            Dispose(false);
        }

        private async Task ReadLoop(HttpResponseMessage httpResponse, CancellationToken token)
        {
            await Task.Factory.StartNew(async () =>
            {
                using (httpResponse)
                {
                    using (var content = await httpResponse.Content.ReadAsStreamAsync())
                    {
                        using (var streamReader = new StreamReader(content))
                        {
                            string eventName = null;

                            while (true)
                            {
                                token.ThrowIfCancellationRequested();

                                var read = await streamReader.ReadLineAsync();

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

                                    Update(eventName, read.Substring(6));
                                }

                                // start over
                                eventName = null;
                            }
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning).Unwrap();
        }

        public void Cancel()
        {
            CancellationToken.Cancel();
        }

        private void Update(string eventName, string p)
        {
            switch (eventName)
            {
                case "put":
                case "patch":
                    using (var reader = new JsonTextReader(new StringReader(p)))
                    {
                        reader.DateParseHandling = DateParseHandling.None;
                        ReadToNamedPropertyValue(reader, "path");
                        reader.Read();

                        var path = reader.Value.ToString();

                        if (eventName == "put")
                        {
                            TemporaryCache.Replace(path, ReadToNamedPropertyValue(reader, "data"));
                        }
                        else
                        {
                            TemporaryCache.Update(path, ReadToNamedPropertyValue(reader, "data"));
                        }
                    }
                    break;
            }
        }

        private JsonReader ReadToNamedPropertyValue(JsonReader reader, string property)
        {
            while (reader.Read() && reader.TokenType != JsonToken.PropertyName)
            {
                // skip the property
            }

            var prop = reader.Value.ToString();

            if (property != prop)
            {
                throw new InvalidOperationException("Error parsing response. Expected json property named: " + property);
            }

            return reader;
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
                TemporaryCache.Dispose();
                CancellationToken.Dispose();
            }
        }
    }
}