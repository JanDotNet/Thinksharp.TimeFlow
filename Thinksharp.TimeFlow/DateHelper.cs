namespace Thinksharp.TimeFlow
{
  using System;
  using System.Collections.Concurrent;
  using System.Linq;

  internal static class DateHelper
  {
    private static readonly ConcurrentDictionary<int, Tuple<DateTime, DateTime>> cache = new ConcurrentDictionary<int, Tuple<DateTime, DateTime>>();
    private static TimeZoneInfo defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
    public static TimeZoneInfo EmptyTimeSeriesZoneInfo { get; } = TimeZoneInfo.Utc;
    public static Period EmptyTimeSeriesFrequency { get; } = Period.Milliseconds;

    public static int GetHoursOfDay(this DateTime date)
    {
      if (!cache.TryGetValue(date.Year, out var specialDays))
      {
        var dayWith23Hours = GetLastDayInMonth(date.Year, 3, DayOfWeek.Sunday);
        var dayWith25Hours = GetLastDayInMonth(date.Year, 10, DayOfWeek.Sunday);

        specialDays = new Tuple<DateTime, DateTime>(dayWith23Hours, dayWith25Hours);

        cache.TryAdd(date.Year, specialDays);
      }

      if (specialDays.Item1 == date.Date)
      {
        return 23;
      }

      if (specialDays.Item2 == date.Date)
      {
        return 25;
      }

      return 24;
    }
    public static int GetQuarterYear(this DateTime date) => (date.Month - 1) / 3 + 1;
    public static int GetQuarterYear(this DateTimeOffset date) => date.DateTime.GetQuarterYear();

    public static TimeZoneInfo GetDefaultTimeZone() => defaultTimeZone;
    public static TimeZoneInfo SetDefaultTimeZone(TimeZoneInfo timeZone) => defaultTimeZone = timeZone;

    private static DateTime GetLastDayInMonth(int year, int month, DayOfWeek dayOfWeek)
    {
      var date = new DateTime(year, month, 01);
      date = date.AddMonths(1).AddDays(-1);
      while (date.DayOfWeek != dayOfWeek)
      {
        date = date.AddDays(-1);
      }

      return date;
    }

    public static DateTimeOffset Min(params DateTimeOffset[] values)
    {
      return values.Where(v => v != default(DateTimeOffset)).Min();
    }

    public static DateTimeOffset Max(params DateTimeOffset[] values)
    {
      return values.Where(v => v != default(DateTimeOffset)).Max();
    }
  }
}