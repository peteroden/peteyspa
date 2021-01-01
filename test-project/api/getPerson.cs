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

namespace api
{
    public static class getPerson
    {
        [FunctionName("getPerson")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/{id:int}")] HttpRequest req,
            //[Blob("people/{id}", FileAccess.Read)] Stream myBlob,
            int id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = JsonConvert.SerializeObject(new {
                firstName = "Pete",
                lastName = "Roden"
                });
            return new OkObjectResult(responseMessage);
        }
    }
}
