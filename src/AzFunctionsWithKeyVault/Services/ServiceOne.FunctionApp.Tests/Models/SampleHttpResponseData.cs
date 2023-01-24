using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ServiceOne.FunctionApp.Tests.Models;

public class SampleHttpResponseData : HttpResponseData
{
    public SampleHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; } = new();
    public override Stream Body { get; set; } = new MemoryStream();
    public override HttpCookies Cookies { get; } = default!;
}