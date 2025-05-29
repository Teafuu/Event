using Domain.Caches;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Caches
{
    [TestFixture]
    public class MemoryCache_Tests
    {
        public MemoryCache SUT { get; private set; }

        [SetUp]
        public void Setup()
        {
            SUT = new MemoryCache(Mock.Of<ILogger<MemoryCache>>());
        }

        [TestCase("key", 34234)]
        [TestCase("key", "cachedvalue")]
        [TestCase("key", Language.NO)]
        [TestCase("key", null)]
        public void It_should_cache_value(string key, object? value)
        {
            SUT.SetAsync(key, value);

            Assert.That(SUT.GetAsync<object>(key).Result, Is.EqualTo(value));
        }

        [Test]
        public void It_should_replace_cached_value()
        {
            SUT.SetAsync("key", "original");
            SUT.SetAsync("key", "replacement");

            Assert.That(SUT.GetAsync<string>("key").Result, Is.EqualTo("replacement"));
        }

        [Test]
        public void It_should_not_get_expired_values()
        {
            SUT.SetAsync("key", "original", TimeSpan.FromSeconds(-5));

            Assert.That(SUT.GetAsync<string>("key").Result, Is.EqualTo(null));
        }

        [Test]
        public void It_should_not_throw_exception_when_getting_wrong_type()
        {
            SUT.SetAsync("key", "original");
            Assert.ThrowsAsync<InvalidCastException>(async () => await SUT.GetAsync<int>("key"));
        }
    }
}
