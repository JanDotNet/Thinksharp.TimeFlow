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
    public string Name { get; set; }
    public int Count => this.timeSeries.Count;
    public DateTimeOffset Start { get; private set; } = DateTimeOffset.MaxValue;
    public DateTimeOffset End { get; private set; } = DateTimeOffset.MinValue;
    public Frequency Frequency { get; private set; }

    public IEnumerable<DateTimeOffset> IterateTimePoints()
    {
      var current = this.Start;

      while(current < this.End)
      {
        yield return current;
        current = this.Frequency.AddFreq(current);
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
        this.Frequency = timeSeries.Freq;
      }
      else if (timeSeries.Freq != this.Frequency)
      {
        throw new InvalidOperationException($"Time series frequency ('{timeSeries.Freq}' must be equal the frequency of the frame ('{this.Frequency}').");
      }

      var pair = new NameTimeSeriesPair(name, timeSeries);
      this.timeSeries.Add(pair);
      this.timeSeriesDictionary.Add(name, timeSeries);

      if (timeSeries.Start < this.Start)
      {
        this.Start = timeSeries.Start;
      }
      if (timeSeries.End > this.End)
      {
        this.End = timeSeries.End;
      }
    }
    public void Remove(string name)
    {
      var existingTimeSeriesPair = this.timeSeries.FirstOrDefault(p => timeSeriesNameComparer.Equals(p.Name, name));

      if (existingTimeSeriesPair != null)
      {
        this.timeSeriesDictionary.Remove(existingTimeSeriesPair.Name);
        this.timeSeries.Remove(existingTimeSeriesPair);

        if (this.Start == existingTimeSeriesPair.TimeSeries.Start)
        {
          this.Start = this.timeSeries.Select(p => p.TimeSeries.Start).DefaultIfEmpty(DateTimeOffset.MaxValue).Min();
        }
        if (this.End == existingTimeSeriesPair.TimeSeries.End)
        {
          this.End = this.timeSeries.Select(p => p.TimeSeries.End).DefaultIfEmpty(DateTimeOffset.MinValue).Max();
        }
      }
    }

    public IEnumerator<KeyValuePair<string, TimeSeries>> GetEnumerator()
    {
      return this.timeSeries.Select(x => new KeyValuePair<string, TimeSeries>(x.Name, x.TimeSeries)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

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
