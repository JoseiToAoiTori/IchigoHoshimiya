using System.Globalization;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace IchigoHoshimiya.Modules.InteractionModules;

[UsedImplicitly]
public class CalendarButtonModule(ICalendarService calendarService) 
    : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("calendar-previous")]
    [UsedImplicitly]
    public Task HandlePrevious() => HandleNavigation(-1);

    [UsedImplicitly]
    [ComponentInteraction("calendar-next")]
    public Task HandleNext() => HandleNavigation(1);

    [UsedImplicitly]
    [ComponentInteraction("calendar-today")]
    public Task HandleToday() => UpdateCalendarMessage(DateTime.UtcNow);
    
    private async Task HandleNavigation(int dayModifier)
    {
        var originalMessage = Context.Interaction.Message;
        var originalEmbed = originalMessage.Embeds.FirstOrDefault();
        
        if (originalEmbed?.Footer == null)
        {
            await RespondWithErrorAsync("Could not find the original date. Please try the `/calendar` command again.");
            return;
        }

        // Parse the date from the footer
        var footerText = originalEmbed.Footer.Text.Replace("Date: ", "");
        if (!DateTime.TryParseExact(footerText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var currentDate))
        {
            await RespondWithErrorAsync("Invalid date format in the original message. Please try the `/calendar` command again.");
            return;
        }
        
        var newDate = currentDate.AddDays(dayModifier);
        await UpdateCalendarMessage(newDate);
    }
    
    private async Task UpdateCalendarMessage(DateTime targetDate)
    {
        var newEmbed = await calendarService.GetCalendar(targetDate.DayOfWeek);
        
        newEmbed.Footer = new EmbedFooterProperties
        {
            Text = $"Date: {targetDate:yyyy-MM-dd}"
        };
        newEmbed.Timestamp = DateTimeOffset.UtcNow;

        // Recreate the buttons, otherwise they gone
        ButtonProperties previousButton = new("calendar-previous", "⬅️ Previous Day", ButtonStyle.Primary);
        ButtonProperties nextButton = new("calendar-next", "Next Day ➡️", ButtonStyle.Primary);
        ButtonProperties todayButton = new("calendar-today", "Today", ButtonStyle.Primary);
        ActionRowProperties components = new([previousButton, todayButton, nextButton]);
        
        InteractionMessageProperties updatedMessage = new()
        {
            Embeds = [newEmbed],
            Components = [components]
        };
        
        await Context.Interaction.SendFollowupMessageAsync(updatedMessage);
    }
    
    private async Task RespondWithErrorAsync(string message)
    {
        InteractionMessageProperties errorResponse = new()
        {
            Content = message,
            Flags = MessageFlags.Ephemeral
        };
        
        await Context.Interaction.SendFollowupMessageAsync(errorResponse);
    }
}