using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FunctionApp17
{
    public class Function12
    {
        private readonly ILogger _logger;

        public Function12(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function12>();
        }

        [Function("Function12")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content - Type", "text / plain; charset = utf - 8");
            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}