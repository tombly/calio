using Microsoft.Extensions.Configuration;

namespace Calio;

public class Program
{
    public static void Main(string[] args)
    {
        var workingFolder = ReadConfig("config.json").GetValue<string>("WorkingFolder")
                                ?? throw new InvalidOperationException("WorkingFolder not found in config file.");

        var calendarFilePaths = Directory.GetFiles(workingFolder, "*.ics").ToList();

        var eventLists = new List<CalendarEventModel>[calendarFilePaths.Count];
        for (int i = 0; i < calendarFilePaths.Count; i++)
        {
            eventLists[i] = CalendarService.GetUpcomingEvents(calendarFilePaths[i]);
        }

        CreateAllMonthlyCalendars(eventLists, workingFolder);
    }

    static IConfiguration ReadConfig(string configFileName)
    {
        var configBuilder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile(configFileName, optional: false);
        return configBuilder.Build();
    }

    static void CreateAllMonthlyCalendars(List<CalendarEventModel>[] eventLists, string workingFolder)
    {
        int year = DateTime.Now.Year;
        int startMonth = DateTime.Now.Month;
        for (int m = 0; m < 6; m++)
        {
            int month = ((startMonth - 1 + m) % 12) + 1;
            int thisYear = year + ((startMonth - 1 + m) / 12);
            string fileName = Path.Combine(workingFolder, $"Calendar_{thisYear}_{month:D2}.html");
            CalendarHtmlGenerator.CreateMonthlyCalendarHtml(fileName, eventLists, thisYear, month);
        }
    }
}
