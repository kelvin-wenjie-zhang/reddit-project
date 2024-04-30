# reddit-project
This is a simple ASP.NET app to show post with most upvotes and user with most post for a given subreddit.

## Prerequisite
You need to have Reddit API access and create an application with "installed app" type that has no secret, and with redirect uri being "https://localhost:7251/authorize_callback". After it is done, you should have the client id and redirect uri which would be used for the input in the web app.

## Step 1:
Open the RedditProject.sln solution in Visual Studio, and then restore Nuget pacakge and build the solution.

## Step 2:
Right-click RedditWeb project and Set As Stargup Project, then run the project. A web page should be automatically pop up and direct to "https://localhost:7251"

## Step 3:
On the web page, copy the client id and redirect uri into the textbox, click "Generate Authorization Link". A new hypertext link would show up below, click it and direct you to the Reddit authorization page. Click "Allow".
![Alt text](/images/1.png "")

![Alt text](/images/2.png "")

![Alt text](/images/3.png "")

## Step 4:
The web would return back to the localhost:7251 with an input for subreddit name, type in the name such as "gaming"
![Alt text](/images/4.png "")

## Step 5:
It would show the posts with most upvotes and the user with most posts in the subreddit.
![Alt text](/images/5.png "")