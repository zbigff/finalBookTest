using Microsoft.Azure.WebJobs;
using ServerlessFaceAggregator.DTO;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class MoveOrderToModelTraining
    {
        [FunctionName("MoveOrderToModelTraining")]
        public static async Task Run([ActivityTrigger] RecognitionOrder recognitionOrder, [Queue("modeltraining", Connection = "StorageConnectionString")]IAsyncCollector<RecognitionOrder> queue)
        {
            await queue.AddAsync(recognitionOrder);
            await queue.FlushAsync();
        }
    }
}
