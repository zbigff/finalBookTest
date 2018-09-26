using Microsoft.ProjectOxford.Face;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace ServerlessFaceAggregator
{
    public static class SharedResources
    {
        public static string FaceApiSubscriptionKey = Environment.GetEnvironmentVariable("FaceApiSubscriptionKey");
        public static string FaceApiUri = Environment.GetEnvironmentVariable("FaceApiUri");
        public static IFaceServiceClient FaceServiceClient = new FaceServiceClient(FaceApiSubscriptionKey, FaceApiUri);
        public static CloudBlobClient BlobClient = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnectionString"))
            .CreateCloudBlobClient();
    }
}
