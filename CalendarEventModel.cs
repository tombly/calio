namespace Calio;

public class CalendarEventModel
{
    public string EventName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool AllDay { get; set; }
}