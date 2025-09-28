using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules.TextCommands;

[UsedImplicitly]
public class CalendarTextCommandModule(ICalendarService calendarService)
    : CommandModule<CommandContext>
{
    [Command("calendar")]
    [UsedImplicitly]
    public async Task GetCalendar(string? dayOfWeek = null)
    {
        var today = DateTime.UtcNow.Date;
        DateTime targetDate;

        if (string.IsNullOrWhiteSpace(dayOfWeek))
        {
            // default to today
            targetDate = today;
        }
        else if (Enum.TryParse<DayOfWeek>(dayOfWeek, true, out var parsedDay))
        {
            // compute upcoming target date
            var daysUntilTarget = ((int)parsedDay - (int)today.DayOfWeek + 7) % 7;
            targetDate = today.AddDays(daysUntilTarget);
        }
        else
        {
            MessageProperties errorResponse = new()
            {
                Content = "Not a valid day of the week"
            };

            await Context.Message.SendAsync(errorResponse);

            return;
        }

        var embed = await calendarService.GetCalendar(targetDate);

        // keep footer for navigation/consistency
        embed.Footer = new EmbedFooterProperties
        {
            Text = $"Date: {targetDate:yyyy-MM-dd}"
        };

        embed.Timestamp = DateTimeOffset.UtcNow;

        MessageProperties response = new()
        {
            Embeds = [embed]
        };

        await Context.Message.SendAsync(response);
    }
}