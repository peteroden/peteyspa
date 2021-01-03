namespace Api
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Api.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Extensions.Storage;
    using Microsoft.Extensions.Logging;

    public static class GetPerson
    {
        [FunctionName("GetPerson")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/{id}")] HttpRequest req,
            [Blob("people/{id}.json", FileAccess.Read, Connection = "GreatPeopleStorage")] string personInfoBlob,
            string id,
            ILogger log)
        {
            if (req == null)
            {
                personInfoBlob = null;
                return new BadRequestResult();
            }

            // TODO: only throw 403 if user doesn't not have users role
            log.LogInformation(personInfoBlob);
            if (personInfoBlob == null)
            {
                return new NoContentResult();
            }
            else
            {
                var returnValue = JsonSerializer.Deserialize<PersonInfo>(personInfoBlob);
                return new OkObjectResult(returnValue);
            }
        }
    }
}