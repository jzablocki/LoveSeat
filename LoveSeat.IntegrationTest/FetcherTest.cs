using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace LoveSeat.IntegrationTest
{
    [TestFixture]
    public class FetcherTest
    {
        [Test]
        public void ShouldFetchBodyData()
        {
            var result = Fetcher.Fetch(new Uri("http://www.msn.com"));
            Assert.IsNotEmpty(result.Response);
        }

        [Test]
        public void ShouldFetchHeaderData()
        {
            var result = Fetcher.Fetch(new Uri("http://www.msn.com"));
            Assert.IsNotEmpty(result.Headers);
        }

        [Test]
        public void ShouldFetchETag()
        {
            var result = Fetcher.Fetch(new Uri("http://motorpool.com/questions?recent"));
            Assert.IsNotEmpty(result.Etag);
        }
        [Test]
        public void ShouldFetchStatusCode()
        {
            var result = Fetcher.Fetch(new Uri("http://www.msn.com"));
            Assert.IsNotNull(result.StatusCode);
        }
    }
}
