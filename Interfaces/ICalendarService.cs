using NetCord.Rest;

namespace IchigoHoshimiya.Interfaces;

public interface ICalendarService
{
    public Task<EmbedProperties> GetCalendar(DayOfWeek? dayOfWeek);
}