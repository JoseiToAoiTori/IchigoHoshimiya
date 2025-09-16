namespace IchigoHoshimiya.Interfaces;

public interface ITouchGrassService
{
    public Task AddGrassToucher(ulong guildId, ulong channelId, ulong userId, string period);
}