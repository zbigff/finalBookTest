using Microsoft.Azure.WebJobs;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessFaceAggregator.DTO;
using System;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class ValidateNumberOfFaces
    {
        [FunctionName("ValidateNumberOfFaces")]
        public static async Task<Tuple<FileInfo, Face[]>> Run([ActivityTrigger] FileInfo fileInfo)
        {
            var photoBlob = await SharedResources.BlobClient.GetBlobReferenceFromServerAsync(new Uri(fileInfo.Uri));
            var photoStream = await photoBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(),
                new BlobRequestOptions(), new OperationContext());
            var faces = await SharedResources.FaceServiceClient.DetectAsync(photoStream);
            return new Tuple<FileInfo, Face[]>(fileInfo, faces);
        }
    }
}
