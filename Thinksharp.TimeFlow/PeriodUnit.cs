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
      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, dt.Offset);
      }

      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromMilliseconds(1);

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

      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Offset);
      }

      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromSeconds(1);

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

      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0, dt.Offset);
      }
      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromMinutes(1);

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
      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, 0, dt.Offset);
      }

      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromHours(1);

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

      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, dt.Offset);
      }
      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromDays(1);

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

      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, dt.Month, 1, 0, 0, 0, 0, dt.Offset);
      }

      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromDays(30);

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

      public override DateTimeOffset GetFirstTimePoint(DateTimeOffset dt)
      {
        return new DateTimeOffset(dt.Year, 1, 1, 0, 0, 0, 0, dt.Offset);
      }
      internal override TimeSpan PeriodUnitInterval { get; } = TimeSpan.FromDays(365);

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
    public abstract DateTimeOffset GetFirstTimePoint(DateTimeOffset dt);
    public abstract DateTimeOffset AddPeriod(DateTimeOffset dt, int value, TimeZoneInfo timeZone = null);
    internal abstract TimeSpan PeriodUnitInterval { get; }
    public abstract string Name { get; }

    public override string ToString() => Name;
    public override bool Equals(object other) => this.GetType() == other.GetType();
    public override int GetHashCode() => this.GetType().GetHashCode();
    public static bool operator ==(PeriodUnit left, PeriodUnit right)
      => Compare(left, right,
          bothNull: true,
          oneNull: false,
          noneNullFunc: (l, r) => l.Equals(r));

    public static bool operator !=(PeriodUnit left, PeriodUnit right)
      => Compare(left, right,
          bothNull: false,
          oneNull: true,
          noneNullFunc: (l, r) => !l.Equals(r));

    public static bool operator <=(PeriodUnit left, PeriodUnit right)
      => Compare(left, right,
          bothNull: true,
          oneNull: false,
          noneNullFunc: (l, r) => l.PeriodUnitInterval <= r.PeriodUnitInterval);

    public static bool operator >=(PeriodUnit left, PeriodUnit right)
      => Compare(left, right,
          bothNull: true,
          oneNull: false,
          noneNullFunc: (l, r) => l.PeriodUnitInterval >= r.PeriodUnitInterval);

    public static bool operator <(PeriodUnit left, PeriodUnit right) 
      => Compare(left, right,
        bothNull: false,
        oneNull: false,
        noneNullFunc: (l, r) => l.PeriodUnitInterval < r.PeriodUnitInterval);

    public static bool operator >(PeriodUnit left, PeriodUnit right)
      => Compare(left, right, 
        bothNull: false, 
        oneNull: false,
        noneNullFunc: (l, r) => l.PeriodUnitInterval > r.PeriodUnitInterval);

    private static bool Compare(PeriodUnit left, PeriodUnit right,
      bool bothNull,
      bool oneNull,
      Func<PeriodUnit, PeriodUnit, bool> noneNullFunc)
    {
      var isLeftNull = ReferenceEquals(left, null);
      var isRightNull = ReferenceEquals(right, null);
      if (isLeftNull && isRightNull)
      {
        return bothNull;
      }

      if (isLeftNull || isRightNull)
      {
        return oneNull;
      }

      return noneNullFunc(left, right);
    }
  }
}