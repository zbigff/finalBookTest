using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using ServerlessFaceAggregator.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class ProcessPhotosSequence
    {
        [FunctionName("ProcessPhotosSequence")]
        public static async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContext context, TraceWriter log)
        {
            try
            {
                var recognitionOrderWithPersonGroup = context.GetInput<RecognitionOrderWithPersonGroup>();
                IEnumerable<FileInfo> files = await context.CallActivityAsync<IList<FileInfo>>("GetAllFilesInStorage",
                    Utils.GetStorageName(recognitionOrderWithPersonGroup.RecognitionOrder.SourcePath));
                files = files.Where(e => e.RelativePath.StartsWith(recognitionOrderWithPersonGroup.RecognitionOrder.SourcePath));
                var identificationTasks = new List<Task<FaceOnPhoto>>();
                foreach (var fileInfo in files)
                {
                    identificationTasks.Add(context.CallActivityAsync<FaceOnPhoto>("IdentifyPersonOnPhoto", new PhotoUriWithPersonGroup() { PersonGroupId = recognitionOrderWithPersonGroup.PersonGroup, PhotoUri = fileInfo.Uri }));
                }
                var identificationResults = await Task.WhenAll(identificationTasks);
                var photoMovedTasks = new List<Task>();
                var selectedPhotos = new List<string>();
                foreach (var identificationResult in identificationResults)
                {
                    if (!identificationResult.IsFaceIdentifyOnPhoto)
                        continue;
                    selectedPhotos.Add(identificationResult.PhotoUri);
                    photoMovedTasks.Add(context.CallActivityAsync("MovePhoto",
                        new PhotoDestination()
                        {
                            DestinationPath = recognitionOrderWithPersonGroup.RecognitionOrder.DestinationFolder,
                            PhotoUri = identificationResult.PhotoUri
                        }));
                }
                await Task.WhenAll(photoMovedTasks);
                await context.CallActivityAsync("InformUser",
                    new UserInfo()
                    {
                        Message = "Face recognized correctly!",
                        OperationSuccessful = true,
                        RecognitionOrder = recognitionOrderWithPersonGroup.RecognitionOrder
                    });
                await context.CallActivityAsync("SaveOrderToHistory",
                    new HistoricalOrder()
                    {
                        RecognitionOrder = recognitionOrderWithPersonGroup.RecognitionOrder,
                        RecognizedFiles = selectedPhotos
                    });
            }
            catch (Exception e)
            {
                log.Error("ProcessPhotos sequence failed.", e);
            }
        }

        [FunctionName("ProcessPhotosSequenceStart")]
        public static async Task TrainPersonSequenceStart(
    [QueueTrigger("orderProcessing", Connection = "StorageConnectionString")]RecognitionOrderWithPersonGroup recognitionOrderWithPersonGroup,
    [OrchestrationClient]DurableOrchestrationClient starter,
    TraceWriter log)
        {
            string instanceId = await starter.StartNewAsync("ProcessPhotosSequence", recognitionOrderWithPersonGroup);
            log.Info($"Started orchestration with ID = '{instanceId}'. Processing: {recognitionOrderWithPersonGroup.RecognitionOrder.RecognitionName}.");
        }

    }
}
