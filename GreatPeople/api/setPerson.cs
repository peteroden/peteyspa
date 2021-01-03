namespace Api
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Api.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Extensions.Storage;
    using Microsoft.Extensions.Logging;

    public static class SetPerson
    {
        [FunctionName("SetPerson")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "person/{id}")] HttpRequest req,
            [Blob("people/{id}.json", FileAccess.Write, Connection = "GreatPeopleStorage")] out string personInfoBlob,
            string id,
            ILogger log)
        {
            if (req == null)
            {
                personInfoBlob = null;
                return new BadRequestResult();
            }

            ClaimsPrincipal claimsPrincipal = StaticWebAppsAuth.Parse(req);

            using var streamReader = new StreamReader(req.Body);
            string requestBody = streamReader.ReadToEnd();

            PersonInfo personInfo = JsonSerializer.Deserialize<PersonInfo>(requestBody);

            bool isSelf = id == claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value && id == personInfo.Id;
            bool isAdmin = claimsPrincipal.IsInRole("administrator");

            if (isSelf || isAdmin)
            {
                personInfoBlob = JsonSerializer.Serialize(personInfo);
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
