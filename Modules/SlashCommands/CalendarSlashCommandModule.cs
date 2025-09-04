using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

public enum CalendarDay
{
    [SlashCommandChoice(Name = "Sunday")] Sunday,
    [SlashCommandChoice(Name = "Monday")] Monday,
    [SlashCommandChoice(Name = "Tuesday")] Tuesday,

    [SlashCommandChoice(Name = "Wednesday")]
    Wednesday,

    [SlashCommandChoice(Name = "Thursday")]
    Thursday,
    [SlashCommandChoice(Name = "Friday")] Friday,

    [SlashCommandChoice(Name = "Saturday")]
    Saturday
}

[UsedImplicitly]
public class CalendarSlashCommandModule(ICalendarService calendarService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("calendar", "Get the airing calendar for a specified day (defaults to today)")]
    [UsedImplicitly]
    public async Task GetCalendar(
        [SlashCommandParameter(Name = "dayofweek", Description = "Sunday Monday Chu-Chu Tuesday")]
        CalendarDay? dayOfWeek =
            null)
    {
        await RespondAsync(InteractionCallback.DeferredMessage());
        var targetDate = DateTime.UtcNow;

        switch (string.IsNullOrWhiteSpace(dayOfWeek.ToString()))
        {
            case false when Enum.TryParse<DayOfWeek>(dayOfWeek.ToString(), true, out var parsedDay):
            {
                var daysToAdd = (int)parsedDay - (int)targetDate.DayOfWeek;
                targetDate = targetDate.AddDays(daysToAdd);

                break;
            }
            case false:
                InteractionMessageProperties errorResponse = new()
                {
                    Content = "Not a valid day of the week"
                };

                await Context.Interaction.SendFollowupMessageAsync(errorResponse);

                return;
        }

        var embed = await calendarService.GetCalendar(targetDate.DayOfWeek);

        // Hack to maintain state for the arrow buttons
        embed.Footer = new EmbedFooterProperties
        {
            Text = $"Date: {targetDate:yyyy-MM-dd}"
        };

        embed.Timestamp = DateTimeOffset.UtcNow;

        ButtonProperties previousButton = new("calendar-previous", "⬅️ Previous Day", ButtonStyle.Primary);
        ButtonProperties nextButton = new("calendar-next", "Next Day ➡️", ButtonStyle.Primary);
        ButtonProperties todayButton = new("calendar-today", "Today", ButtonStyle.Primary);

        ActionRowProperties components = new([previousButton, todayButton, nextButton]);

        InteractionMessageProperties response = new()
        {
            Embeds = [embed],
            Components = [components]
        };

        await Context.Interaction.SendFollowupMessageAsync(response);
    }
}