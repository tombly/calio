namespace Calio;

public class CalendarHtmlGenerator
{
    public static void CreateMonthlyCalendarHtml(string filePath, List<CalendarEventModel>[] eventLists, int year, int month)
    {
        string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        int daysInMonth = DateTime.DaysInMonth(year, month);
        // Build a lookup for events by day and by list index
        var monthEvents = new Dictionary<int, List<(CalendarEventModel ev, int listIdx)>>();
        for (int listIdx = 0; listIdx < eventLists.Length; listIdx++)
        {
            foreach (var ev in eventLists[listIdx])
            {
                if (ev.Date.Month == month && ev.Date.Year == year)
                {
                    int day = ev.Date.Day;
                    if (!monthEvents.ContainsKey(day))
                        monthEvents[day] = new List<(CalendarEventModel, int)>();
                    monthEvents[day].Add((ev, listIdx));
                }
            }
        }
        var html = "<!DOCTYPE html>\n" +
                  "<html>\n" +
                  "<head>\n" +
                  "    <meta charset='UTF-8'>\n" +
                  "    <title>" + new DateTime(year, month, 1).ToString("MMMM yyyy") + "</title>\n" +
                  "    <link href='https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600&display=swap' rel='stylesheet'>\n" +
                  "    <style>\n" +
                  "        body { font-family: 'Montserrat', 'Segoe UI', 'Arial', 'Helvetica Neue', Helvetica, Arial, sans-serif; font-weight: 500; }\n" +
                  "        table { border-collapse: collapse; width: 100%; max-width: 1400px; margin: 0 auto 40px auto; }\n" +
                  "        th, td { border: 1px solid #999; width: 14.28%; height: 120px; vertical-align: top; padding: 4px; box-sizing: border-box; }\n" +
                  "        th { background: #fffcf8; height: 30px; }\n" +
                  "        .day { font-weight: bold; }\n" +
                  "        .weekend { background: #fffcf8; }\n" +
                  "        .event0 { background: #d2d9f1; }\n" +
                  "        .event1 { background: #d8e0f4; }\n" +
                  "        .event2 { background: #ebeff4; }\n" +
                  "        .event3 { background: #e4e3e3; }\n" +
                  "        .event { margin-top: 4px; font-size: 0.85em; border-radius: 4px; padding: 2px 4px; display: block; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }\n" +
                  "        h2 { text-align: center; margin-top: 40px; }\n" +
                  "    </style>\n" +
                  "</head>\n" +
                  "<body>\n";
        html += $"<h2>{new DateTime(year, month, 1):MMMM yyyy}</h2>\n<table>\n<tr>";
        foreach (var d in days)
            html += "<th>" + d + "</th>";
        html += "</tr>\n<tr>";

        var eventColorCount = 4;

        // Calculate the starting column (Monday = 0)
        var firstOfMonth = new DateTime(year, month, 1);
        int startDayOfWeek = ((int)firstOfMonth.DayOfWeek + 6) % 7;
        int col = 0;
        for (int i = 0; i < startDayOfWeek; i++)
        {
            // Apply weekend background to Saturday (col 5) and Sunday (col 6)
            html += (col == 5 || col == 6) ? "<td class='weekend'></td>" : "<td></td>";
            col++;
        }
        for (int day = 1; day <= daysInMonth; day++)
        {
            string weekendClass = (col == 5 || col == 6) ? " class='weekend'" : "";
            html += "<td" + weekendClass + "><span class='day'>" + day + "</span>";
            if (monthEvents.ContainsKey(day))
            {
                foreach (var (ev, listIdx) in monthEvents[day])
                {
                    string colorClass = $"event event{listIdx % eventColorCount}";
                    var displayName = System.Net.WebUtility.HtmlEncode(ev.EventName.Length > 16 ? ev.EventName.Substring(0, 16) : ev.EventName);
                    var titleAttr = ev.EventName.Length > 16 ? $" title=\"{System.Net.WebUtility.HtmlEncode(ev.EventName)}\"" : "";
                    html += $"<span class='{colorClass}'{titleAttr}>{displayName}</span>";
                }
            }
            html += "</td>";
            col++;
            if (col > 6 && day != daysInMonth)
            {
                html += "</tr>\n<tr>";
                col = 0;
            }
        }
        while (col > 0 && col < 7)
        {
            html += (col == 5 || col == 6) ? "<td class='weekend'></td>" : "<td></td>";
            col++;
        }
        html += "</tr>\n</table>\n";
        html += "</body>\n</html>";
        File.WriteAllText(filePath, html);
    }
}