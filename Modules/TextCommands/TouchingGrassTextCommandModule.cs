using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules.TextCommands;

public class TouchingGrassTextCommandModule(ITouchGrassService touchGrassService, IConfiguration configuration)
    : CommandModule<CommandContext>
{
    [Command("touchgrass")]
    [UsedImplicitly]
    public async Task<string> AddGrassToucher([CommandParameter(Remainder = true)] string period)
    {
        var touchingGrassGuildId = configuration.GetValue<ulong>("TouchingGrassGuild");

        if (Context.Message.GuildId != touchingGrassGuildId)
        {
            return "You can't use that here";
        }

        await touchGrassService.AddGrassToucher(
            touchingGrassGuildId,
            Context.Message!.ChannelId,
            Context.User.Id,
            period);

        return $"{Context.User.Username} has chosen to touch grass!";
    }
}