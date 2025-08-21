namespace IchigoHoshimiya.Interfaces;

public interface ITwitterReplacementService
{
    public Task<string?> GetReplacedContentAsync(string originalContent, string username);
}