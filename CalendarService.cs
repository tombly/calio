namespace Calio;

public class CalendarService
{
    public static List<CalendarEventModel> GetUpcomingEvents(string icsPath)
    {
        var icsContent = File.ReadAllText(icsPath);
        var calendar = Ical.Net.Calendar.Load(icsContent);
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var sixMonthsFromNow = now.AddMonths(6);
        var events = new List<CalendarEventModel>();
        foreach (var e in calendar.Events)
        {
            // Handle recurring events
            if (e.RecurrenceRules != null && e.RecurrenceRules.Count > 0)
            {
                var occurrences = e.GetOccurrences(startOfMonth, sixMonthsFromNow);
                foreach (var occ in occurrences)
                {
                    var occStart = occ.Period.StartTime.AsSystemLocal.Date;
                    var occEnd = occ.Period.EndTime.AsSystemLocal.Date;
                    if (occStart == occEnd) // single-day event
                    {
                        events.Add(new CalendarEventModel
                        {
                            EventName = e.Summary ?? string.Empty,
                            Date = occStart,
                            AllDay = e.IsAllDay
                        });
                    }
                    else // multi-day event
                    {
                        for (var d = occStart; d < occEnd; d = d.AddDays(1))
                        {
                            events.Add(new CalendarEventModel
                            {
                                EventName = e.Summary ?? string.Empty,
                                Date = d,
                                AllDay = e.IsAllDay
                            });
                        }
                    }
                }
            }
            else
            {
                var start = e.DtStart?.AsSystemLocal.Date ?? e.Start.AsSystemLocal.Date;
                var end = e.DtEnd?.AsSystemLocal.Date ?? e.End.AsSystemLocal.Date;
                bool allDay = e.IsAllDay;
                if (start >= startOfMonth && start <= sixMonthsFromNow.Date)
                {
                    if (start == end) // single-day event
                    {
                        events.Add(new CalendarEventModel
                        {
                            EventName = e.Summary ?? string.Empty,
                            Date = start,
                            AllDay = allDay
                        });
                    }
                    else // multi-day event
                    {
                        for (var d = start; d < end; d = d.AddDays(1))
                        {
                            events.Add(new CalendarEventModel
                            {
                                EventName = e.Summary ?? string.Empty,
                                Date = d,
                                AllDay = allDay
                            });
                        }
                    }
                }
            }
        }
        return events;
    }
}