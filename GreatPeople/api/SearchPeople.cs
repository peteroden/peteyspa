namespace Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Api.Models;
    using Azure;
    using Azure.Search.Documents;
    using Azure.Search.Documents.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public static class SearchPeople
    {
        [FunctionName("SearchPeople")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "people")] HttpRequest req,
            ILogger log)
        {
            if (req == null)
            {
                return new BadRequestResult();
            }

            string query = req.Query["query"];

            if (string.IsNullOrEmpty(query))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    var data = JsonDocument.Parse(requestBody);
                    query = data.RootElement.GetProperty("query").GetString();
                }
                catch (JsonException e)
                {
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
                SearchFields = { "firstName", "lastName", "about", "skills" },
                Select = { "id", "email", "firstName", "lastName", "about", "skills", "lookingFor" },
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
