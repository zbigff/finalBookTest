using System.Collections.Generic;

namespace ServerlessFaceAggregator.DTO
{
    public class PathWithContainerBlobs
    {
        public IList<FileInfo> Blobs { get; set; }
        public string Path { get; set; }
    }
}
