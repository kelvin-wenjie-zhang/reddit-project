using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RedditWeb.Models;

namespace RedditWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    static int counter = 0;
    private static string _authorizationCode;
    private static RedditService.RedditService _redditService;
    private static string _subreddit;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.ShowAuthorizationInput = string.IsNullOrEmpty(_authorizationCode);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(string name)
    {
        _subreddit = name;
        var postResult = _redditService.GetPostWithMostUpvotes(_subreddit);
        var userResult = _redditService.GetUserWithMostPosts(_subreddit);

        TempData["mostUpvotePost"] = postResult.Item1;
        TempData["userWithMostPost"] = userResult.Item1;
        TempData["subreddit"] = name;
        SetContentUrl(postResult.Item2, userResult.Item2);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAuthorizationUrl(string clientId, string redirectUrl)
    {
        _redditService = new RedditService.RedditService(clientId, redirectUrl);
        TempData["authorizationLink"] = _redditService.GetAuthorizationUrl();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetUpdatedResult()
    {
        var dict = new Dictionary<string, string>();
        if (_redditService == null || string.IsNullOrEmpty(_subreddit))
        {
            dict.Add("mostUpvotePost", "");
            dict.Add("userWithMostPost", "");
        }
        else
        {
            var postResult = _redditService.GetPostWithMostUpvotes(_subreddit);
            var userResult = _redditService.GetUserWithMostPosts(_subreddit);

            dict.Add("mostUpvotePost", postResult.Item1);
            dict.Add("userWithMostPost", userResult.Item1);

            SetContentUrl(postResult.Item2, userResult.Item2);
        }
        return Json(dict);
    }

    [HttpGet("authorize_callback")]
    public async Task<IActionResult> GetAuthorizationResult(string error, string state, string code)
    {
        _authorizationCode = code;
        await _redditService.GetAccessTokenAndRefreshToken(code);
        return RedirectToAction("Index");
    }

    private void SetContentUrl(string postUrl, string iconUrl)
    {
        TempData["postUrl"] = postUrl;
        TempData["iconUrl"] = iconUrl;
    }
}

