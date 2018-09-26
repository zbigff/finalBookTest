using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessFaceAggregator.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class GetAllFilesInStorage
    {
        [FunctionName("GetAllFilesInStorage")]
        public static async Task<IList<FileInfo>> Run([ActivityTrigger] string storageName)
        {
            var mainContainer = SharedResources.BlobClient.GetContainerReference(storageName);
            var listedBlobs = await mainContainer.ListBlobsSegmentedAsync(String.Empty, true, BlobListingDetails.All, Int32.MaxValue, null, new BlobRequestOptions(), new OperationContext());
            var storageNameWithSlash = "/" + storageName;
            return listedBlobs.Results.Where(x => !x.Uri.AbsoluteUri.Contains("_thumbnail.")).Select(e => new FileInfo() { Uri = e.Uri.AbsoluteUri, RelativePath = e.Uri.LocalPath.Substring(e.Uri.LocalPath.IndexOf(storageNameWithSlash)) }).ToList();
        }
    }
}
