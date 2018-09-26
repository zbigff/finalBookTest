using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face.Contract;
using ServerlessFaceAggregator.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class ValidationSequence
    {
        [FunctionName("ValidationSequence")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, TraceWriter log)
        {
            try
            {
                var recognitionOrder = context.GetInput<RecognitionOrder>();
                var storageName = Utils.GetStorageName(recognitionOrder.PatternFaces.First());
                var blobsUris = await context.CallActivityAsync<IList<DTO.FileInfo>>("GetAllFilesInStorage", storageName);
                var patternItemsTasks = new List<Task<IList<DTO.FileInfo>>>();

                foreach (var directoryToPatternFace in recognitionOrder.PatternFaces)
                {
                    patternItemsTasks.Add(context.CallActivityAsync<IList<DTO.FileInfo>>("GetAllPhotosUris", new PathWithContainerBlobs() { Blobs = blobsUris, Path = directoryToPatternFace }));
                }
                var patternItems = (await Task.WhenAll(patternItemsTasks.ToArray())).SelectMany(e => e);
                if (!patternItems.Any())
                {
                    await context.CallActivityAsync("InformUser", new UserInfo() { Message = "There are no pattern images within selected directories.", RecognitionOrder = recognitionOrder, OperationSuccessful = false });
                    return;
                }

                var faceDetectionTasks = new List<Task<Tuple<DTO.FileInfo, Face[]>>>();
                foreach (var patternItem in patternItems)
                {
                    faceDetectionTasks.Add(context.CallActivityAsync<Tuple<DTO.FileInfo, Face[]>>("ValidateNumberOfFaces", patternItem));
                }

                var faceResults = await Task.WhenAll(faceDetectionTasks);
                if (faceResults.All(e => e.Item2.Length != 1))
                {
                    await context.CallActivityAsync("InformUser", new UserInfo() { Message = "Selected pattern images don't comply with a rule: one face on photo.", RecognitionOrder = recognitionOrder, OperationSuccessful = false });
                    return;
                }
                recognitionOrder.PatternFaces = faceResults.Where(e => e.Item2.Length == 1).Select(e => e.Item1.Uri);
                await context.CallActivityAsync("MoveOrderToModelTraining", recognitionOrder);
            }
            catch (Exception e)
            {
                log.Error("Validation sequence failed.", e);
            }
        }
        [FunctionName("ValidationSequenceStart")]
        public static async Task ValidationSequenceStart(
            [QueueTrigger("recognitionqueue", Connection = "StorageConnectionString")]RecognitionOrder order,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            string instanceId = await starter.StartNewAsync("ValidationSequence", order);
            log.Info($"Started orchestration with ID = '{instanceId}'. For recognition: {order.RecognitionName}.");
        }

    }
}
