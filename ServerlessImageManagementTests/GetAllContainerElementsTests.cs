using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using Moq;
using Utilities;
using ServerlessImageManagementTests.TestHelpers;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using System.Reflection;
using System.Linq;
using ServerlessImageManagement;
using ServerlessImageManagement.DTO;
using System.Security.Principal;
using Utilities.IdentityProvider;

namespace ServerlessImageManagementTests
{
    public class GetAllContainerElementsTests : FunctionTest
    {
        [Fact]
        public async void GetAllContainerElements_Returns_Elements_Hierarchy()
        {
            CloudBlockBlob[] blobs = new CloudBlockBlob[]
            {
                new CloudBlockBlob(new Uri("http://mockContainer/mockUser/image1.jpg")),
                new CloudBlockBlob(new Uri("http://mockContainer/mockUser/vacation/image2.jpg"))
            };
            var containerMock = new Mock<CloudBlobContainer>(new Uri("http://mockContainer"));
            containerMock.Setup(x => x.CreateIfNotExistsAsync()).ReturnsAsync(false);
            containerMock.Setup(x => x.ListBlobsSegmentedAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<BlobListingDetails>(),
                It.IsAny<int>(), It.IsAny<BlobContinuationToken>(), It.IsAny<BlobRequestOptions>(), It.IsAny<OperationContext>()))
                 .ReturnsAsync(CreateTestBlobResultSegment(false, blobs));

            var binderMock = new Mock<IBinder>();
            binderMock.Setup(x => x.BindAsync<CloudBlobContainer>(It.IsAny<Attribute>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(containerMock.Object);

            var mockProvider = new Mock<IIdentityProvider>();
            mockProvider.Setup(x => x.WhoAmI(It.IsAny<IIdentity>())).Returns("mockUser");

            var request = HttpRequestSetup("", HttpMethod.Get, "https://localhost");
            var result = await GetAllContainerElements.Run(req: request, binder: binderMock.Object, identityProvider: mockProvider.Object, log: log);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var resultHierarchyJson = await result.Content.ReadAsStringAsync();
            var resultHierarchy = JsonConvert.DeserializeObject<TreeElement>(resultHierarchyJson);
            Assert.NotNull(resultHierarchy);
            Assert.NotNull(resultHierarchy.Children);
            Assert.Equal(2, resultHierarchy.Children.Count);
            Assert.Equal("mockUser", resultHierarchy.Name);
        }

        private BlobResultSegment CreateTestBlobResultSegment(bool continuationToken, params CloudBlockBlob[] blobs)
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            BlobContinuationToken token = continuationToken ? new BlobContinuationToken() : null;
            object[] internalCtorArgs = { blobs, token };
            object obj = Activator.CreateInstance(typeof(BlobResultSegment), bindingFlags, null, internalCtorArgs, null);
            return obj as BlobResultSegment;
        }

    }
}

