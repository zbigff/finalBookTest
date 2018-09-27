using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Primitives;

namespace ServerlessImageManagementTests.TestHelpers
{
    public abstract class FunctionTest
    {

        protected TraceWriter log = new VerboseDiagnosticsTraceWriter();

        public HttpRequestMessage HttpRequestSetup(string json, HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(requestUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            return request;
        }

    }

    public class AsyncCollector<T> : IAsyncCollector<T>
    {
        public readonly List<T> Items = new List<T>();

        public Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {

            Items.Add(item);

            return Task.FromResult(true);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}

