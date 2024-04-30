using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedditServiceTest
{
    [TestClass]
    public class RedditServiceTest
    {
        [TestMethod]
        public void TestGetAuthorizationUrl()
        {
            var service = new RedditService.RedditService("clientId", "redirectUrl");
            var authenticationUrl = service.GetAuthorizationUrl();
            var expected = $"https://www.reddit.com/api/v1/authorize?client_id=clientId&response_type=code&state=MY_STATE&redirect_uri=redirectUrl&duration=permanent&scope=identity edit flair history modconfig modflair modlog modposts modwiki mysubreddits privatemessages read report save submit subscribe vote wikiedit wikiread";

            Assert.AreEqual(expected, authenticationUrl);
        }
    }
}
