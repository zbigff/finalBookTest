using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ServerlessImageManagement
{
    public static class GetImage
    {
        [FunctionName("GetImage")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            Stream photoStream;
            try
            {
                string imagePath = req.GetQueryStrings()["path"];
                var photoBlob = await Utils.BlobClient.GetBlobReferenceFromServerAsync(new Uri(imagePath, UriKind.Absolute));
                photoStream = await photoBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(),
                    new BlobRequestOptions(), new OperationContext());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(photoStream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return result;
        }
    }
}
