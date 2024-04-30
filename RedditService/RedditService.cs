using System.Text.Json;
using Reddit;
using Reddit.Controllers;

namespace RedditService
{
	public class RedditService
	{
		private const string _state = "MY_STATE";
		private const string _userAgent = "MockClient/0.1 by Me";

        private readonly string _clientId;
		private readonly string _redirectUrl;
		private string _authorizationCode;
		private string _accessToken;
		private string _refreshToken;

		public RedditService(string clientId, string redirectUrl)
		{
			_clientId = clientId;
			_redirectUrl = redirectUrl;
		}

		public string GetAuthorizationUrl()
		{
            return $"https://www.reddit.com/api/v1/authorize?client_id={_clientId}&response_type=code&state={_state}&redirect_uri={_redirectUrl}&duration=permanent&scope=identity edit flair history modconfig modflair modlog modposts modwiki mysubreddits privatemessages read report save submit subscribe vote wikiedit wikiread";
        }

		public async Task GetAccessTokenAndRefreshToken(string authorizationCode)
		{
			_authorizationCode = authorizationCode;

			using (HttpClient client = new HttpClient())
			{
				var parameters = new Dictionary<string, string>
				{
					{ "grant_type", "authorization_code" },
					{ "code", authorizationCode },
					{ "redirect_uri", _redirectUrl }
				};

				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authorizationCode}");
				client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
				var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://www.reddit.com/api/v1/access_token")
				{
					Content = new FormUrlEncodedContent(parameters)
				};

				var byteArray = System.Text.Encoding.ASCII.GetBytes($"{_clientId}:{string.Empty}");
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				var tokenResponse = await client.SendAsync(tokenRequest);
				var tokenResponseBody = await tokenResponse.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(tokenResponseBody);
                var rootElement = jsonDoc.RootElement;
                _accessToken = rootElement.GetProperty("access_token").GetString();
                _refreshToken = rootElement.GetProperty("refresh_token").GetString();
            }
		}

		public Tuple<string, string> GetPostWithMostUpvotes(string subreddit)
		{
			try
			{
                var redditClient = new RedditClient(_clientId, _refreshToken, "", _accessToken, _userAgent);
                var subReddits = redditClient.GetSubreddits("popular");
                var result = subReddits.Where(s => s.Name.Equals(subreddit, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (result != null)
                {
                    var topPost = result.Posts.Top.OrderByDescending(p => p.UpVotes).First();
					return Tuple.Create(topPost.Title, (topPost as LinkPost)?.URL);
                }
            }
			catch(Exception e)
			{
				Console.WriteLine("Exception is thrown when finding post with most upvotes: " + e);
			}

            return Tuple.Create($"Cannot find the post with most upvotes in subreddit {subreddit}", "");
        }

		public Tuple<string, string> GetUserWithMostPosts(string subreddit)
		{
            try
            {
                var redditClient = new RedditClient(_clientId, _refreshToken, "", _accessToken, _userAgent);
                var subReddits = redditClient.GetSubreddits("popular");
                var result = subReddits.Where(s => s.Name.Equals(subreddit, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (result != null)
                {
                    var userName = result.Posts.Top.Where(t => t.Author != "[deleted]").GroupBy(t => t.Author).OrderByDescending(p => p.Count()).FirstOrDefault().Key;
					var iconUrl = redditClient.SearchUsers(new Reddit.Inputs.Search.SearchGetSearchInput(userName)).FirstOrDefault()?.About().IconImg;
					return Tuple.Create(userName, iconUrl?.Split("?")[0]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception is thrown when finding user with most posts: " + e);
            }

            return Tuple.Create($"Cannot find the the user with most posts in subreddit {subreddit}", "");
        }
	}
}

