using NetCord.Rest;

namespace IchigoHoshimiya.Interfaces;

public interface IAnimethemeService
{
    public string GetAnimetheme(string query);

    public EmbedProperties GetAllAnimethemes(string query);
}