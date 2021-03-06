﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MyCouch.Extensions;

namespace MyCouch.Net
{
    public class BasicHttpClientConnection : IConnection
    {
        protected HttpClient HttpClient { get; private set; }
        protected bool IsDisposed { get; private set; }

        public Uri Address
        {
            get { return HttpClient.BaseAddress; }
        }

        public BasicHttpClientConnection(Uri dbUri)
        {
            Ensure.That(dbUri, "dbUri").IsNotNull();

            HttpClient = CreateHttpClient(dbUri);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ThrowIfDisposed();

            IsDisposed = true;

            if (!disposing)
                return;

            HttpClient.CancelPendingRequests();
            HttpClient.Dispose();
            HttpClient = null;
        }

        protected virtual void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private HttpClient CreateHttpClient(Uri dbUri)
        {
            var client = new HttpClient { BaseAddress = new Uri(BuildCleanUrl(dbUri)) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentTypes.Json));

            if (!string.IsNullOrWhiteSpace(dbUri.UserInfo))
            {
                var parts = dbUri.UserInfo
                    .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => Uri.UnescapeDataString(p))
                    .ToArray();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", string.Join(":", parts).AsBase64Encoded());
            }

            return client;
        }

        private string BuildCleanUrl(Uri uri)
        {
            EnsureValidUri(uri);

            var url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, uri.LocalPath);
            while (url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);

            return url;
        }

        private void EnsureValidUri(Uri uri)
        {
            Ensure.That(uri, "uri").IsNotNull();
            Ensure.That(uri.LocalPath, "uri.LocalPath")
                  .IsNotNullOrEmpty()
                  .WithExtraMessageOf(() => ExceptionStrings.BasicHttpClientConnectionUriIsMissingDb);
        }

        public virtual async Task<HttpResponseMessage> SendAsync(HttpRequest httpRequest)
        {
            ThrowIfDisposed();

            return await HttpClient.SendAsync(OnBeforeSend(httpRequest)).ForAwait();
        }

        public virtual async Task<HttpResponseMessage> SendAsync(HttpRequest httpRequest, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return await HttpClient.SendAsync(OnBeforeSend(httpRequest), cancellationToken).ForAwait();
        }

        public virtual async Task<HttpResponseMessage> SendAsync(HttpRequest httpRequest, HttpCompletionOption completionOption)
        {
            ThrowIfDisposed();

            return await HttpClient.SendAsync(OnBeforeSend(httpRequest), completionOption).ForAwait();
        }

        public virtual async Task<HttpResponseMessage> SendAsync(HttpRequest httpRequest, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return await HttpClient.SendAsync(OnBeforeSend(httpRequest), completionOption, cancellationToken).ForAwait();
        }

        protected virtual HttpRequest OnBeforeSend(HttpRequest httpRequest)
        {
            ThrowIfDisposed();

            return httpRequest.RemoveRequestType();
        }
    }
}