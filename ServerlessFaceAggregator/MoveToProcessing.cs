using Microsoft.Azure.WebJobs;
using ServerlessFaceAggregator.DTO;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class MoveToProcessing
    {
        [FunctionName("MoveToProcessing")]
        public static async Task Run([ActivityTrigger] RecognitionOrderWithPersonGroup recognitionOrder, [Queue("orderprocessing", Connection = "StorageConnectionString")]IAsyncCollector<RecognitionOrderWithPersonGroup> queue)
        {
            await queue.AddAsync(recognitionOrder);
            await queue.FlushAsync();
        }
    }
}
