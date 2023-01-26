using System.Threading.Tasks;
using Xunit;

namespace Sandbox.Generator.Tests
{
    public class MinimalGeneratorTests
    {
        [Fact]
        public async Task MinimalClass()
        {
            var code = @"
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunctionApp17
{
    public class Function1
    {
        private readonly ILogger _logger;
        private IConfiguration? _configuration;
        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        public Function1(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _configuration = configuration;
        }

        [Function(""Function1"")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, ""get"", ""post"")] HttpRequestData req)
        {
            _logger.LogInformation(""C# "" + _configuration[""A""]);
            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [Function(""Function2"")]
        public HttpResponseData Run2([HttpTrigger(AuthorizationLevel.Anonymous, ""get"", ""post"")] HttpRequestData httpReq, FunctionContext context)
        {
            var response = httpReq.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}

";

        var generatedFileName = "MyGeneratedFile.g.cs";
        var expectedOutput = @"// Auto-generated code
using System;
public class FooClass
{
}
".Replace("'", "\"");

            await TestHelpers.RunTestAsync<MinimalSourceGenerator>(
                generatedFileName,
                code,
                expectedOutput);
        }
    }
}