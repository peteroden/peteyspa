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
    public static class setPerson
    {
        [FunctionName("setPerson")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "person/{id:int}")] HttpRequest req,
            [Blob("people/{id}.json", FileAccess.Write)] out String personInfoBlob,
            int id,
            ILogger log)
        {
            String requestBody = new StreamReader(req.Body).ReadToEnd();
            PersonInfo personInfo = JsonConvert.DeserializeObject<PersonInfo>(requestBody);
            personInfoBlob = JsonConvert.SerializeObject(personInfo);
            return new OkObjectResult(personInfo);
        }
    }
}
