using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Thinksharp.TimeFlow
{
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

    public TimeFrame()
    { }

    private TimeFrame(IEnumerable<NameTimeSeriesPair> timeSeries, Period frequency)
    {
      this.timeSeries = timeSeries.ToList();
      this.timeSeriesDictionary = timeSeries.ToDictionary(x => x.Name, x => x.TimeSeries);
      this.Frequency = frequency;

      this.RecalculateStartEnd();
    }

    public string Name { get; set; }
    public int Count => this.timeSeries.Count;
    public DateTimeOffset Start { get; private set; } = DateTimeOffset.MaxValue;
    public DateTimeOffset End { get; private set; } = DateTimeOffset.MinValue;
    public Period Frequency { get; private set; }

    public IEnumerable<DateTimeOffset> EnumerateTimePoints()
    {
      var current = this.Start;

      while(current < this.End)
      {
        yield return current;
        current = this.Frequency.AddPeriod(current);
      }
    }

    public void Add(string name, TimeSeries timeSeries)
    {
      if (this.timeSeriesDictionary.ContainsKey(name))
      {
        throw new InvalidOperationException($"Time series with name '{name}' already exists.");
      }

      if (this.timeSeries.Count == 0)
      {
        this.Frequency = timeSeries.Frequency;
      }
      else if (timeSeries.Frequency != this.Frequency)
      {
        throw new InvalidOperationException($"Time series frequency ('{timeSeries.Frequency}' must be equal the frequency of the frame ('{this.Frequency}').");
      }

      var pair = new NameTimeSeriesPair(name, timeSeries);
      this.timeSeries.Add(pair);
      this.timeSeriesDictionary.Add(name, timeSeries);

      this.RecalculateStartEnd();
    }
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

    public void ReSample(Period period, AggregationType aggregationType)
    {
      if (this.Frequency == period)
      {
        return;
      }

      this.Frequency = period;
      foreach (var ts in this.timeSeries)
      {
        this[ts.Name] = ts.TimeSeries.ReSample(period, aggregationType);
      }
    }

    public void ReSample(Period period, Dictionary<string, AggregationType> aggragationTypes)
    {
      if (this.Frequency == period)
      {
        return;
      }

      this.Frequency = period;
      foreach (var ts in this.timeSeries)
      {
        if (!aggragationTypes.TryGetValue(ts.Name, out var aggregationType))
        {
          throw new InvalidOperationException($"Unable to resample time series '{ts.Name}' because the aggregation type is not specified.");
        }
        this[ts.Name] = ts.TimeSeries.ReSample(period, aggregationType);
      }
    }

    private void RecalculateStartEnd()
    {
      this.Start = this.timeSeries.Select(p => p.TimeSeries.Start).DefaultIfEmpty(DateTimeOffset.MaxValue).Min();
      this.End = this.timeSeries.Select(p => p.TimeSeries.End).DefaultIfEmpty(DateTimeOffset.MinValue).Max();
    }

    public IEnumerator<KeyValuePair<string, TimeSeries>> GetEnumerator()
    {
      return this.timeSeries.Select(x => new KeyValuePair<string, TimeSeries>(x.Name, x.TimeSeries)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public TimeFrame this[params string[] names]
    {
      get
      {
        var filteredTimeSeries = this.timeSeries.Where(x => names.Contains(x.Name));

        return new TimeFrame(filteredTimeSeries, this.Frequency);
      }
    }
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
    public decimal? this[string name, DateTimeOffset timePoint] => this.timeSeriesDictionary.TryGetValue(name, out var ts) ? ts[timePoint]: null;
  }
}
