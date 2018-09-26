using Microsoft.Azure.WebJobs;
using ServerlessFaceAggregator.DTO;
using System;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class SaveOrderToHistory
    {
        [FunctionName("SaveOrderToHistory")]
        public static async Task Run([ActivityTrigger] HistoricalOrder historicalOrder, [CosmosDB(
            databaseName: "FaceAggregator",
            collectionName: "OrdersHistory",
            ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<dynamic> documents)
        {
            await documents.AddAsync(new { id = Guid.NewGuid(), historicalOrder.RecognitionOrder.RecognitionName, historicalOrder.RecognitionOrder.SourcePath, historicalOrder.RecognitionOrder.PatternFaces, historicalOrder.RecognitionOrder.DestinationFolder, historicalOrder.RecognitionOrder.EmailAddress, historicalOrder.RecognitionOrder.PhoneNumber, historicalOrder.RecognizedFiles });
            await documents.FlushAsync();
        }
    }
}
