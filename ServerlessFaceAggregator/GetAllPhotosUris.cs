using Microsoft.Azure.WebJobs;
using ServerlessFaceAggregator.DTO;
using System.Collections.Generic;
using System.Linq;

namespace ServerlessFaceAggregator
{
    public class GetAllPhotosUris
    {
        [FunctionName("GetAllPhotosUris")]
        public static IList<FileInfo> Run([ActivityTrigger] PathWithContainerBlobs pathWithContainerBlobs)
        {
            return pathWithContainerBlobs.Blobs.Where(e => e.RelativePath.StartsWith(pathWithContainerBlobs.Path))
                .Where(e => e.RelativePath.Contains('.')).ToList();
        }
    }
}

