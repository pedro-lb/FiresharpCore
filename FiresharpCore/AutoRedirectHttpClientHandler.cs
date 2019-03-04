using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiresharpCore
{
    internal class AutoRedirectHttpClientHandler : DelegatingHandler
    {
        private int MaximumAutomaticRedirections
        {
            get
            {
                return _MaximumAutomaticRedirections;
            }
            set
            {
                _MaximumAutomaticRedirections = Math.Max(1, value);
            }
        }

        public int _MaximumAutomaticRedirections { get; set; }

        public AutoRedirectHttpClientHandler()
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = false };

            MaximumAutomaticRedirections = handler.MaxAutomaticRedirections;
            InnerHandler = handler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!IsRedirect(response))
            {
                return response;
            }

            var redirectCount = 0;

            while (IsRedirect(response))
            {
                redirectCount++;

                if (redirectCount > MaximumAutomaticRedirections)
                {
                    throw new WebException("Too many automatic redirections were attempted.");
                }

                request.RequestUri = response.Headers.Location;
                response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

        private static bool IsRedirect(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.RedirectMethod:
                case HttpStatusCode.Redirect:
                    return true;

                default:
                    return false;
            }
        }
    }
}