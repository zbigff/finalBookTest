
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities;
using AzureFunctions.Autofac;

namespace ServerlessImageManagement
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class RecognitionStart
    {
        [FunctionName("RecognitionStart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
            [Queue("recognitionqueue", Connection = "ImageStorageAccount")]IAsyncCollector<RecognitionOrder> queueWithRecOrders,
            [Inject] IRecOrderValidator validator, TraceWriter log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            RecognitionOrder recognitionOrder = JsonConvert.DeserializeObject<RecognitionOrder>(requestBody);
            if (!validator.IsValid(recognitionOrder))
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Provided data is invalid");
            }
            await queueWithRecOrders.AddAsync(recognitionOrder);
            await queueWithRecOrders.FlushAsync();
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}

