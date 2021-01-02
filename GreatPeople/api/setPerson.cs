namespace api
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Extensions.Storage;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using api.Models;

    public static class setPerson
    {
        [FunctionName("setPerson")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "person/{id}")] HttpRequest req,
            [Blob("people/{id}.json", FileAccess.Write, Connection = "GreatPeopleStorage")] out String personInfoBlob,
            String id,
            ILogger log)
        {
            ClaimsPrincipal claimsPrincipal = StaticWebAppsAuth.Parse(req);
            String requestBody = new StreamReader(req.Body).ReadToEnd();
            PersonInfo personInfo = JsonConvert.DeserializeObject<PersonInfo>(requestBody);

            bool isSelf = (id == claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value && id == personInfo.Id);
            bool isAdmin = (claimsPrincipal.IsInRole("administrator"));

            if (isSelf || isAdmin) {
                personInfoBlob = JsonConvert.SerializeObject(personInfo);
                return new OkObjectResult(personInfo);
            }
            else
            {
                personInfoBlob = null;
                return new UnauthorizedResult();
            }
        }
    }
}
