using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MyFunctionApp
{
    public static class RetrieveTermsDataFunction
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [FunctionName("RetrieveDataFromOdataQuery")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Example For Odata Query
            string odataQuery = "Terms?$filter=StartDate le 2024-03-13Z and EndDate ge 2024-03-13Z&$select=Id,Code,Name";

            //  OData endpoint URL
            string odataEndpointUrl = "https://sisclientweb-700031.campusnexus.cloud:443/ds/odata";

            // Construct the full URL with the OData query
            string requestUrl = $"{odataEndpointUrl}/{odataQuery}";

            try
            {
                // Set basic authentication credentials
                string username = "your_username";
                string password = "your_password";
                string authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

                // Add basic authentication header to the HTTP request
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);

                // Send the HTTP request to the OData endpoint
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                // Check if the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }

                // Read and return the response content
                string responseData = await response.Content.ReadAsStringAsync();
                return new OkObjectResult(responseData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An error occurred while fetching data from the OData endpoint.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
