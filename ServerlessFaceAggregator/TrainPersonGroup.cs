
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class TrainPersonGroup
    {
        [FunctionName("TrainPersonGroup")]
        public async static Task Run([ActivityTrigger] string personGroupId)
        {
            await SharedResources.FaceServiceClient.TrainPersonGroupAsync(personGroupId);
        }
    }
}
