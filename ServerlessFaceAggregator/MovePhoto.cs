using Microsoft.Azure.WebJobs;
using ServerlessFaceAggregator.DTO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class MovePhoto
    {
        [FunctionName("MovePhoto")]
        public static async Task Run([ActivityTrigger] PhotoDestination photoDestination)
        {
            var storageName = Utils.GetStorageName(photoDestination.DestinationPath);
            var filePath = Path.Combine(Utils.GetPathWithoutStorageNameAndLeadingSlash(photoDestination.DestinationPath),
                Path.GetFileName(photoDestination.PhotoUri));
            var storage = SharedResources.BlobClient.GetContainerReference(storageName);
            var destination = storage.GetBlobReference(filePath);
            await destination.StartCopyAsync(new Uri(photoDestination.PhotoUri));
        }
    }
}
