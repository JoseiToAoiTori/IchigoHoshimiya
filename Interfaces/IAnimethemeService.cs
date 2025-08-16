using IchigoHoshimiya.DTO;

namespace IchigoHoshimiya.Interfaces;

public interface IAnimethemeService
{
    public AnimethemeDto GetAnimetheme(string query);

    public List<AnimethemeDto>? GetAllAnimethemes(string query);
}