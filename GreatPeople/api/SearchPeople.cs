using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Newtonsoft.Json;
using api.Models;

namespace api
{
    public static class SearchPeople
    {
        [FunctionName("SearchPeople")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "people")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string query = req.Query["query"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.name;

            // Get the service endpoint and API key from the environment
            Uri endpoint = new Uri(Environment.GetEnvironmentVariable("SEARCH_ENDPOINT"));
            string key = Environment.GetEnvironmentVariable("SEARCH_API_KEY");
            string indexName = "people_index";

            // Create a client
            AzureKeyCredential credential = new AzureKeyCredential(key);
            SearchClient client = new SearchClient(endpoint, indexName, credential);

            SearchResults<PersonInfo> response = await client.SearchAsync<PersonInfo>(query);
            List<PersonInfo> peopleList = new List<PersonInfo>();
            await foreach (SearchResult<PersonInfo> result in response.GetResultsAsync())
            {
                PersonInfo doc = result.Document;
                peopleList.Add(doc);
                log.LogInformation($"{doc.Id}: {doc.Email}");
            }

            string responseMessage = JsonConvert.SerializeObject(peopleList);
            return new OkObjectResult(responseMessage);
        }
    }
}
