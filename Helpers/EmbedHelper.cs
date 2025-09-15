using Microsoft.Extensions.Configuration;
using NetCord;
using NetCord.Rest;

namespace IchigoHoshimiya.Helpers;

public static class EmbedHelper
{
    private static Color _defaultColor;
    
    public static void Initialize(EmbedColours colours)
    {
        _defaultColor = new Color(
            (byte)colours.Red, 
            (byte)colours.Green, 
            (byte)colours.Blue);
    }

    public static EmbedProperties Build(
        string? title = null,
        string? description = null,
        Color? overrideColor = null)
    {
        var embed = new EmbedProperties();

        if (!string.IsNullOrWhiteSpace(title))
        {
            embed = embed.WithTitle(title);
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            embed = embed.WithDescription(description);
        }

        embed = embed.WithColor(overrideColor ?? _defaultColor);

        return embed;
    }
}