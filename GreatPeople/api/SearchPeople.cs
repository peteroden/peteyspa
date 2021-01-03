using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
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

            if (string.IsNullOrEmpty(query))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                try
                {
                    var data = JsonDocument.Parse(requestBody);
                    query = data.RootElement.GetProperty("query").GetString();
                } catch (Exception e) {
                    return new BadRequestObjectResult("No valid query found");
                }
            } 

            // Get the service endpoint and API key from the environment
            Uri endpoint = new Uri(Environment.GetEnvironmentVariable("SEARCH_ENDPOINT"));
            string key = Environment.GetEnvironmentVariable("SEARCH_API_KEY");
            string indexName = "people-index";

            // Create a client
            AzureKeyCredential credential = new AzureKeyCredential(key);
            SearchClient client = new SearchClient(endpoint, indexName, credential);

            log.LogInformation("Query: " + query);

            var options = new SearchOptions()
            {
                SearchFields = { "firstName", "lastName", "about", "skills"},
                Select = { "id", "email", "firstName", "lastName", "about", "skills", "lookingFor"}
            };

            SearchResults<PersonInfo> response = await client.SearchAsync<PersonInfo>(query, options).ConfigureAwait(false);
            List<PersonInfo> peopleList = new List<PersonInfo>();
            await foreach (SearchResult<PersonInfo> result in response.GetResultsAsync().ConfigureAwait(false))
            {
                log.LogInformation(JsonSerializer.Serialize(result));
                PersonInfo doc = result.Document;
                peopleList.Add(doc);
                log.LogInformation($"{doc.Id}: {doc.Email}");
            }

            string responseMessage = JsonSerializer.Serialize(peopleList);
            return new OkObjectResult(responseMessage);
        }
    }
}
