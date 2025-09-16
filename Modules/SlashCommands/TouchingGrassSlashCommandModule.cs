using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

public class TouchingGrassSlashCommandModule(ITouchGrassService touchGrassService, IConfiguration configuration)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("touchgrass", "When you need to touch grass...")]
    [UsedImplicitly]
    public string AddGrassToucher(
        [SlashCommandParameter(Name = "hours", Description = "Hours to touch grass for.")]
        string hours)
    {
        var touchingGrassGuildId = configuration.GetValue<ulong>("TouchingGrassGuild");

        if (Context.Interaction.GuildId != touchingGrassGuildId)
        {
            return "You can't use that here";
        }

        // Fire and forget
        touchGrassService.AddGrassToucher(
            touchingGrassGuildId,
            Context.Channel.Id,
            Context.User.Id,
            hours);

        return $"{Context.User.Username} has chosen to touch grass!";
    }
}