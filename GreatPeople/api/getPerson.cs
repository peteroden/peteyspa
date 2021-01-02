using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using api.Models;

namespace api
{
    public static class getPerson
    {
        [FunctionName("getPerson")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/{id}")] HttpRequest req,
            [Blob("people/{id}.json", FileAccess.Read, Connection = "GreatPeopleStorage")] String personInfoBlob,
            String id,
            ILogger log)
        {
            // TODO: only throw 403 is user doesn not have users role
            log.LogInformation(personInfoBlob);
            if (personInfoBlob == null)
            {
                return new NoContentResult();
            }
            else
            {
                var returnValue = JsonConvert.DeserializeObject<PersonInfo>(personInfoBlob);
                return new OkObjectResult(returnValue);
            }
        }
    }
}