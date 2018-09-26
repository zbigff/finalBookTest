using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using ServerlessFaceAggregator.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class TrainPersonSequence
    {
        [FunctionName("TrainPersonSequence")]
        public static async Task Run(
               [OrchestrationTrigger] DurableOrchestrationContext context, TraceWriter log)
        {
            try
            {
                var recognitionOrder = context.GetInput<RecognitionOrder>();
                PersonInfo personInfo = await context.CallActivityAsync<PersonInfo>("CreatePerson", recognitionOrder.RecognitionName);
                var assigningFacesTasks = new List<Task>();
                foreach (var personPhotoUri in recognitionOrder.PatternFaces)
                {
                    assigningFacesTasks.Add(context.CallActivityAsync("AssignDetectedFaces", new PersonInfoWithPhoto() { PersonInfo = personInfo, PhotoUri = personPhotoUri }));
                }
                await Task.WhenAll(assigningFacesTasks);
                await context.CallActivityAsync<string>("TrainPersonGroup", personInfo.PersonGroupId);
                await context.CallActivityAsync("MoveToProcessing",
                    new RecognitionOrderWithPersonGroup() { PersonGroup = personInfo.PersonGroupId, RecognitionOrder = recognitionOrder });
            }
            catch (Exception e)
            {
                log.Error("TrainPerson sequence failed.", e);
            }
        }
        [FunctionName("TrainPersonSequenceStart")]
        public static async Task TrainPersonSequenceStart(
            [QueueTrigger("modeltraining", Connection = "StorageConnectionString")]RecognitionOrder order,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            string instanceId = await starter.StartNewAsync("TrainPersonSequence", order);
            log.Info($"Started orchestration with ID = '{instanceId}'. Processing: {order.RecognitionName}.");
        }


    }
}
