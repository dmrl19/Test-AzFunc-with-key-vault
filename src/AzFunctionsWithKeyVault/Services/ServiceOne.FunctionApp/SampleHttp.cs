using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ServiceOne.FunctionApp;

public class SampleHttp
{
    private readonly IConfiguration _configuration;
    public SampleHttp(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [Function("SampleHttp")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var firstAppKey = _configuration["App-Key-One"];
        var secondAppKey = _configuration["App-Key-Two"];
        var thirdAppKey = _configuration["App-Key-Three"];
        var keyValueRef = _configuration["KeyVaultRef"];
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"Welcome to Azure Functions!\n");
        response.WriteString($"First value from App configuration: {firstAppKey}\n");
        response.WriteString($"Second value from App configuration: {secondAppKey}\n");
        response.WriteString($"Third value from App configuration: {thirdAppKey}\n");
        response.WriteString($"Key Vault reference from App configuration: {keyValueRef}\n");

        return response;
    }
}