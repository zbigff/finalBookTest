using Microsoft.Azure.WebJobs;
using ServerlessFaceAggregator.DTO;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class InformUser
    {
        [FunctionName("InformUser")]
        public static async Task SayHello([ActivityTrigger] UserInfo userInfo, [Queue("informuserqueue", Connection = "StorageConnectionString")]IAsyncCollector<UserInfo> userInfos)
        {
            await userInfos.AddAsync(userInfo);
            await userInfos.FlushAsync();
        }
    }
}
