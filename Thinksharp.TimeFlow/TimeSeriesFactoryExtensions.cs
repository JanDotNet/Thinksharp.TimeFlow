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
    public static TimeSeries Empty(this ITimeSeriesFactory facotry, Period freq, TimeZoneInfo timeZone = null)
    {
      return new TimeSeries(Enumerable.Empty<IndexedSeriesItem<DateTimeOffset, decimal?>>(), freq, timeZone);
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
  }
}
