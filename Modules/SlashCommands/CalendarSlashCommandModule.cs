using IchigoHoshimiya.Handlers;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

[UsedImplicitly]
public class CalendarSlashCommandModule(ICalendarService calendarService, IConfiguration configuration)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("calendar", "Get the airing calendar for a specified day (defaults to today)")]
    [UsedImplicitly]
    public async Task GetCalendar(
        [SlashCommandParameter(Name = "dayofweek", Description = "Sunday Monday Chu-Chu Tuesday")]
        string? dayOfWeek =
            null)
    {
        var command = new GenericDeferredSlashCommandHandlerForEmbed(Context, CalendarFunction);

        await command.ExecuteAsync();

        return;

        async Task<EmbedProperties> CalendarFunction()
        {
            if (string.IsNullOrWhiteSpace(dayOfWeek))
            {
                return await calendarService.GetCalendar(null);
            }

            if (Enum.TryParse<DayOfWeek>(dayOfWeek, true, out var parsedDay))
            {
                return await calendarService.GetCalendar(parsedDay);
            }

            return new EmbedProperties
            {
                Title = "Invalid Day",
                Color = new Color(
                    (byte)short.Parse(configuration["EmbedColours:Red"]!),
                    (byte)short.Parse(configuration["EmbedColours:Green"]!),
                    (byte)short.Parse(configuration["EmbedColours:Blue"]!)),
                Description = $"`{dayOfWeek}` is not a valid day of the week.\n" +
                              "Please use values like `Monday`, `Tuesday`, etc."
            };
        }
    }
}