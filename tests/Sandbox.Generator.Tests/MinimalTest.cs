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

namespace MyCompany
{
namespace MyProduct
{
    public class MyPClass
{
    public class MyHttpTriggers
    {
        private readonly ILogger _logger;
        private IConfiguration? _configuration;
        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function(""FunctionA"")]
        public HttpResponseData Run1([HttpTrigger(AuthorizationLevel.Anonymous, ""get"", ""post"")] HttpRequestData req)
        {
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
}
}
";

        var generatedFileName = "MyGeneratedFile.g.cs";
        var expectedOutput = @"
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Azure.Functions.Worker.Core.FunctionMetadata;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Invocation;
namespace FunctionApp26
{
    internal class DirectFunctionExecutor : IFunctionExecutor
    {
        public async Task ExecuteAsync(FunctionContext context)
        {
            if (string.Equals(context.FunctionDefinition.Name, ""MyCompany.MyProduct.MyPClass.MyHttpTriggers.Run1"",StringComparison.OrdinalIgnoreCase))
            {
                var p1 = context.InstanceServices.GetService<ILoggerFactory>();
                var modelBindingFeature = context.Features.Get<IModelBindingFeature>()!;
                var inputArguments = await modelBindingFeature.BindFunctionInputAsync(context);
                var t = new MyCompany.MyProduct.MyPClass.MyHttpTriggers(p1);
                context.GetInvocationResult().Value = t.Run1((HttpRequestData)inputArguments[1]);
            }
        }
    }
}
".Replace("'", "\"");

            await TestHelpers.RunTestAsync<MinimalSourceGenerator>(
                generatedFileName,
                code,
                expectedOutput);
        }
    }
}