using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules;

[UsedImplicitly]
public class MultiThemeTextCommandModule(IAnimethemeService animethemeService)
    : CommandModule<CommandContext>
{
    [Command("themes")]
    [UsedImplicitly]
    public MessageProperties Themes([CommandParameter(Remainder = true)] string query)
    {
        EmbedProperties embed = animethemeService.GetAllAnimethemes(query);

        MessageProperties props = new MessageProperties()
           .WithEmbeds([embed]);

        return props;
    }
}