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
        var targetDate = DateTime.UtcNow;

        switch (string.IsNullOrWhiteSpace(dayOfWeek))
        {
            case false when Enum.TryParse<DayOfWeek>(dayOfWeek, true, out var parsedDay):
            {
                var daysToAdd = (int)parsedDay - (int)targetDate.DayOfWeek;
                targetDate = targetDate.AddDays(daysToAdd);

                break;
            }
            case false:
                MessageProperties errorResponse = new()
                {
                    Content = "Not a valid day of the week"
                };

                await Context.Message.SendAsync(errorResponse);
                return;
        }
        
        var embed = await calendarService.GetCalendar(targetDate.DayOfWeek);
        
        // Hack to maintain state for the arrow buttons
        embed.Footer = new EmbedFooterProperties
        {
            Text = $"Date: {targetDate:yyyy-MM-dd}"
        };
        embed.Timestamp = DateTimeOffset.UtcNow;
        
        MessageProperties response = new()
        {
            Embeds = [embed],
        };

        await Context.Message.SendAsync(response);
    }
}