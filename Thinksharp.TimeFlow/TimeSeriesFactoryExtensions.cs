using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinksharp.TimeFlow
{
  internal class TimeSeriesFactory : ITimeSeriesFactory
  { }

  public interface ITimeSeriesFactory
  { }

  public static class TimeSeriesFactoryExtensions
  {
    /// <summary>
    /// Creates an empty time series.
    /// </summary>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static TimeSeries Empty(this ITimeSeriesFactory factory)
    {
      return new TimeSeries(Enumerable.Empty<IndexedSeriesItem<DateTimeOffset, decimal?>>(), DateHelper.EmptyTimeSeriesFrequency, DateHelper.EmptyTimeSeriesZoneInfo);
    }

    /// <summary>
    ///   Creates a new time series with N constant values.
    /// </summary>
    /// <param name="timePoints">
    ///   An  enumerable of time point / value pairs to create the time series for.
    ///   NOTE: The period between the time points must be equal!
    /// </param>    
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with N constant values.
    /// </returns>
    public static TimeSeries FromEnumerable(this ITimeSeriesFactory factory, IEnumerable<IndexedSeriesItem<DateTimeOffset, decimal?>> timePoints, TimeZoneInfo timeZone = null)
    {
      var timePointList = timePoints.OrderBy(x => x.Key).ToList();

      var period = timePointList.Count < 2
        ? Period.Day
        : Period.FromTimePoints(timePointList[0].Key, timePointList[1].Key);

      return new TimeSeries(timePointList, period, timeZone ?? DateHelper.GetDefaultTimeZone());
    }

    /// <summary>
    ///   Creates a new time series with N constant values.
    /// </summary>
    /// <param name="value">
    ///   The constant value to use.
    /// </param>
    /// <param name="startDate">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="count">
    ///   The number of time points to generate.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with N constant values.
    /// </returns>
    public static TimeSeries FromValue(this ITimeSeriesFactory factory, decimal? value, DateTime startDate, int count, Period freq, TimeZoneInfo timeZone = null)
    {
      return factory.FromValue(value, new DateTimeOffset(startDate), count, freq, timeZone);
    }

    /// <summary>
    ///   Creates a new time series with the specified values.
    /// </summary>
    /// <param name="values">
    ///   The values to use.
    /// </param>
    /// <param name="startDate">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the specified values.
    /// </returns>
    public static TimeSeries FromValues(this ITimeSeriesFactory factory, IEnumerable<decimal?> values, DateTime startDate, Period freq, TimeZoneInfo timeZone = null)
      => factory.FromValues(values, new DateTimeOffset(startDate), freq, timeZone);

    /// <summary>
    ///   Creates a new time series with the specified values.
    /// </summary>
    /// <param name="values">
    ///   The values to use.
    /// </param>
    /// <param name="startDate">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the specified values.
    /// </returns>
    public static TimeSeries FromValues(this ITimeSeriesFactory factory, IEnumerable<decimal?> values, DateTimeOffset startDate, Period freq, TimeZoneInfo timeZone = null)
    {
      timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      var date = startDate;
      foreach (var value in values)
      {
        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(date, value));
        date = freq.AddPeriod(date, timeZone);
      }

      return new TimeSeries(result, freq, timeZone);
    }

    /// <summary>
    ///   Creates a new time series with N constant values.
    /// </summary>
    /// <param name="value">
    ///   The constant value to use.
    /// </param>
    /// <param name="startDate">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="count">
    ///   The number of time points to generate.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with N constant values.
    /// </returns>
    public static TimeSeries FromValue(this ITimeSeriesFactory factory, decimal? value, DateTimeOffset startDate, int count, Period freq, TimeZoneInfo timeZone = null)
    {
      if (count <= 0)
      {
        throw new ArgumentException($"{nameof(count)} must be > 0");
      }

      timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      var date = startDate;
      for (var i = 0; i < count; i++)
      {
        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(date, value));
        date = freq.AddPeriod(date, timeZone);
      }

      return new TimeSeries(result, freq, timeZone);
    }

    /// <summary>
    ///   Creates a new time series with constant values between a specified time range.
    /// </summary>
    /// <param name="value">
    ///   The constant value to use.
    /// </param>
    /// <param name="startDate">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="end">
    ///   The last time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with N constant values.
    /// </returns>
    public static TimeSeries FromValue(this ITimeSeriesFactory factory, decimal? value, DateTime startDate, DateTime end, Period freq, TimeZoneInfo timeZone = null)
    {
      return factory.FromValue(value, new DateTimeOffset(startDate), new DateTimeOffset(end), freq, timeZone);
    }

    /// <summary>
    ///   Creates a new time series with constant values between a specified time range.
    /// </summary>
    /// <param name="value">
    ///   The constant value to use.
    /// </param>
    /// <param name="startDate">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="end">
    ///   The last time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with N constant values.
    /// </returns>
    public static TimeSeries FromValue(this ITimeSeriesFactory factory, decimal? value, DateTimeOffset startDate, DateTimeOffset end, Period freq, TimeZoneInfo timeZone = null)
    {
      timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

      var result = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();
      var date = startDate;
      while (date <= end)
      {
        result.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(date, value));
        date = freq.AddPeriod(date, timeZone);
      }

      return new TimeSeries(result, freq, timeZone);
    }

    /// <summary>
    ///   Creates a new time series with generated values.
    /// </summary>
    /// <param name="firstTimePoint">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="lastTimePoint">
    ///   The last time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="generator">
    ///   Generator to generate the values based on the time point.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the generated values.
    /// </returns>
    public static TimeSeries FromGenerator(this ITimeSeriesFactory factory, DateTime firstTimePoint, DateTime lastTimePoint, Period freq, Func<DateTimeOffset, decimal?> generator, TimeZoneInfo timeZone = null)
      => factory.FromGenerator(new DateTimeOffset(firstTimePoint), new DateTimeOffset(lastTimePoint), freq, generator, timeZone);

    /// <summary>
    ///   Creates a new time series with generated values.
    /// </summary>
    /// <param name="firstTimePoint">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="lastTimePoint">
    ///   The last time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use for generation.
    /// </param>
    /// <param name="generator">
    ///   Generator to generate the values based on the time point.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the generated values.
    /// </returns>
    public static TimeSeries FromGenerator(this ITimeSeriesFactory factory, DateTimeOffset firstTimePoint, DateTimeOffset lastTimePoint, Period freq, Func<DateTimeOffset, decimal?> generator, TimeZoneInfo timeZone = null)
    {
      timeZone = timeZone ?? DateHelper.GetDefaultTimeZone();

      if (firstTimePoint > lastTimePoint)
      {
        throw new InvalidOperationException("firstTimePoint must be before or equal to lastTimePoint");
      }

      var currentTimePoint = firstTimePoint;
      var timePoints = new List<IndexedSeriesItem<DateTimeOffset, decimal?>>();

      while (currentTimePoint <= lastTimePoint)
      {
        timePoints.Add(new IndexedSeriesItem<DateTimeOffset, decimal?>(currentTimePoint, generator(currentTimePoint)));

        currentTimePoint = freq.AddPeriod(currentTimePoint, timeZone);
      }

      return new TimeSeries(timePoints, freq, timeZone);
    }

    /// <summary>
    ///   Creates a new time series for the specified dictionary.
    ///   The times series starts by the first and ends with the last time time point in the dictionary.
    /// </summary>
    /// <param name="timePoints">
    ///   The dictionary that contains the time points to use.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use. Note that the frequency have to match the time points in the dictionary.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the generated values.
    /// </returns>
		public static TimeSeries FromDictionary(this ITimeSeriesFactory factory, IDictionary<DateTimeOffset, decimal?> timePoints, Period freq, TimeZoneInfo timeZone = null)
	    => factory.FromGenerator(timePoints.Keys.Min(), timePoints.Keys.Max(), freq, tp => timePoints.TryGetValue(tp, out var val) ? val : null, timeZone);

    /// <summary>
    ///   Creates a new time series for the specified dictionary.
    ///   The times series starts by the first and ends with the last time time point in the dictionary.
    /// </summary>
    /// <param name="timePoints">
    ///   The dictionary that contains the time points to use.
    /// </param>
    /// <param name="firstTimePoint">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="lastTimePoint">
    ///   The last time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use. Note that the frequency have to match the time points in the dictionary.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the generated values.
    /// </returns>
    public static TimeSeries FromDictionary(this ITimeSeriesFactory factory, IDictionary<DateTimeOffset, decimal?> timePoints, DateTimeOffset firstTimePoint, DateTimeOffset lastTimePoint, Period freq, TimeZoneInfo timeZone = null)
      => factory.FromGenerator(firstTimePoint, lastTimePoint, freq, tp => timePoints.TryGetValue(tp, out var val) ? val : null, timeZone);

    /// <summary>
    ///   Creates a new time series for the specified dictionary.
    ///   The times series starts by the first and ends with the last time time point in the dictionary.
    /// </summary>
    /// <param name="timePoints">
    ///   The dictionary that contains the time points to use.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use. Note that the frequency have to match the time points in the dictionary.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the generated values.
    /// </returns>
    public static TimeSeries FromDictionary(this ITimeSeriesFactory factory, IDictionary<DateTimeOffset, decimal> timePoints, Period freq, TimeZoneInfo timeZone = null)
      => factory.FromGenerator(timePoints.Keys.Min(), timePoints.Keys.Max(), freq, tp => timePoints.TryGetValue(tp, out var val) ? (decimal?)val : null, timeZone);

    /// <summary>
    ///   Creates a new time series for the specified dictionary.
    ///   The times series starts by the first and ends with the last time time point in the dictionary.
    /// </summary>
    /// <param name="timePoints">
    ///   The dictionary that contains the time points to use.
    /// </param>
    /// <param name="firstTimePoint">
    ///   The first time point of the time series.
    /// </param>
    /// <param name="lastTimePoint">
    ///   The last time point of the time series.
    /// </param>
    /// <param name="freq">
    ///   The frequency to use. Note that the frequency have to match the time points in the dictionary.
    /// </param>
    /// <param name="timeZone">
    ///   The time zone to use. (Default: 'W. Europe Standard Time')
    /// </param>
    /// <returns>
    ///   A new time series with the generated values.
    /// </returns>
    public static TimeSeries FromDictionary(this ITimeSeriesFactory factory, IDictionary<DateTimeOffset, decimal> timePoints, DateTimeOffset firstTimePoint, DateTimeOffset lastTimePoint, Period freq, TimeZoneInfo timeZone = null)
      => factory.FromGenerator(firstTimePoint, lastTimePoint, freq, tp => timePoints.TryGetValue(tp, out var val) ? (decimal?)val : null, timeZone);
  }
}
