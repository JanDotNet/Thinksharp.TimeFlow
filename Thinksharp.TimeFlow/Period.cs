using System;

namespace Thinksharp.TimeFlow
{ 
  public abstract class Period
  {
    private class PeriodMillisecond : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddMilliseconds(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string ToString() => "Milliseconds";
    }
    private class PeriodSecond : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddSeconds(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string ToString() => "Seconds";
    }
    private class PeriodMinute : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddMinutes(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string ToString() => "Minutes";
    }
    private class PeriodHour : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddHours(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string ToString() => "Hours";
    }
    private class PeriodDay : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddDays(value);
        var localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
        var offsetDiff = dt.Offset - localTime.Offset;
        return localTime + offsetDiff;
      }

      public override string ToString() => "Days";
    }
    private class PeriodMonth : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var date = dt.AddMonths(value);
        return new DateTimeOffset(date.DateTime, timeZone.GetUtcOffset(date.DateTime));
      }

      public override string ToString() => "Months";
    }
    private class PeriodYear : Period
    {
      public override DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null)
      {
        timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

        var utc = dt.ToUniversalTime().AddYears(value);
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZone.Id);
      }

      public override string ToString() => "Years";
    }
    public static Period Milliseconds { get; } = new PeriodMillisecond();
    public static Period Seconds { get; } = new PeriodSecond();
    public static Period Minutes { get; } = new PeriodMinute();
    public static Period Hours { get; } = new PeriodHour();
    public static Period Days { get; } = new PeriodDay();
    public static Period Months { get; } = new PeriodMonth();
    public static Period Years { get; } = new PeriodYear();
    public abstract DateTimeOffset AddFreq(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null);
    public override bool Equals(object other) => this.GetType() == other.GetType();
    public override int GetHashCode() => this.GetType().GetHashCode();
    public static bool operator ==(Period left, Period right)
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
    public static bool operator !=(Period left, Period right)
    {
      return !(left == right);
    }
  }
}