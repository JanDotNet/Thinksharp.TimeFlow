using System;
using System.Linq;

namespace Thinksharp.TimeFlow
{ 
  public abstract class PeriodUnit
  {
    private class PeriodUnitMillisecond : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddMilliseconds(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string Name => "ms";
    }
    private class PeriodUnitSecond : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddSeconds(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string Name => "s";
    }
    private class PeriodUnitMinute : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddMinutes(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string Name => "min";
    }
    private class PeriodUnitHour : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddHours(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string Name => "h";
    }
    private class PeriodUnitDay : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddDays(value);
        var localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
        var offsetDiff = dt.Offset - localTime.Offset;
        return localTime + offsetDiff;
      }

      public override string Name => "d";
    }
    private class PeriodUnitMonth : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var date = dt.AddMonths(value);
        return new DateTimeOffset(date.DateTime, timeZone.GetUtcOffset(date.DateTime));
      }

      public override string Name => "mth";
    }
    private class PeriodUnitYear : PeriodUnit
    {
      public override DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddYears(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string Name => "yr";
    }
    public static PeriodUnit Millisecond { get; } = new PeriodUnitMillisecond();
    public static PeriodUnit Second { get; } = new PeriodUnitSecond();
    public static PeriodUnit Minute { get; } = new PeriodUnitMinute();
    public static PeriodUnit Hour { get; } = new PeriodUnitHour();
    public static PeriodUnit Day { get; } = new PeriodUnitDay();
    public static PeriodUnit Month { get; } = new PeriodUnitMonth();
    public static PeriodUnit Year { get; } = new PeriodUnitYear();
    private static PeriodUnit[] AllUnits { get; } = new[]
    {
      Millisecond,
      Second,
      Minute,
      Hour,
      Day,
      Month,
      Year
    };
    internal static PeriodUnit Parse(string unitString)
    {
      var unit = AllUnits.FirstOrDefault(u => u.Name.Equals(unitString, StringComparison.InvariantCultureIgnoreCase));

      if (unit == null)
      {
        throw new FormatException($"'{unitString}' is not a valid period unit.");
      }

      return unit;
    }
    public abstract DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null);
    public abstract string Name { get; }
    public override bool Equals(object other) => this.GetType() == other.GetType();
    public override int GetHashCode() => this.GetType().GetHashCode();
    public static bool operator ==(PeriodUnit left, PeriodUnit right)
    {
      var isLeftNull = ReferenceEquals(left, null);
      var isRightNull = ReferenceEquals(right, null);
      if (isLeftNull && isRightNull)
      {
        return true;
      }

      if (isLeftNull || isRightNull)
      {
        return false;
      }

      return left.Equals(right);
    }
    public static bool operator !=(PeriodUnit left, PeriodUnit right)
    {
      return !(left == right);
    }
  }
}