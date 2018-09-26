using Microsoft.Azure.WebJobs;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessFaceAggregator.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class IdentifyPersonOnPhoto
    {
        [FunctionName("IdentifyPersonOnPhoto")]
        public static async Task<FaceOnPhoto> Run([ActivityTrigger] PhotoUriWithPersonGroup photoUriWithPersonGroup)
        {
            var photoBlob =
                await SharedResources.BlobClient.GetBlobReferenceFromServerAsync(new Uri(photoUriWithPersonGroup.PhotoUri));
            var photoStream = await photoBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(),
                new BlobRequestOptions(), new OperationContext());
            var faces = await SharedResources.FaceServiceClient.DetectAsync(photoStream);
            if (!faces.Any())
                return new FaceOnPhoto() { IsFaceIdentifyOnPhoto = false, PhotoUri = photoUriWithPersonGroup.PhotoUri };
            var facesIds = faces.Select(e => e.FaceId);
            IdentifyResult[] identifyResults = await SharedResources.FaceServiceClient.IdentifyAsync(facesIds.ToArray(),
                photoUriWithPersonGroup.PersonGroupId);
            FaceOnPhoto result = new FaceOnPhoto()
            {
                IsFaceIdentifyOnPhoto = identifyResults.Any(e => e.Candidates.Any()),
                PhotoUri = photoUriWithPersonGroup.PhotoUri
            };
            return result;
        }
    }
}
