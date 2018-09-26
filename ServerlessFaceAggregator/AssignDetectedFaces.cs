using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessFaceAggregator.DTO;
using System;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class AssignDetectedFaces
    {
        [FunctionName("AssignDetectedFaces")]
        public static async Task Run([ActivityTrigger] PersonInfoWithPhoto personInfoWithPhoto)
        {
            var photoBlob =
                await SharedResources.BlobClient.GetBlobReferenceFromServerAsync(new Uri(personInfoWithPhoto.PhotoUri));
            var photoStream = await photoBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(),
                new BlobRequestOptions(), new OperationContext());

            await SharedResources.FaceServiceClient.AddPersonFaceInPersonGroupAsync(
                personInfoWithPhoto.PersonInfo.PersonGroupId, personInfoWithPhoto.PersonInfo.PersonId, photoStream);
        }
    }
}
