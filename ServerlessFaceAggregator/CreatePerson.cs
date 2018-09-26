using Microsoft.Azure.WebJobs;
using Microsoft.ProjectOxford.Face.Contract;
using ServerlessFaceAggregator.DTO;
using System;
using System.Threading.Tasks;

namespace ServerlessFaceAggregator
{
    public static class CreatePerson
    {
        [FunctionName("CreatePerson")]
        public static async Task<PersonInfo> Run([ActivityTrigger] string recognitionName)
        {
            string personGroupId = Guid.NewGuid().ToString();

            await SharedResources.FaceServiceClient.CreatePersonGroupAsync(personGroupId, "FaceSamplesGroup");
            CreatePersonResult samplePerson = await SharedResources.FaceServiceClient.CreatePersonInPersonGroupAsync(
                personGroupId,
                recognitionName
            );

            return new PersonInfo() { PersonGroupId = personGroupId, PersonId = samplePerson.PersonId };
        }
    }
}
