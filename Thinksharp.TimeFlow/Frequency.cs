namespace Thinksharp.TimeFlow
{
  using System;

  public class Frequency : IComparable<Frequency>
  {
    public Frequency(Period period, int value)
    {
      if (value <= 0)
      {
        throw new ArgumentException("Frequency value must be greater than 0");
      }

      this.Value = value;
      this.Period = period;
    }

    public int Value { get; }
    public Period Period { get; }

    public static Frequency Milliseconds { get; } = new Frequency(Period.Milliseconds, 1);
    public static Frequency Seconds { get; } = new Frequency(Period.Seconds, 1);
    public static Frequency Minutes { get; } = new Frequency(Period.Minutes, 1);
    public static Frequency QuarterHours { get; } = new Frequency(Period.Minutes, 15);
    public static Frequency Hours { get; } = new Frequency(Period.Hours, 1);
    public static Frequency Days { get; } = new Frequency(Period.Days, 1);
    public static Frequency Months { get; } = new Frequency(Period.Months, 1);
    public static Frequency QuarterYears { get; } = new Frequency(Period.Months, 3);
    public static Frequency Years { get; } = new Frequency(Period.Years, 1);

    public DateTimeOffset AddFreq(DateTimeOffset dt, TimeZoneInfo timeZone = null)
      => this.Period.AddFreq(dt, this.Value, timeZone);
    
    public DateTimeOffset SubtractFreq(DateTimeOffset dt, TimeZoneInfo timeZone = null)
      => this.Period.AddFreq(dt, -this.Value, timeZone);
    
    public static DateTimeOffset operator +(DateTimeOffset dt, Frequency freq) => freq.AddFreq(dt);
    public static DateTimeOffset operator +(Frequency freq, DateTimeOffset dt) => freq.AddFreq(dt);
    public static DateTimeOffset operator -(DateTimeOffset dt, Frequency freq) => freq.SubtractFreq(dt);

    public static bool operator ==(Frequency left, Frequency right)
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
    public static bool operator !=(Frequency left, Frequency right)
    {
      return !(left == right);
    }
    public static bool operator >(Frequency left, Frequency right) => left?.CompareTo(right) > 0;
    public static bool operator <(Frequency left, Frequency right) => left?.CompareTo(right) < 0;
    public static bool operator >=(Frequency left, Frequency right) => left?.CompareTo(right) >= 0;
    public static bool operator <=(Frequency left, Frequency right) => left?.CompareTo(right) <= 0;

    public override string ToString()
    {
      return $"{this.Value} {this.Period}";
    }

    public override bool Equals(object obj)
    {
      var other = obj as Frequency;

      if (other is null ||
          this.Period != other.Period ||
          this.Value != other.Value)
      {
        return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashcode = 1430287;
        hashcode = (hashcode * 7302013) ^ this.Period.GetHashCode();
        hashcode = (hashcode * 7302013) ^ this.Value.GetHashCode();
        return hashcode;
      }
    }

    public int CompareTo(Frequency other)
    {
      if (other == null)
      {
        return 1;
      }

      var thisInterval = this.AddFreq(DateTimeOffset.MinValue);
      var otherInterval = other.AddFreq(DateTimeOffset.MinValue);

      return thisInterval.CompareTo(otherInterval);
    }
  }
}