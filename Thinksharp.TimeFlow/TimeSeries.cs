namespace Thinksharp.TimeFlow
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class TimeSeries : IndexedSeries<DateTimeOffset, decimal?>
  {
    internal TimeSeries(IEnumerable<IndexedSeriesItem<DateTimeOffset, decimal?>> sortedSeries, Frequency freq, TimeZoneInfo timeZone)
      : base(sortedSeries)
    {
      this.Freq = freq;
      this.TimeZone = timeZone;
    }

    public static ITimeSeriesFactory Factory { get; } = new TimeSeriesFactory();
    public static ITimeSeriesSettings Settings { get; } = new TimeSeriesSettings();

    public TimeFrame ToFrame(string name)
    {
      var frame = new TimeFrame();
      frame.Add(name, this);
      return frame;
    }

    /// <summary>
    ///   Gets the time zone object of this time series.
    /// </summary>
    public TimeZoneInfo TimeZone { get; }

    /// <summary>
    ///   Gets the frequency of this time series..
    /// </summary>
    public Frequency Freq { get; }

    /// <summary>
    ///   Returns a new date time series where all values are mapped using the specified mapping function.
    /// </summary>
    /// <param name="map">
    ///   The mapping function to use for mapping values.
    /// </param>
    /// <returns>
    ///   Returns a new date time series where all values are mapped using the specified mapping function.
    /// </returns>
    public TimeSeries Apply(Func<decimal?, decimal?> func)
    {
      return new TimeSeries(this.sortedValues.Select(x => new IndexedSeriesItem<DateTimeOffset, decimal?>(x.Key, func(x.Value))), this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Returns a new date time series where all values are mapped using the specified mapping function whereas null values
    ///   remain as null values.
    /// </summary>
    /// <param name="map">
    ///   The mapping function to use for mapping non-nullable values.
    /// </param>
    /// <returns>
    ///   Returns a new date time series where all values are mapped using the specified mapping function whereas null values
    ///   remain as null values.
    /// </returns>
    public TimeSeries ApplyValues(Func<decimal, decimal> func)
    {
      return new TimeSeries(this.sortedValues.Select(x => new IndexedSeriesItem<DateTimeOffset, decimal?>(x.Key, x.Value.HasValue ? (decimal?)func(x.Value.Value) : null)), this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Joins the specified time series to the this time series using the specified aggregation function to combine the
    ///   values.
    /// </summary>
    /// <param name="dateTimeSeries">
    ///   The time series to join.
    /// </param>
    /// <param name="agg">
    ///   The aggregation function to combine values from 2 equal time points.
    ///   The first value of the aggregation function is from this time series, the second value from the passed one.
    /// </param>
    /// <returns>
    ///   A new time series with the same time points as this one but with values produced by the join.
    /// </returns>
    public TimeSeries JoinLeft(TimeSeries dateTimeSeries, Func<decimal?, decimal?, decimal?> agg)
    {
      if (dateTimeSeries.Freq != this.Freq)
      {
        throw new InvalidOperationException("Unable to join time series with different frequencies.");
      }

      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      foreach (var sortedValue in this.sortedValues)
      {
        var leftValue = sortedValue.Value;
        var rightValue = dateTimeSeries[sortedValue.Key];

        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(sortedValue.Key, agg(leftValue, rightValue)));
      }

      return new TimeSeries(result, this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Joins the specified time series to the this time series using the specified aggregation function to combine the
    ///   values.
    /// </summary>
    /// <param name="dateTimeSeries">
    ///   The time series to join.
    /// </param>
    /// <param name="op">
    ///   The join operation to use.
    /// </param>
    /// <returns>
    ///   A new time series with the same time points as this one but with values produced by the join.
    /// </returns>
    public TimeSeries JoinLeft(TimeSeries dateTimeSeries, JoinOperation op)
    {
      return this.JoinLeft(dateTimeSeries, op.Apply);
    }

    /// <summary>
    ///   Joins the specified time series to the this time series using the specified aggregation function to combine the
    ///   values.
    /// </summary>
    /// <param name="dateTimeSeries">
    ///   The time series to join.
    /// </param>
    /// <param name="agg">
    ///   The aggregation function to combine values from 2 equal time points.
    ///   The first value of the aggregation function is from this time series, the second value from the passed one.
    /// </param>
    /// <returns>
    ///   A new time series with the same time points as this one but with values produced by the join.
    /// </returns>
    public TimeSeries JoinFull(TimeSeries dateTimeSeries, Func<decimal?, decimal?, decimal?> agg)
    {
      if (dateTimeSeries.Freq != this.Freq)
      {
        throw new InvalidOperationException("Unable to join time series with different frequencies.");
      }

      if (this.Start <= dateTimeSeries.Start && this.End >= dateTimeSeries.End)
      {
        return this.JoinLeft(dateTimeSeries, agg);
      }

      var ts = TimeSeries.Factory.FromValue(null,
        DateHelper.Min(this.Start, dateTimeSeries.Start),
        DateHelper.Max(this.End, dateTimeSeries.End),
        this.Freq,
        this.TimeZone);

      return ts.JoinLeft(this, (l, r) => r).JoinLeft(dateTimeSeries, agg);
    }

    /// <summary>
    ///   Joins the specified time series to the this time series using the specified aggregation function to combine the
    ///   values.
    /// </summary>
    /// <param name="ts1">
    ///   The first time series to join.
    /// </param>
    /// <param name="ts2">
    ///   The second time series to join.
    /// </param>
    /// <param name="agg">
    ///   The aggregation function to combine values from 3 equal time points.
    ///   The first value of the aggregation function is from this time series, the second value from ts1 and the third from
    ///   ts2.
    /// </param>
    /// <returns>
    ///   A new time series with the same time points as this one but with values produced by the join.
    /// </returns>
    public TimeSeries JoinFull(TimeSeries ts1, TimeSeries ts2, Func<decimal?, decimal?, decimal?, decimal?> agg)
    {
      if (ts1.Freq != this.Freq || ts2.Freq != this.Freq)
      {
        throw new InvalidOperationException("Unable to join time series with different frequencies.");
      }

      var current = DateHelper.Min(this.Start, ts1.Start, ts2.Start);
      var end = DateHelper.Max(this.End, ts1.End, ts2.End);

      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      while (current <= end)
      {
        var v1 = this[current];
        var v2 = ts1[current];
        var v3 = ts2[current];

        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(current, agg(v1, v2, v3)));

        current = this.Freq.AddFreq(current, this.TimeZone);
      }

      return new TimeSeries(result, this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Joins the specified time series to the this time series using the specified aggregation function to combine the
    ///   values.
    /// </summary>
    /// <param name="ts1">
    ///   The first time series to join.
    /// </param>
    /// <param name="ts2">
    ///   The second time series to join.
    /// </param>
    /// <param name="ts3">
    ///   The third time series to join.
    /// </param>
    /// <param name="agg">
    ///   The aggregation function to combine values from 4 equal time points.
    ///   The first value of the aggregation function is from this time series, the second value from ts1 and the third from
    ///   ts2 and the fourth from ts3.
    /// </param>
    /// <returns>
    ///   A new time series with the same time points as this one but with values produced by the join.
    /// </returns>
    public TimeSeries JoinFull(TimeSeries ts1, TimeSeries ts2, TimeSeries ts3, Func<decimal?, decimal?, decimal?, decimal?, decimal?> agg)
    {
      if (ts1.Freq != this.Freq || ts2.Freq != this.Freq || ts3.Freq != this.Freq)
      {
        throw new InvalidOperationException("Unable to join time series with different frequencies.");
      }

      var current = DateHelper.Min(this.Start, ts1.Start, ts2.Start, ts3.Start);
      var end = DateHelper.Max(this.End, ts1.End, ts2.End, ts3.End);

      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      while (current <= end)
      {
        var v1 = this[current];
        var v2 = ts1[current];
        var v3 = ts2[current];
        var v4 = ts3[current];

        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(current, agg(v1, v2, v3, v4)));

        current = this.Freq.AddFreq(current, this.TimeZone);
      }

      return new TimeSeries(result, this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Joins the specified time series to the this time series using the specified aggregation function to combine the
    ///   values.
    /// </summary>
    /// <param name="dateTimeSeries">
    ///   The time series to join.
    /// </param>
    /// <param name="op">
    ///   The join operation to use.
    /// </param>
    /// <returns>
    ///   A new time series with the same time points as this one but with values produced by the join.
    /// </returns>
    public TimeSeries JoinFull(TimeSeries dateTimeSeries, JoinOperation op)
    {
      return this.JoinFull(dateTimeSeries, op.Apply);
    }

    /// <summary>
    ///   Creates a new time series with changed frequency.
    /// </summary>
    /// <param name="frequency">
    ///   The new frequency.
    /// </param>
    /// <param name="aggregationType">
    ///   The aggregation type to use for up/down sampling
    /// </param>
    /// <returns>
    ///   A new time series with the new frequency.
    /// </returns>
    public TimeSeries ReSample(Frequency frequency, AggregationType aggregationType)
    {
      var aggregator = aggregationType == AggregationType.Sum
        ? new Func<IEnumerable<decimal>, decimal>(x => x.Sum())
        : x => x.Average();

      if (frequency == this.Freq)
      {
        return this;
      }

      // down-sampling
      if (this.Freq < frequency)
      {
        if (frequency == Frequency.Hours)
        {
          return this.DownSample(x => new DateTimeOffset(x.Year, x.Month, x.Day, x.Hour, 0, 0, x.Offset), aggregator, frequency);
        }
        
        if (frequency == Frequency.Days)
        {
          return this.DownSample(x => x.Date, aggregator, frequency);
        }
        if (frequency == Frequency.Months)
        {
          return this.DownSample(x => new DateTime(x.Year, x.Month, 1), aggregator, frequency);
        }
        if (frequency == Frequency.QuarterYears)
        {
          return this.DownSample(x => x.Year * 10000 + x.GetQuarterYear(), aggregator, frequency);
        }
        if (frequency == Frequency.Years)
        {
          return this.DownSample(x => x.Year, aggregator, frequency);
        }
      }

      // up-sampling
      if (this.Freq > frequency)
      {
        return this.UpSample(aggregationType, frequency);
      }

      // not yet supported
      throw new InvalidOperationException($"Re-sample from '{this.Freq}' to '{frequency}' is not supported yet.");
    }

    private TimeSeries UpSample(AggregationType agg, Frequency frequency)
    {
      // just upsample values
      if (agg == AggregationType.Mean)
      {
        var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
        foreach (var timepoint in this.sortedValues)
        {
          var current = timepoint.Key;
          var end = this.Freq.AddFreq(current, this.TimeZone);
          while (current < end)
          {
            result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(current, timepoint.Value));
            current = frequency.AddFreq(current, this.TimeZone);
          }
        }
        return new TimeSeries(result, frequency, this.TimeZone);
      }

      if (agg == AggregationType.Sum)
      {
        var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
        foreach (var timepoint in this.sortedValues)
        {
          var current = timepoint.Key;
          var end = this.Freq.AddFreq(current, this.TimeZone);
          var timeSpanSource = end - current;
          while (current < end)
          {
            var next = frequency.AddFreq(current, this.TimeZone);
            var timeSpanTarget = next - current;
            var part = timeSpanSource.Ticks / (decimal)timeSpanTarget.Ticks;
            result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(current, timepoint.Value / part));
            current = next;
          }
        }
        return new TimeSeries(result, frequency, this.TimeZone);
      }

      throw new NotSupportedException($"AggregationType '{agg}' is not supported.");
    }

    private TimeSeries DownSample<TGRoup>(
      Func<DateTimeOffset, TGRoup> keySelector,
      Func<IEnumerable<decimal>, decimal> aggregator,
      Frequency frequency)
    {
      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      var groupedByDay = this.sortedValues.GroupBy(x => keySelector(x.Key)).OrderBy(x => x.Key);
      foreach (var grouped in groupedByDay)
      {
        var range = grouped;
        var dateOffset = range.Min(x => x.Key);
        var isEmpty = range.Select(x => x.Value).All(v => !v.HasValue);
        var aggValue = isEmpty ? null : (decimal?) aggregator(range.Select(x => x.Value ?? 0));

        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(dateOffset, aggValue));
      }

      return new TimeSeries(result, frequency, this.TimeZone);
    }

    /// <summary>
    ///   Creates a new time series that contains only time points for the specified day.
    ///   If the day is not part of this time series, an empty day will be returned.
    /// </summary>
    /// <param name="date">
    ///   The date to get the new time series for.
    /// </param>
    /// <returns>
    ///   A new time series that contains only time points for the specified day.
    /// </returns>
    public TimeSeries Slice(DateTime date)
    {
      return new TimeSeries(this.sortedValues.Where(p => p.Key.Date == date), this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Creates a new time series that contains only time points for the specified time range.
    ///   Time points within the time range that are not part of the time series are not generated.
    /// </summary>
    /// <param name="timestampFrom">
    ///   The including start time.
    /// </param>
    /// <param name="timestampTo">
    ///   The including end time.
    /// </param>
    /// <returns>
    ///   A new time series that contains only time points for the specified range.
    /// </returns>
    public TimeSeries Slice(DateTimeOffset timestampFrom, DateTimeOffset timestampTo)
    {
      return new TimeSeries(this.sortedValues.Where(p => p.Key >= timestampFrom && p.Key <= timestampTo), this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Creates a new time series that contains only time points for the specified range.
    /// </summary>
    /// <param name="startIndex">
    ///   The zero based start index.
    /// </param>
    /// <param name="count">
    ///   The number of time points to get.
    /// </param>
    /// <returns>
    ///   A new time series that contains only time points for the specified range.
    /// </returns>
    public TimeSeries Slice(int startIndex, int count)
    {
      return new TimeSeries(this.sortedValues.Skip(startIndex).Take(count), this.Freq, this.TimeZone);
    }

    /// <summary>
    ///   Creates a new time series where leading and trailing time points with specified values are dropped.
    /// </summary>
    /// <param name="valuesToTrim">
    ///   The value to trim.
    /// </param>
    /// <returns>
    ///   A new time series where leading and trailing time points with specified values are dropped.
    /// </returns>
    public TimeSeries Trim(bool dropLeading = true, bool dropTrailing = true, params decimal?[] valuesToTrim)
    {
      if (valuesToTrim == null || valuesToTrim.Length == 0)
      {
        valuesToTrim = new[] {(decimal?) null};
      }

      var startIndex = 0;

      if (dropLeading)
      {
        for (var i = 0; i < this.Count; i++)
        {
          if (!valuesToTrim.Contains(this[i].Value))
          {
            break;
          }

          startIndex++;
        }
      }

      var count = this.Count - startIndex;

      if (dropTrailing)
      {
        for (var i = this.Count - 1; i >= startIndex; i--)
        {
          if (!valuesToTrim.Contains(this[i].Value))
          {
            break;
          }

          count--;
        }
      }

      return this.Slice(startIndex, count);
    }

    public string ToTsv(IFormatProvider formatProvider = null)
    { 
      var sb = new StringBuilder();
      foreach (var keyValuePair in this.sortedValues)
      {
        if (sb.Length > 0)
        {
          sb.AppendLine();
        }
        sb.Append($"{keyValuePair.Key.ToString(formatProvider)}\t{keyValuePair.Value?.ToString(formatProvider)}");
      }

      return sb.ToString();
    }

    public override bool Equals(object obj)
    {
      var other = obj as TimeSeries;

      if (other is null ||
          this.Freq != other.Freq ||
          this.Count != other.Count ||
          this.Start != other.Start ||
          this.End != other.End)
      {
        return false;
      }

      var thisValues = this.sortedValues.Select(x => x.Value).ToArray();
      var otherValues = other.sortedValues.Select(x => x.Value).ToArray();

      for (var i = 0; i < thisValues.Length; i++)
      {
        if (thisValues[i] != otherValues[i])
        {
          return false;
        }
      }

      return true;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashcode = 1430287;
        hashcode = (hashcode * 7302013) ^ this.Count.GetHashCode();
        hashcode = (hashcode * 7302013) ^ this.Freq.Value.GetHashCode();
        hashcode = (hashcode * 7302013) ^ this.Start.GetHashCode();
        hashcode = (hashcode * 7302013) ^ this.End.GetHashCode();
        return hashcode;
      }
    }

    #region Operators

    public static TimeSeries operator +(TimeSeries ts)
    {
      return ts;
    }

    public static TimeSeries operator -(TimeSeries ts)
    {
      return ts.ApplyValues(x => x * -1);
    }
    public static TimeSeries operator +(TimeSeries left, TimeSeries right)
    {
      return left.JoinFull(right, JoinOperation.Add);
    }

    public static TimeSeries operator -(TimeSeries left, TimeSeries right)
    {
      return left.JoinFull(right, JoinOperation.Subtract);
    }

    public static TimeSeries operator *(TimeSeries left, TimeSeries right)
    {
      return left.JoinFull(right, JoinOperation.Multiply);
    }

    public static TimeSeries operator /(TimeSeries left, TimeSeries right)
    {
      return left.JoinFull(right, JoinOperation.Divide);
    }

    // DateTimeSeries / int
    public static TimeSeries operator +(TimeSeries left, int value)
    {
      return left.Apply(v => v + value);
    }

    public static TimeSeries operator -(TimeSeries left, int value)
    {
      return left.Apply(v => v - value);
    }

    public static TimeSeries operator *(TimeSeries left, int value)
    {
      return left.Apply(v => v * value);
    }

    public static TimeSeries operator /(TimeSeries left, int value)
    {
      return left.Apply(v => v / value);
    }

    // DateTimeSeries / long
    public static TimeSeries operator +(TimeSeries left, long value)
    {
      return left.Apply(v => v + value);
    }

    public static TimeSeries operator -(TimeSeries left, long value)
    {
      return left.Apply(v => v - value);
    }

    public static TimeSeries operator *(TimeSeries left, long value)
    {
      return left.Apply(v => v * value);
    }

    public static TimeSeries operator /(TimeSeries left, long value)
    {
      return left.Apply(v => v / value);
    }

    // DateTimeSeries / decimal
    public static TimeSeries operator +(TimeSeries left, decimal value)
    {
      return left.Apply(v => v + value);
    }

    public static TimeSeries operator -(TimeSeries left, decimal value)
    {
      return left.Apply(v => v - value);
    }

    public static TimeSeries operator *(TimeSeries left, decimal value)
    {
      return left.Apply(v => v * value);
    }

    public static TimeSeries operator /(TimeSeries left, decimal value)
    {
      return left.Apply(v => v / value);
    }

    // DateTimeSeries / int
    public static TimeSeries operator +(int value, TimeSeries right)
    {
      return right.Apply(v => value + v);
    }

    public static TimeSeries operator -(int value, TimeSeries right)
    {
      return right.Apply(v => value - v);
    }

    public static TimeSeries operator *(int value, TimeSeries right)
    {
      return right.Apply(v => value * v);
    }

    public static TimeSeries operator /(int value, TimeSeries right)
    {
      return right.Apply(v => value / v);
    }

    // DateTimeSeries / long
    public static TimeSeries operator +(long value, TimeSeries right)
    {
      return right.Apply(v => value + v);
      return right.Apply(v => value + v);
    }

    public static TimeSeries operator -(long value, TimeSeries right)
    {
      return right.Apply(v => value - v);
    }

    public static TimeSeries operator *(long value, TimeSeries right)
    {
      return right.Apply(v => value * v);
    }

    public static TimeSeries operator /(long value, TimeSeries right)
    {
      return right.Apply(v => value / v);
    }

    // DateTimeSeries / decimal
    public static TimeSeries operator +(decimal value, TimeSeries right)
    {
      return right.Apply(v => value + v);
    }

    public static TimeSeries operator -(decimal value, TimeSeries right)
    {
      return right.Apply(v => value - v);
    }

    public static TimeSeries operator *(decimal value, TimeSeries right)
    {
      return right.Apply(v => value * v);
    }

    public static TimeSeries operator /(decimal value, TimeSeries right)
    {
      return right.Apply(v => value / v);
    }

    public static bool operator ==(TimeSeries left, TimeSeries right)
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

    public static bool operator !=(TimeSeries left, TimeSeries right)
    {
      return !(left == right);
    }

    #endregion  
  }
}