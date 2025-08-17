using NetCord.Rest;

namespace IchigoHoshimiya.Interfaces;

public interface IAnimethemeService
{
    public string GetAnimetheme(string query, string? slug);

    public EmbedProperties GetAllAnimethemes(string query, string? slug);
}