using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Thinksharp.TimeFlow
{
  /// <summary>
  /// A time frame is a collection of named time series with the same frequency that can be processed together.
  /// </summary>
  public class TimeFrame : IEnumerable<KeyValuePair<string, TimeSeries>>
  {
    private static StringComparer timeSeriesNameComparer = StringComparer.InvariantCultureIgnoreCase;

    private readonly List<NameTimeSeriesPair> timeSeries = new List<NameTimeSeriesPair>();
    private readonly Dictionary<string, TimeSeries> timeSeriesDictionary = new Dictionary<string, TimeSeries>(timeSeriesNameComparer);
    private class NameTimeSeriesPair
    {
      public NameTimeSeriesPair(string name, TimeSeries timeSeries)
      {
        Name = name;
        TimeSeries = timeSeries;
      }

      public string Name { get; }
      public TimeSeries TimeSeries { get; }
    }

    /// <summary>
    /// Creates a new instance of the time frame.
    /// </summary>
    public TimeFrame()
    {
      this.Frequency = DateHelper.EmptyTimeSeriesFrequency;
      this.TimeZone = DateHelper.EmptyTimeSeriesZoneInfo;
    }

    private TimeFrame(IEnumerable<NameTimeSeriesPair> timeSeries, Period frequency, TimeZoneInfo timeZone)
    {
      this.timeSeries = timeSeries.ToList();
      this.timeSeriesDictionary = timeSeries.ToDictionary(x => x.Name, x => x.TimeSeries);
      this.Frequency = frequency;
      this.TimeZone = timeZone;

      this.RecalculateStartEnd();
    }

    /// <summary>
    /// Gets the number of time series within the frame.
    /// </summary>
    public int Count => this.timeSeries.Count;

    /// <summary>
    /// Gets the first time point of the frame.
    /// </summary>
    public DateTimeOffset Start { get; private set; } = DateTimeOffset.MaxValue;

    /// <summary>
    /// Gets the last time point of the frame.
    /// </summary>
    public DateTimeOffset End { get; private set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Gets the frequency of all time series within the frame.
    /// </summary>
    public Period Frequency { get; private set; }

    /// <summary>
    ///   Gets the time zone object of this time frame.
    /// </summary>
    public TimeZoneInfo TimeZone { get; private set; }

    /// <summary>
    /// Enumerates all time points of the frame.
    /// </summary>
    /// <returns>
    /// An Enumeration of all time points.
    /// </returns>
    public IEnumerable<DateTimeOffset> EnumerateTimePoints()
    {
      var current = this.Start;

      while(current <= this.End)
      {
        yield return current;
        current = this.Frequency.AddPeriod(current);
      }
    }

    /// <summary>
    /// Enumerates all time points of the frame.
    /// </summary>
    /// <returns>
    /// An Enumeration of all time points.
    /// </returns>
    public IEnumerable<TimeSeries> EnumerateTimeSeries() => this.timeSeries.Select(x => x.TimeSeries);

    /// <summary>
    /// Enumerates all names of the frame.
    /// </summary>
    /// <returns>
    /// An enumeration of all names.
    /// </returns>
    public IEnumerable<string> EnumerateNames() => this.timeSeries.Select(x => x.Name);

    /// <summary>
    /// Adds a time series with the specified name to the frame.
    /// Note that the frequency of the time series must be equal to the frequency of the frame.
    /// </summary>
    /// <param name="name">
    /// The name to use for the time series.
    /// </param>
    /// <param name="timeSeries">
    /// The time series to add.
    /// </param>
    public void Add(string name, TimeSeries timeSeries)
    {
      if (this.timeSeriesDictionary.ContainsKey(name))
      {
        throw new InvalidOperationException($"Time series with name '{name}' already exists.");
      }

      if (this.timeSeries.Count == 0 || this.timeSeries.All(ts => ts.TimeSeries.IsEmpty))
      {
        this.Frequency = timeSeries.Frequency;
        this.TimeZone = timeSeries.TimeZone;
      }
      else 
      {
        if (timeSeries.Frequency != this.Frequency)
        {
          throw new InvalidOperationException($"Time series frequency ('{timeSeries.Frequency}' must be equal the frequency of the frame ('{this.Frequency}').");
        }
        if (timeSeries.TimeZone != this.TimeZone)
        {
          throw new InvalidOperationException($"Time series time zone ('{timeSeries.TimeZone}' must be equal the time zone of the frame ('{this.TimeZone}').");
        }
      }

      var pair = new NameTimeSeriesPair(name, timeSeries);
      this.timeSeries.Add(pair);
      this.timeSeriesDictionary.Add(name, timeSeries);

      this.RecalculateStartEnd();
    }

    /// <summary>
    /// Removes the time series with the specified name from the frame.
    /// </summary>
    /// <param name="name">
    /// The name of the time series to remove.
    /// </param>
    public void Remove(string name)
    {
      var existingTimeSeriesPair = this.timeSeries.FirstOrDefault(p => timeSeriesNameComparer.Equals(p.Name, name));

      if (existingTimeSeriesPair != null)
      {
        this.timeSeriesDictionary.Remove(existingTimeSeriesPair.Name);
        this.timeSeries.Remove(existingTimeSeriesPair);

        this.RecalculateStartEnd();
      }
    }

    /// <summary>
    /// Creates a copy of the TimeFrame.
    /// </summary>
    /// <returns>
    /// The copy of the time frame.
    /// </returns>
    public TimeFrame Copy() => new TimeFrame(this.timeSeries, this.Frequency, this.TimeZone);

    /// <summary>
    /// Resamples all time series within the frame to the specified period.
    /// </summary>
    /// <param name="period">
    /// The period.
    /// </param>
    /// <param name="aggregationType">
    /// The aggregation type used for all time series.
    /// </param>
    public void ReSample(Period period, AggregationType aggregationType)
    {
      if (this.Frequency == period)
      {
        return;
      }

      this.Frequency = period;
      foreach (var ts in this.timeSeries.ToList())
      {
        this[ts.Name] = ts.TimeSeries.ReSample(period, aggregationType);        
      }      
    }

    /// <summary>
    /// Resamples all time series within the frame to the specified period using the specified aggregation types.    
    /// </summary>
    /// <param name="period">
    /// The period.
    /// </param>
    /// <param name="aggregationTypes">
    /// A dictionary containing one aggregation type for each time series name.
    /// </param>
    public void ReSample(Period period, Dictionary<string, AggregationType> aggregationTypes)
    {
      if (this.Frequency == period)
      {
        return;
      }

      // transform all time series before modify time frame
      // to avoid invalid state in case of errors
      var tsResampled = new Dictionary<string, TimeSeries>();
      foreach (var ts in this.timeSeries)
      {
        if (!aggregationTypes.TryGetValue(ts.Name, out var aggregationType))
        {
          throw new ArgumentException($"Unable to re-sample time series '{ts.Name}' because the aggregation type is not specified.");
        }
        tsResampled[ts.Name] = ts.TimeSeries.ReSample(period, aggregationType);
      }

      this.Frequency = period;
      foreach (var ts in this.timeSeries.ToList())
      {
        this[ts.Name] = tsResampled[ts.Name];
      }
    }

    public TimeFrame Slice(DateTime day)
    {
      return new TimeFrame(this.Select(ts => new NameTimeSeriesPair(ts.Key, ts.Value.Slice(day))), this.Frequency, this.TimeZone);
    }
    public TimeFrame Slice(DateTimeOffset start, DateTimeOffset end)
    {
      return new TimeFrame(this.Select(ts => new NameTimeSeriesPair(ts.Key, ts.Value.Slice(start, end))), this.Frequency, this.TimeZone);
    }

    public TimeFrame Slice(DateTimeOffset start, Period period)
    {
      return new TimeFrame(this.Select(ts => new NameTimeSeriesPair(ts.Key, ts.Value.Slice(start, period))), this.Frequency, this.TimeZone);
    }

    public TimeFrame Slice(DateTime start, Period period)
    {
      return new TimeFrame(this.Select(ts => new NameTimeSeriesPair(ts.Key, ts.Value.Slice(start, period))), this.Frequency, this.TimeZone);
    }

    public TimeFrame Slice(DateTime start, DateTime end)
    {
      return Slice(new DateTimeOffset(start), new DateTimeOffset(end));
    }

    private void RecalculateStartEnd()
    {
      var nonEmtpy = this.timeSeries.Where(ts => !ts.TimeSeries.IsEmpty).ToArray();
      this.Start = nonEmtpy.Select(p => p.TimeSeries.Start).DefaultIfEmpty(DateTimeOffset.MaxValue).Min();
      this.End = nonEmtpy.Select(p => p.TimeSeries.End).DefaultIfEmpty(DateTimeOffset.MinValue).Max();

      if (nonEmtpy.Length == 0)
      {
        this.Frequency = DateHelper.EmptyTimeSeriesFrequency;
        this.TimeZone = DateHelper.EmptyTimeSeriesZoneInfo;
      }
    }

    public IEnumerator<KeyValuePair<string, TimeSeries>> GetEnumerator()
    {
      return this.timeSeries.Select(x => new KeyValuePair<string, TimeSeries>(x.Name, x.TimeSeries)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>
    /// Returns a new time frame with the specified time series.
    /// </summary>
    /// <param name="names">
    /// The names of the time series to get.
    /// </param>
    /// <returns>
    /// A new time frame with the specified time series.
    /// </returns>
    public TimeFrame this[params string[] names]
    {
      get
      {
        var filteredTimeSeries = this.timeSeries.Where(x => names.Contains(x.Name));

        return new TimeFrame(filteredTimeSeries, this.Frequency, this.TimeZone);
      }
    }

    /// <summary>
    /// Gets or sets the time series with the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the time series to get.
    /// </param>
    /// <returns>
    /// The time series with the specified name.
    /// </returns>
    public TimeSeries this[string name]
    {
      get
      {
        return this.timeSeriesDictionary.TryGetValue(name, out var ts) ? ts : null;
      }
      set
      {
        if (this.timeSeriesDictionary.ContainsKey(name))
        {
          this.Remove(name);
        }

        this.Add(name, value);
      }
    }

    /// <summary>
    /// Gets the value of the time series with the specified name for the specified time point.
    /// </summary>
    /// <param name="name">
    /// The name of the time series to get the value for.
    /// </param>
    /// <param name="timePoint">
    /// The time to get the value for.
    /// </param>
    /// <returns>
    /// The value of the time series with the specified name for the specified time point.
    /// </returns>
    public decimal? this[string name, DateTimeOffset timePoint] => this.timeSeriesDictionary.TryGetValue(name, out var ts) ? ts[timePoint]: null;
  }
}
