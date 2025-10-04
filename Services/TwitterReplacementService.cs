using System.Text.RegularExpressions;
using IchigoHoshimiya.Interfaces;

namespace IchigoHoshimiya.Services;

public partial class TwitterReplacementService : ITwitterReplacementService
{
    private const string ReplacementDomain = "https://girlcockx.com";
    private static readonly string[] SDomainsToWatch = ["https://twitter.com", "https://x.com"];

    private static readonly Regex STweetLinkRegex = TweetLinkRegex();

    public Task<string?> GetReplacedContentAsync(string originalContent, string username)
    {
        var hasLink = SDomainsToWatch.Any(originalContent.Contains);
        if (!hasLink)
        {
            return Task.FromResult<string?>(null);
        }
        
        var content = STweetLinkRegex.Replace(
            originalContent,
            match =>
            {
                var replaced = match.Value
                                    .Replace("https://twitter.com", ReplacementDomain)
                                    .Replace("https://x.com", ReplacementDomain);

                return $"{replaced}/en";
            });

        return content != originalContent
            ? Task.FromResult<string?>($"Sent by {username}: {content}")
            : Task.FromResult<string?>(null);
    }
    
    [GeneratedRegex(@"https://(?:twitter\.com|x\.com)/[^/\s]+/status/[^\s?|]+", RegexOptions.Compiled)]
    private static partial Regex TweetLinkRegex();
}