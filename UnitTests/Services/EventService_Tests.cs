using Domain.Caches;
using Domain.EventClient;
using Domain.Models;
using Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace UnitTests.Services
{
    [TestFixture]
    public class EventService_Tests
    {
        public EventService SUT { get; private set; }
        public Mock<IBIEventClient> BIClient { get; private set; }

        [SetUp]
        public void Setup()
        {
            BIClient = new Mock<IBIEventClient>();

            SUT = new EventService(BIClient.Object, new MemoryCache(Mock.Of<ILogger<MemoryCache>>()));
        }

        [Test]
        public async Task It_returns_valid_events()
        {
            BIClient.Setup(c => c.GetEvents(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Language>()))
                .ReturnsAsync([new Event { Title = "Title" }, new Event { Title = "Choo" }]);

            var result = await SUT.GetEvents();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Title, Is.EqualTo("Title"));
            Assert.That(result.Last().Title, Is.EqualTo("Choo"));
        }

        [Test]
        public void It_throws_exception_when_getting_events()
        {
            BIClient.Setup(c => c.GetEvents(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Language>()))
                .Throws(new HttpRequestException());


            Assert.ThrowsAsync<HttpRequestException>(
                async () => await SUT.GetEvents()
            );
        }
    }
}
