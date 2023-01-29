using System.Net;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Moq;
using ServiceOne.FunctionApp.Tests.Models;

namespace ServiceOne.FunctionApp.Tests;

public class SampleHttpTests
{
    [Fact]
    public void GivenSampleHttpRequestData_SampleHttpRun_ShouldReturnSuccessfullyResponse()
    {
        var functionContextMock = new Mock<FunctionContext>();
        var configurationMock = new Mock<IConfiguration>();
        var function = new SampleHttp(configurationMock.Object);
        var funcResponse = function.Run(new SampleHttpRequestData(functionContextMock.Object), functionContextMock.Object);

        funcResponse.Should().NotBeNull();
        funcResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        funcResponse.Headers.Should().NotBeEmpty();
        funcResponse.Headers.Should().ContainKey("Content-Type");
    }
}