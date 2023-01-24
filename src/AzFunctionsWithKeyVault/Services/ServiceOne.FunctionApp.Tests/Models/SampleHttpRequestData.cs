using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ServiceOne.FunctionApp.Tests.Models;

public class SampleHttpRequestData:HttpRequestData
{
    public SampleHttpRequestData(FunctionContext functionContext) : base(functionContext)
    {
    }

    public async Task WriteBodyAsync(string body)
    {
        await Body.WriteAsync(Encoding.UTF8.GetBytes(body));
        Body.Position = 0;
    }

    public override HttpResponseData CreateResponse()
    {
        return new SampleHttpResponseData(FunctionContext);
    }

    public override Stream Body { get; } = new MemoryStream();
    public override HttpHeadersCollection Headers { get; } = new HttpHeadersCollection();
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = default!;
    public override Uri Url { get; } = default!;
    public override IEnumerable<ClaimsIdentity> Identities { get; } = default!;
    public override string Method { get; }    = default!;
}