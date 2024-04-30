using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditWeb.Controllers;

namespace RedditServiceTest
{
	[TestClass]
	public class HomeControllerTest
	{
		[TestMethod]
		public void TestIndex()
		{
			var controller = new HomeController(null);
			var result = controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsTrue(controller.ViewBag.ShowAuthorizationInput);
		}

		[TestMethod]
		public async void TestGenerateAuthorizationUrl()
		{
            var controller = new HomeController(null);
			var result = await controller.GenerateAuthorizationUrl("clientId", "redirectUrl");
			Assert.IsNotNull(result);
			Assert.IsNotNull(controller.TempData["authorizationLink"]);
		}
    }
}

