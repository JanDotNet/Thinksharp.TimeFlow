namespace Thinksharp.TimeFlow
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TimeSeriesTest
  {
    [TestMethod]
    public void TestConst_1_start_count()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, 96, Period.QuarterHour);
      
      Assert.AreEqual(96, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);      
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValue_1_start_end()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.QuarterHour);

      Assert.AreEqual(96, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);      
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValue_1_start_end_march()
    {
      var start = new DateTimeOffset(new DateTime(2021, 03, 28));
      var end = new DateTimeOffset(new DateTime(2021, 03, 28, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.QuarterHour);

      Assert.AreEqual(92, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValue_1_start_end_october()
    {
      var start = new DateTimeOffset(new DateTime(2021, 10, 31));
      var end = new DateTimeOffset(new DateTime(2021, 10, 31, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.QuarterHour);

      Assert.AreEqual(100, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValues()
    {
      var start = new DateTimeOffset(new DateTime(2021, 1, 1));
      var end = new DateTimeOffset(new DateTime(2021, 12, 1));
      var values = Enumerable.Range(1, 12).Select(x => (decimal?)x);
      var ts = TimeSeries.Factory.FromValues(values, start, Period.Month);

      Assert.AreEqual(12, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == x.Key.Month));
    }

    [TestMethod]
    public void TestFromEnumerable()
    {
      var timePoints = Period.Hour.GenerateTimePointSequence(new DateTime(2021, 1, 1)).Take(5).ToList();
      var ts = TimeSeries.Factory.FromEnumerable(timePoints.Select(x => new IndexedSeriesItem<DateTimeOffset, decimal?>(x, x.Hour)));
      Assert.AreEqual(5, ts.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 1, 1)), ts[0].Key);
      Assert.AreEqual(0M, ts[0].Value);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 1, 1).AddHours(4)), ts[4].Key);
      Assert.AreEqual(4M, ts[4].Value);

      Assert.AreEqual(Period.Hour, ts.Frequency);
    }

    [TestMethod]
    public void TestFromValue_Null()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(null, start, 96, Period.QuarterHour);

      Assert.AreEqual(96, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == null));
    }

    [TestMethod]
    public void TestMapValue()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(2, start, 96, Period.QuarterHour);

      Assert.AreEqual(96, ts.Count);
      Assert.IsTrue(ts.All(x => x.Value == 2));

      var ts2 = ts.ApplyValues(x => x * x);
      Assert.IsTrue(ts2.All(x => x.Value == 4));

      var ts3 = ts.Apply(x => (x ?? 0) * (x ?? 0));
      Assert.IsTrue(ts3.All(x => x.Value == 4));
    }

    [TestMethod]
    public void TestSlice_days()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.Day);

      Assert.AreEqual(365, ts.Count);
      Assert.AreEqual(31, ts.Slice(start, Period.Month).Count);
      Assert.AreEqual(31 + 28 + 31, ts.Slice(start, new Period(3, PeriodUnit.Month)).Count);
    }

    [TestMethod]
    public void TestSlice_hours()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 0, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.Hour);

      Assert.AreEqual(365 * 24, ts.Count);
      Assert.AreEqual(24, ts.Slice(start, Period.Day).Count);
      Assert.AreEqual(31 * 24, ts.Slice(start, Period.Month).Count);
      Assert.AreEqual(31 * 24 + 28 * 24 + 31 * 24 -1, ts.Slice(start, new Period(3, PeriodUnit.Month)).Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestSlice_period_must_be_greater_than_timeseries_frequency()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.Day);

      var sts = ts.Slice(start, Period.Minutes);
    }

    [TestMethod]
    public void TestJoinLeft_right_within_left()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end2 = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Period.QuarterHour);

      var joined = left.JoinLeft(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinLeft_right_within_left_op()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end2 = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Period.QuarterHour);

      var joined = left.JoinLeft(right, JoinOperation.Add);

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinLeft_left_within_right()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Period.QuarterHour);

      var joined = left.JoinLeft(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(0, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(0, part3.Count);

      Assert.IsTrue(part2.All(x => x.Value == 4));
    }

    [TestMethod]
    public void TestJoinLeft_left_withJoinOperator()
    {
      var ts1 = TimeSeries.Factory.FromValue(1,
          new DateTime(2021, 01, 01), // start
          new DateTime(2021, 01, 05), // end
          Period.Day);

      var ts2 = TimeSeries.Factory.FromValue(2,
          new DateTime(2021, 01, 03), // start
          new DateTime(2021, 01, 07), // end
          Period.Day);

      // Use pre defined JoinOperation to ignore nulls
      var ts3 = ts1.JoinLeft(ts2, JoinOperation.Add);             // 1, 1, 3, 3, 3

      CollectionAssert.AreEqual(new decimal?[] { 1M, 1M, 3M, 3M, 3M }, ts3.Values.ToArray());
    }

      [TestMethod]
    public void TestJoinFull_right_within_left()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end2 = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Period.QuarterHour);

      var joined = left.JoinFull(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinFull_left_within_right()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Period.QuarterHour);

      var joined = left.JoinFull(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinFull_left_within_right_op()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Period.QuarterHour);

      var joined = left.JoinFull(right, JoinOperation.Add);

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinFull_right_before_right()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Period.QuarterHour);

      var start2 = new DateTimeOffset(new DateTime(2020, 12, 30));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 02, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(5, start2, end2, Period.QuarterHour);

      decimal? Agg(decimal? l, decimal? r) => l == null && r == null ? (decimal?) null : (l ?? 0) + (r ?? 0);
      var joined = left.JoinFull(right, Agg);

      var part1 = joined.Slice(new DateTime(2020, 12, 30), new DateTime(2021, 1, 02, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 03), new DateTime(2021, 01, 09, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));

      Assert.AreEqual(4 * 96, part1.Count);
      Assert.AreEqual(7 * 96, part2.Count);
      Assert.AreEqual(3 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 5));
      Assert.IsTrue(part2.All(x => x.Value == null));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }
      
    [TestMethod]
    public void Test_DownSample_15min_to_hours()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 0, 0));

      // Value: 1
      var ts = TimeSeries.Factory.FromValue(1M, start, 96, Period.QuarterHour);

      var ts_rs = ts.ReSample(Period.Hour, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 4M));

      ts_rs = ts.ReSample(Period.Hour, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 1M));

      // Value: null
      ts = TimeSeries.Factory.FromValue(null, start, 96, Period.QuarterHour);

      ts_rs = ts.ReSample(Period.Hour, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));

      ts_rs = ts.ReSample(Period.Hour, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));
    }

    [TestMethod]
    public void Test_DownSample_15min_to_days()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 0, 0, 0));

      // Value: 1
      var ts = TimeSeries.Factory.FromValue(1M, start, 96, Period.QuarterHour);

      var ts_rs = ts.ReSample(Period.Day, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 96M));

      ts_rs = ts.ReSample(Period.Day, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 1M));

      // Value: null
      ts = TimeSeries.Factory.FromValue(null, start, 96, Period.QuarterHour);

      ts_rs = ts.ReSample(Period.Day, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));

      ts_rs = ts.ReSample(Period.Day, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));
    }

    [TestMethod]
    public void TestDownSample_15min_to_3days()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 3, 0, 0, 0));

      // Value: 1
      var ts = TimeSeries.Factory.FromValue(1M, start, 96 * 3, Period.QuarterHour);

      var ts_rs = ts.ReSample(Period.Day, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 96M));

      ts_rs = ts.ReSample(Period.Day, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 1M));

      // Value: null
      ts = TimeSeries.Factory.FromValue(null, start, 96 * 3, Period.QuarterHour);

      ts_rs = ts.ReSample(Period.Day, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));

      ts_rs = ts.ReSample(Period.Day, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));
    }

    [TestMethod]
    public void TestDownSample_15min_to_month()
    {
      var start = new DateTimeOffset(new DateTime(2021, 03, 01));
      var end = new DateTimeOffset(new DateTime(2021, 04, 01)) - Period.QuarterHour;

      var ts = TimeSeries.Factory.FromValue(1M, start, end, Period.QuarterHour);
      var ts_month_mean = ts.ReSample(Period.Month, AggregationType.Mean);
      var ts_month_sum = ts.ReSample(Period.Month, AggregationType.Sum);

      Assert.AreEqual(1, ts_month_mean.Count);
      Assert.AreEqual(1, ts_month_sum.Count);

      Assert.AreEqual(1M, ts_month_mean[0].Value);
      Assert.AreEqual((decimal)31 * 24 * 4 - 4, ts_month_sum[0].Value);
    }

    [TestMethod]
    public void TestUpSample_from_year_to_quarteryear()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Period.Year);

      var ts_quarter_sum = ts_year.ReSample(Period.QuarterYear, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Period.QuarterYear, AggregationType.Mean);

      Assert.AreEqual(4, ts_quarter_mean.Count);
      Assert.AreEqual(4, ts_quarter_sum.Count);

      Assert.AreEqual(365, ts_quarter_mean[0].Value);
      Assert.AreEqual(89.958, (double)ts_quarter_sum[0].Value, 0.001);

      Assert.AreEqual(365, ts_quarter_mean[1].Value);
      Assert.AreEqual(91M, ts_quarter_sum[1].Value);

      Assert.AreEqual(365, ts_quarter_mean[2].Value);
      Assert.AreEqual(92, (double)ts_quarter_sum[2].Value, 0.01);

      Assert.AreEqual(365, ts_quarter_mean[3].Value);
      Assert.AreEqual(92.0416, (double)ts_quarter_sum[3].Value, 0.01);
    }

    [TestMethod]
    public void TestUpSample_from_year_to_month()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Period.Year);

      var ts_quarter_sum = ts_year.ReSample(Period.Month, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Period.Month, AggregationType.Mean);

      Assert.AreEqual(12, ts_quarter_mean.Count);
      Assert.AreEqual(12, ts_quarter_sum.Count);

      for (int i = 1; i <= 12; i++)
      {
        if (i == 3)
        {
          Assert.AreEqual(30.9583333333333, (double)ts_quarter_sum[i - 1].Value, 0.001);
          
        }
        else if (i == 10)
        {
          Assert.AreEqual(31.0416666666667, (double)ts_quarter_sum[i - 1].Value, 0.001);
        }
        else
        {
          Assert.AreEqual(DateTime.DaysInMonth(2021, i), (double)ts_quarter_sum[i - 1].Value, 0.001);
        }
        Assert.AreEqual(365, ts_quarter_mean[i - 1].Value);
      }
    }

    [TestMethod]
    public void TestUpSample_from_year_to_day()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Period.Year);

      var ts_quarter_sum = ts_year.ReSample(Period.Day, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Period.Day, AggregationType.Mean);

      Assert.AreEqual(365, ts_quarter_mean.Count);
      Assert.AreEqual(365, ts_quarter_sum.Count);

      Assert.IsTrue(ts_quarter_mean.All(x => x.Value == 365));

      foreach (var item in ts_quarter_sum)
      {
        if (item.Key == new DateTimeOffset(new DateTime(2021, 03, 28)))
        {
          Assert.AreEqual(0.9533333, (double)item.Value, 0.01);
        }
        else if (item.Key == new DateTimeOffset(new DateTime(2021, 10, 31)))
        {
          Assert.AreEqual(1.04166666666, (double)item.Value, 0.01);
        }
        else
        {
          Assert.AreEqual(1M, item.Value);
        }
      }
    }

    [TestMethod]
    public void TestUpSample_from_year_to_hour()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Period.Year);

      var ts_quarter_sum = ts_year.ReSample(Period.Hour, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Period.Hour, AggregationType.Mean);

      Assert.AreEqual(365 * 24, ts_quarter_mean.Count);
      Assert.AreEqual(365 * 24, ts_quarter_sum.Count);

      Assert.IsTrue(ts_quarter_mean.All(x => x.Value == 365));
      Assert.IsTrue(ts_quarter_sum.All(x => x.Value == (1 / 24M)));
    }

    [TestMethod]
    public void TestUpSample_from_daily_year_to_hour()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31));

      var ts = TimeSeries.Factory.FromValue(1, start, end, Period.Day);

      var ts_sum = ts.ReSample(Period.Hour, AggregationType.Sum);
      var ts_mean = ts.ReSample(Period.Hour, AggregationType.Mean);

      Assert.AreEqual(365 * 24, ts_sum.Count);
      Assert.AreEqual(365 * 24, ts_mean.Count);

      Assert.IsTrue(ts_mean.All(x => x.Value == 1));
      Assert.IsTrue(ts_sum.GroupBy(x => x.Value).Count() == 3);
    }

    [TestMethod]
    public void TestUpSample_from_year_to_quarterhour()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Period.Year);

      var ts_quarter_sum = ts_year.ReSample(Period.QuarterHour, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Period.QuarterHour, AggregationType.Mean);

      Assert.AreEqual(365 * 96, ts_quarter_mean.Count);
      Assert.AreEqual(365 * 96, ts_quarter_sum.Count);

      Assert.IsTrue(ts_quarter_mean.All(x => x.Value == 365));
      Assert.IsTrue(ts_quarter_sum.All(x => x.Value == (1 / 96M)));
    }

    [TestMethod]
    public void TestTrim()
    {
      var ts1 = TimeSeries.Factory.FromValue(null, new DateTimeOffset(new DateTime(2021, 01, 01)), 48, Period.Hour);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTimeOffset(new DateTime(2021, 01, 01, 2, 0, 0)), 44, Period.Hour);

      var ts = ts1.JoinLeft(ts2, (l, r) => r);

      Assert.AreEqual(48, ts1.Count);
      Assert.IsTrue(ts.Values.Any(v => v == null));
      Assert.IsTrue(ts.Values.Any(v => v == 1));

      // trim full
      var tsTrimFull = ts.Trim();

      Assert.AreEqual(44, tsTrimFull.Count);
      Assert.IsFalse(tsTrimFull.Values.Any(v => v == null));
      Assert.IsTrue(tsTrimFull.Values.Any(v => v == 1));
      Assert.AreEqual(1M, tsTrimFull[0].Value);
      Assert.AreEqual(1M, tsTrimFull[tsTrimFull.Count - 1].Value);

      // trim left
      var tsTrimLeft = ts.Trim(dropTrailing: false);

      Assert.AreEqual(46, tsTrimLeft.Count);
      Assert.AreEqual(1M, tsTrimLeft[0].Value);
      Assert.AreEqual(null, tsTrimLeft[tsTrimLeft.Count - 1].Value);

      // trim right
      var tsTrimRight = ts.Trim(false);

      Assert.AreEqual(46, tsTrimRight.Count);
      Assert.AreEqual(null, tsTrimRight[0].Value);
      Assert.AreEqual(1M, tsTrimRight[tsTrimRight.Count - 1].Value);

      // trim only 0 values
      var tsZeros = TimeSeries.Factory.FromValue(0, new DateTimeOffset(new DateTime(2021, 01, 01)), 48, Period.Hour);

      var tsTrimZeros = tsZeros.Trim(valuesToTrim: new decimal?[] {0M});
      Assert.AreEqual(0, tsTrimZeros.Count);
    }

    [TestMethod]
    public void Test_Operator_Equals()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);

      Assert.IsTrue(ts1 == ts2);
      Assert.IsFalse(ts1 != ts2);
    }

    [TestMethod]
    public void Test_Operator_Equals_Null()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);

      Assert.IsTrue(ts1 != null);
      Assert.IsTrue(null != ts1);
      Assert.IsFalse(ts1 == null);
      Assert.IsFalse(null == ts1);
      Assert.IsFalse((TimeSeries) null != null);
    }

    [TestMethod]
    public void Test_Operator_Add()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_expected = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_actual = ts1 + ts1;

      Assert.AreEqual(ts_expected, ts_actual);
      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);

      Assert.AreNotEqual(ts1, ts_actual);
      Assert.IsFalse(ts1 == ts_actual);
      Assert.IsTrue(ts1 != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_AddEmpty()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_expected = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_actual = ts1 + TimeSeries.Factory.Empty();

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_AddTwoEmpty()
    {
      var ts_actual = TimeSeries.Factory.Empty() + TimeSeries.Factory.Empty();

      Assert.IsTrue(ts_actual == TimeSeries.Factory.Empty());
    }

    [TestMethod]
    public void Test_Operator_Subtract()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_expected = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 5, Period.Day) +
                        TimeSeries.Factory.FromValue(-1, new DateTime(2001, 01, 06), 5, Period.Day);
      var ts_actual = ts1 - ts2;

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_Multiply()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_expected = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Period.Day) +
                        TimeSeries.Factory.FromValue(0, new DateTime(2001, 01, 06), 5, Period.Day);
      var ts_actual = ts1 * ts2;

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_Divide()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 10, Period.Day);
      var ts_expected = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 5, Period.Day)
                        + TimeSeries.Factory.FromValue(0, new DateTime(2001, 01, 06), 5, Period.Day);
      var ts_actual = ts1 / ts2;

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_Numbers()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Period.Day);
      var ts_expected = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Period.Day);
      var ts_actual = ts1 * 2;

      Assert.IsTrue(ts_expected == ts_actual);

      ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Period.Day);
      ts_expected = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Period.Day);
      ts_actual = ts1 + 2;

      Assert.IsTrue(ts_expected == ts_actual);

      ts1 = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Period.Day);
      ts_expected = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Period.Day);
      ts_actual = ts1 - 2;

      Assert.IsTrue(ts_expected == ts_actual);
    }

    [TestMethod]
    public void Test_Join_2()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts3 = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), 10, Period.Day);

      var ts_expected = TimeSeries.Factory.FromValue(6, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts_actual = ts1.JoinFull(ts2, ts3, (x1, x2, x3) => x1 + x2 + x3);

      Assert.IsTrue(ts_actual == ts_expected);
    }

    [TestMethod]
    public void Test_Join_Empty()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Year);
      var ts2 = TimeSeries.Factory.Empty();
      
      var ts_actual1_full = ts1.JoinFull(ts2, JoinOperation.Add);
      var ts_actual2_full = ts2.JoinFull(ts1, JoinOperation.Add);
      var ts_actual1_left = ts1.JoinLeft(ts2, JoinOperation.Add);
      var ts_actual2_left = ts2.JoinLeft(ts1, JoinOperation.Add);

      Assert.IsTrue(ts1 == ts_actual1_full);
      Assert.IsTrue(ts1 == ts_actual2_full);
      Assert.IsTrue(ts1 == ts_actual1_left);
      Assert.IsTrue(ts2 == ts_actual2_left);
    }

    [TestMethod]
    public void Test_Join_Different_Frequencies()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Year);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Month);

      Assert.ThrowsException<InvalidOperationException>(() => ts1.JoinFull(ts2, (x1, x2) => x1 + x2));
      Assert.ThrowsException<InvalidOperationException>(() => ts2.JoinFull(ts1, (x1, x2) => x1 + x2));
      Assert.ThrowsException<InvalidOperationException>(() => ts1.JoinLeft(ts2, (x1, x2) => x1 + x2));
      Assert.ThrowsException<InvalidOperationException>(() => ts2.JoinLeft(ts1, (x1, x2) => x1 + x2));
    }

    [TestMethod]
    public void Test_Join_Different_TimeZones()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Month, TimeZoneInfo.Utc);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Month);

      Assert.ThrowsException<InvalidOperationException>(() => ts1.JoinFull(ts2, (x1, x2) => x1 + x2));
      Assert.ThrowsException<InvalidOperationException>(() => ts2.JoinFull(ts1, (x1, x2) => x1 + x2));
    }

    [TestMethod]
    public void Test_Join_2_Different_Frequencies()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), 10, Period.Month);
      var ts3 = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), 10, Period.Day);

      Assert.ThrowsException<InvalidOperationException>(() => ts1.JoinFull(ts2, ts3, (x1, x2, x3) => x1 + x2 + x3));
      Assert.ThrowsException<InvalidOperationException>(() => ts2.JoinFull(ts1, ts3, (x1, x2, x3) => x1 + x2 + x3));
      Assert.ThrowsException<InvalidOperationException>(() => ts3.JoinFull(ts1, ts2, (x1, x2, x3) => x1 + x2 + x3));
    }

    [TestMethod]
    public void Test_Join_2_Different_TimeZones()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Day, TimeZoneInfo.Utc);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts3 = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), 10, Period.Day);

      Assert.ThrowsException<InvalidOperationException>(() => ts1.JoinFull(ts2, ts3, (x1, x2, x3) => x1 + x2 + x3));
      Assert.ThrowsException<InvalidOperationException>(() => ts2.JoinFull(ts1, ts3, (x1, x2, x3) => x1 + x2 + x3));
      Assert.ThrowsException<InvalidOperationException>(() => ts3.JoinFull(ts1, ts2, (x1, x2, x3) => x1 + x2 + x3));
    }

    [TestMethod]
    public void Test_Join_3()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts3 = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts4 = TimeSeries.Factory.FromValue(4, new DateTime(2021, 01, 01), 10, Period.Day);

      var ts_expected = TimeSeries.Factory.FromValue(10, new DateTime(2021, 01, 01), 10, Period.Day);
      var ts_actual = ts1.JoinFull(ts2, ts3, ts4, (x1, x2, x3, x4) => x1 + x2 + x3 + x4);

      Assert.IsTrue(ts_actual == ts_expected);
    }

    [TestMethod]
    public void Test_Resample_Empty()
    {
      var ts1 = TimeSeries.Factory.Empty();

      var ts_expected = ts1.ReSample(Period.Day, AggregationType.Mean);
      
      Assert.AreEqual(Period.Milliseconds, ts_expected.Frequency);      
    }

    [TestMethod]
    public void Test_Resample_Relative()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01, 06, 00, 00), new DateTime(2021, 01, 03, 05, 00, 00), Period.Hour);

      var opAbs = new ResampleOption() {  ResampleType = ResampleType.Absolut };
      var opRel = new ResampleOption() { ResampleType = ResampleType.Relative };

      var tsAbs = ts1.ReSample(Period.Day, AggregationType.Sum, opAbs);
      var tsRes = ts1.ReSample(Period.Day, AggregationType.Sum, opRel);

      Assert.AreEqual(new DateTime(2021, 01, 01, 06, 00, 00), tsRes.Start);
      Assert.AreEqual(new DateTime(2021, 01, 02, 06, 00, 00), tsRes.End);
      Assert.IsTrue(tsRes.Values.Select(v => v.Value).All(v => v == 24));

      Assert.AreEqual(new DateTime(2021, 01, 01, 00, 00, 00), tsAbs.Start);
      Assert.AreEqual(new DateTime(2021, 01, 03, 00, 00, 00), tsAbs.End);
      Assert.IsTrue(tsAbs[0].Value == 18);
      Assert.IsTrue(tsAbs[1].Value == 24);
      Assert.IsTrue(tsAbs[2].Value == 6);
    }

    [TestMethod]
    public void Test_Shift()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01, 06, 00, 00), new DateTime(2021, 01, 03, 05, 00, 00), Period.Hour);

      var shiftPeriod = new Period(6, PeriodUnit.Hour);
      var ts_L = ts1.ShiftLeft(shiftPeriod);      

      Assert.AreEqual(new DateTime(2021, 01, 01, 00, 00, 00), ts_L.Start.DateTime);
      Assert.AreEqual(new DateTime(2021, 01, 02, 23, 00, 00), ts_L.End.DateTime);

      Assert.AreEqual(ts1.Count, ts_L.Count);

      for (int i = 0; i < ts1.Count; i++)
      {
        Assert.AreEqual(ts1[i].Value, ts_L[i].Value);
        Assert.AreEqual(ts1[i].Key - shiftPeriod, ts_L[i].Key);
      }

      var ts_R = ts1.ShiftRight(shiftPeriod);
      Assert.AreEqual(new DateTime(2021, 01, 01, 12, 00, 00), ts_R.Start.DateTime);
      Assert.AreEqual(new DateTime(2021, 01, 03, 11, 00, 00), ts_R.End.DateTime);

      Assert.AreEqual(ts1.Count, ts_R.Count);

      for (int i = 0; i < ts1.Count; i++)
      {
        Assert.AreEqual(ts1[i].Value, ts_R[i].Value);
        Assert.AreEqual(ts1[i].Key + shiftPeriod, ts_R[i].Key);
      }
    }

    [TestMethod]
    public void Test_Function()
    {
      var ts = TimeSeries.Factory.FromGenerator(new DateTime(2021, 01, 01), new DateTime(2021, 01, 05), Period.Day, x => x.Day);

      Assert.AreEqual(5M, ts.Count);
      Assert.AreEqual(1M, ts[new DateTime(2021, 01, 01)]);
      Assert.AreEqual(2M, ts[new DateTime(2021, 01, 02)]);
      Assert.AreEqual(3M, ts[new DateTime(2021, 01, 03)]);
      Assert.AreEqual(4M, ts[new DateTime(2021, 01, 04)]);
      Assert.AreEqual(5M, ts[new DateTime(2021, 01, 05)]);
    }

    [TestMethod]
    public void Test_ToTsv()
    {
      var ts = TimeSeries.Factory.FromGenerator(new DateTime(2021, 01, 01), new DateTime(2021, 01, 05), Period.Day, x => x.Day);

      var expected =
        @"01.01.2021 00:00:00 +01:00	1
02.01.2021 00:00:00 +01:00	2
03.01.2021 00:00:00 +01:00	3
04.01.2021 00:00:00 +01:00	4
05.01.2021 00:00:00 +01:00	5";

      Assert.AreEqual(expected, ts.ToTsv(new CultureInfo("de-DE")));
    }

    [TestMethod]
    public void TestFromDictionary_Simple()
    {
      var timePoints = Period.Hour.GenerateTimePointSequence(new DateTime(2021, 1, 1)).Take(5).ToDictionary(x => new DateTimeOffset(x.DateTime), x => (decimal?)x.Hour);
      var ts = TimeSeries.Factory.FromDictionary(timePoints, Period.Hour);
      Assert.AreEqual(5, ts.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 1, 1)), ts[0].Key);
      Assert.AreEqual(0M, ts[0].Value);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 1, 1).AddHours(4)), ts[4].Key);
      Assert.AreEqual(4M, ts[4].Value);

      Assert.AreEqual(Period.Hour, ts.Frequency);
    }

    [TestMethod]
    public void TestFromDictionary_SimpleWithGaps()
    {
      var dictionary = new Dictionary<DateTimeOffset, decimal?>();
      Period.Day.GenerateTimePointSequence(new DateTime(2021, 1, 1)).Take(5).ToList().ForEach(i => dictionary.Add(i, i.Date.Day));
      Period.Day.GenerateTimePointSequence(new DateTime(2021, 2, 1)).Take(4).ToList().ForEach(i => dictionary.Add(i, i.Date.Day));
      
      var ts = TimeSeries.Factory.FromDictionary(dictionary, Period.Day);
      Assert.AreEqual(35, ts.Count);

      for (int i = 0; i < 31; i++)
      {
        Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 1, i+1)), ts[i].Key);
        if (i < 5)
          Assert.AreEqual((decimal)i+1, ts[i].Value);
        else
          Assert.AreEqual((decimal?)null, ts[i].Value);
      }
      for (int i = 0; i < 4; i++)
      {
        Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 2, i+1)), ts[31+i].Key);
        Assert.AreEqual((decimal)i+1, ts[31 + i].Value);
      }

      Assert.AreEqual(Period.Day, ts.Frequency);
    }

    [TestMethod]
    public void TestFromDictionary_WithDates()
    {
      var timePoints = Period.Day.GenerateTimePointSequence(new DateTime(2021, 1, 5)).Take(5).ToDictionary(x => new DateTimeOffset(x.DateTime), x => (decimal?)x.Day);
      var ts = TimeSeries.Factory.FromDictionary(timePoints, new DateTime(2021, 1, 1), new DateTime(2021, 1, 31), Period.Day);
      for (int i = 0; i < 31; i++)
      {
        Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 1, i + 1)), ts[i].Key);
        if (i >= 4 && i <= 8)
          Assert.AreEqual((decimal)i+1, ts[i].Value);
        else
          Assert.AreEqual((decimal?)null, ts[i].Value);
      }

      Assert.AreEqual(Period.Day, ts.Frequency);
    }
  }
}