using NetCord.Rest;

namespace IchigoHoshimiya.Interfaces;

public interface ICalendarService
{
    public Task<EmbedProperties> GetCalendar(DateTime targetDate);
}