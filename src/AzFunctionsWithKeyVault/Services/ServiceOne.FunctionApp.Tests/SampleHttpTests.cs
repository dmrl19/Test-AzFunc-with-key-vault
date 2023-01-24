using System.Net;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Moq;
using ServiceOne.FunctionApp.Tests.Models;

namespace ServiceOne.FunctionApp.Tests;

public class SampleHttpTests
{
    [Fact]
    public void GivenSampleHttpRequestData_SampleHttpRun_ShouldReturnSuccessfullyResponse()
    {
        var functionContextMock = new Mock<FunctionContext>();

        var funcResponse = SampleHttp.Run(new SampleHttpRequestData(functionContextMock.Object), functionContextMock.Object);

        funcResponse.Should().NotBeNull();
        funcResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        funcResponse.Headers.Should().NotBeEmpty();
        funcResponse.Headers.Should().ContainKey("Content-Type");
    }
}