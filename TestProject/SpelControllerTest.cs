using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ReversiRestApi;

namespace TestProject
{
    [TestFixture]
    class SpelControllerTest
    {
        [Test]
        public void GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler_2Resultaten_AreEqual()
        {
            var repo = new SpelRepository();
            var controller = new ReversiRestApi.Controllers.SpelController(repo);
            var result = controller.GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler().Result;
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}

