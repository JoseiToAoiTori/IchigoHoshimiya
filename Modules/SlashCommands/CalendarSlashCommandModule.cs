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

public class CalendarSlashCommandModule(ICalendarService calendarService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("calendar", "Get the airing calendar for a specified day (defaults to today)")]
    [UsedImplicitly]
    public async Task GetCalendar(
        [SlashCommandParameter(Name = "dayofweek", Description = "Sunday Monday Chu-Chu Tuesday")]
        CalendarDay? dayOfWeek = null)
    {
        await RespondAsync(InteractionCallback.DeferredMessage());

        var today = DateTime.UtcNow.Date;
        DateTime targetDate;

        if (dayOfWeek is null)
        {
            targetDate = today;
        }
        else
        {
            if (!Enum.TryParse<DayOfWeek>(dayOfWeek.ToString(), true, out var parsedDay))
            {
                InteractionMessageProperties errorResponse = new()
                {
                    Content = "Not a valid day of the week"
                };

                await Context.Interaction.SendFollowupMessageAsync(errorResponse);

                return;
            }

            // compute upcoming target date
            var daysUntilTarget = ((int)parsedDay - (int)today.DayOfWeek + 7) % 7;
            targetDate = today.AddDays(daysUntilTarget);
        }

        var embed = await calendarService.GetCalendar(targetDate);

        // Footer keeps the resolved calendar date
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