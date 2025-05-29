using Domain.EventClient;
using Moq;
using Moq.Protected;
using System.Net;

namespace UnitTests.Clients;

[TestFixture]
public class BIEventClient_Tests
{
    public BIEventClient SUT { get; private set; }
    public Mock<HttpMessageHandler> MockHttp { get; private set; }

    [SetUp]
    public void Setup()
    {
        MockHttp = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var client = new HttpClient(MockHttp.Object);

        SUT = new BIEventClient(client);
    }

    [Test]
    public async Task It_shall_return_empty_when_response_is_null()
    {
        MockHttp
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = null });

        var result = await SUT.GetEvents();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public void It_shall_throw_exception_on_bad_request()
    {
        MockHttp
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = null });

        Assert.ThrowsAsync<HttpRequestException>(
            async () => await SUT.GetEvents()
        );
    }
}
