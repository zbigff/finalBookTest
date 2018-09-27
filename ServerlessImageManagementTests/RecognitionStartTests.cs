using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using Utilities;
using ServerlessImageManagement;
using Moq;
using ServerlessImageManagementTests.TestHelpers;

namespace ServerlessImageManagementTests

{
    public class RecognitionStartTests : FunctionTest
    {
        [Fact]
        public async void RecognitionStart_Returns_Bad_Request_When_Order_Not_Valid()
        {
            var order = new RecognitionOrder();
            var queueCollector = new AsyncCollector<RecognitionOrder>();
            var mockValidator = new Mock<IRecOrderValidator>();
            mockValidator.Setup(x => x.IsValid(It.IsAny<RecognitionOrder>())).Returns(false);

            var body = JsonConvert.SerializeObject(order);
            var request = HttpRequestSetup(body, HttpMethod.Post, "https://localhost");
            var result = await RecognitionStart.Run(req: request, validator: mockValidator.Object, queueWithRecOrders: queueCollector, log: log);
            var resultObject = result;
            Assert.Equal(HttpStatusCode.BadRequest, resultObject.StatusCode);
        }

        [Fact]
        public async void RecognitionStart_Adds_Message_To_Collector_When_Order_Is_Valid()
        {
            var order = new RecognitionOrder()
            {
                DestinationFolder = "testFolder",
                EmailAddress = "test@gmail.com",
                PhoneNumber = "+48123456789",
                SourcePath = "testSource",
                RecognitionName = "testName",
                PatternFaces = new string[] { }
            };
            var queueCollector = new AsyncCollector<RecognitionOrder>();
            var mockValidator = new Mock<IRecOrderValidator>();
            mockValidator.Setup(x => x.IsValid(It.IsAny<RecognitionOrder>())).Returns(true);

            var body = JsonConvert.SerializeObject(order);
            var request = HttpRequestSetup(body, HttpMethod.Post, "https://localhost");
            await RecognitionStart.Run(req: request, validator: mockValidator.Object, queueWithRecOrders: queueCollector, log: log);

            Assert.NotEmpty(queueCollector.Items);
            Assert.Equal(order.EmailAddress, queueCollector.Items[0].EmailAddress);
        }
    }
}
