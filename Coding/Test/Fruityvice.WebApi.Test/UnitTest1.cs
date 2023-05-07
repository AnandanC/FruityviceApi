using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace Fruityvice.WebApi.Test
{
    [TestClass]
    public class UnitTest1
    {
        HttpClient httpClient = null;

        [TestInitialize]
        public void Setup()
        {
            var webAppFactory = new WebApplicationFactory<Fruityvice.WebApi.Program>();
            httpClient = webAppFactory.CreateDefaultClient();

        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var response = await httpClient.GetAsync("api/Fruit/:nutrition?min=0&max=1000");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}