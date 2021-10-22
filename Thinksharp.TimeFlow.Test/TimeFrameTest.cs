using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinksharp.TimeFlow
{
  [TestClass]
  public class TimeFrameTest
  {
    [TestMethod]
    public void InitFrameWithTwoSeries()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);

      frame.Add("TS1", ts1);
      frame.Add("TS2", ts2);

      Assert.AreEqual(2, frame.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 01)), frame.Start);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 31)), frame.End);
      Assert.AreEqual(Period.Day, frame.Frequency);
    }

    [TestMethod]
    public void ExtendStart()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2020, 01, 01), new DateTime(2021, 01, 31), Period.Day);

      frame.Add("TS1", ts1);
      frame.Add("TS2", ts2);

      Assert.AreEqual(2, frame.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2020, 01, 01)), frame.Start);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 31)), frame.End);
      Assert.AreEqual(Period.Day, frame.Frequency);

      frame.Remove("TS2");

      Assert.AreEqual(1, frame.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 01)), frame.Start);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 31)), frame.End);
      Assert.AreEqual(Period.Day, frame.Frequency);
    }

    [TestMethod]
    public void TestEnumerate()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);

      frame["TS1"] = ts1;
      frame["TS2"] = ts2;

      var names = frame.EnumerateNames().ToArray();
      var timePoints = frame.EnumerateTimePoints().ToArray();
      var timeseries = frame.EnumerateTimeSeries().ToArray();

      var names_exp = new string[] { "TS1", "TS2" };
      var timePoints_exp = Enumerable.Range(1, 31).Select(day => new DateTimeOffset(new DateTime(2021, 01, day))).ToArray();
      var timeSeries_ex = new TimeSeries[] { ts1, ts2 };

      CollectionAssert.AreEqual(names_exp, names);
      CollectionAssert.AreEqual(timePoints_exp, timePoints);
      CollectionAssert.AreEqual(timeSeries_ex, timeseries);
    }

    [TestMethod]
    public void ExtendAndShrinkEnd()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Day);

      frame.Add("TS1", ts1);
      frame.Add("TS2", ts2);

      Assert.AreEqual(2, frame.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 01)), frame.Start);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 31)), frame.End);
      Assert.AreEqual(Period.Day, frame.Frequency);

      frame.Remove("TS2");

      Assert.AreEqual(1, frame.Count);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 01)), frame.Start);
      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 31)), frame.End);
      Assert.AreEqual(Period.Day, frame.Frequency);
    }

    [TestMethod]
    public void DifferentFrquencyNotAllowed()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Month);

      frame.Add("TS1", ts1);
      Assert.ThrowsException<InvalidOperationException>(() => frame.Add("TS2", ts2));
    }

    [TestMethod]
    public void RemoveUntilReset()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 01, 31), Period.Day);

      frame.Add("TS1", ts1);
      frame.Remove("TS1");

      Assert.AreEqual(DateTimeOffset.MinValue, frame.End);
      Assert.AreEqual(DateTimeOffset.MaxValue, frame.Start);
    }

    [TestMethod]
    public void AccessViaNameIndex()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 02, 01), new DateTime(2021, 04, 30), Period.Day);

      frame["TS1"] = ts1;
      frame["TS2"] = ts2;
      frame["TS3"] = frame["TS1"] + frame["TS2"];

      Assert.AreEqual(3, frame.Count);
      Assert.AreEqual(new DateTime(2021, 01, 01), frame.Start);
      Assert.AreEqual(new DateTime(2021, 04, 30), frame.End);
    }

    [TestMethod]
    public void TestForeach()
    {
      var frame = new TimeFrame();

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 02, 01), new DateTime(2021, 04, 30), Period.Day);

      frame.Add("TS1", ts1);
      frame.Add("TS2", ts2);

      var array = frame.ToArray();

      Assert.AreEqual("TS1", array[0].Key);
      Assert.AreEqual(ts1, array[0].Value);
      Assert.AreEqual("TS2", array[1].Key);
      Assert.AreEqual(ts2, array[1].Value);
    }

    [TestMethod]
    public void TestReSample()
    {
      var frame = new TimeFrame();

      frame["TS1"] = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Day);
      frame["TS2"] = TimeSeries.Factory.FromValue(1, new DateTime(2021, 02, 01), new DateTime(2021, 04, 30), Period.Day);

      frame.ReSample(Period.Month, AggregationType.Sum);

      var ts1 = TimeSeries.Factory.FromGenerator(new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Month, ts => DateTime.DaysInMonth(ts.Year, ts.Month));
      var ts2 = TimeSeries.Factory.FromGenerator(new DateTime(2021, 02, 01), new DateTime(2021, 04, 30), Period.Month, ts => DateTime.DaysInMonth(ts.Year, ts.Month));

      Assert.IsTrue(ts1 == frame["TS1"]);
      Assert.IsTrue(ts2 == frame["TS2"]);
    }

    [TestMethod]
    public void TestReSample2()
    {
      var frame = new TimeFrame();

      frame["TS1"] = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Day);
      frame["TS2"] = TimeSeries.Factory.FromValue(1, new DateTime(2021, 02, 01), new DateTime(2021, 04, 30), Period.Day);

      frame.ReSample(Period.Month, new Dictionary<string, AggregationType>
      { 
          { "TS1", AggregationType.Sum },
          { "TS2", AggregationType.Mean}
      });

      var ts1 = TimeSeries.Factory.FromGenerator(new DateTime(2021, 01, 01), new DateTime(2021, 03, 31), Period.Month, ts => DateTime.DaysInMonth(ts.Year, ts.Month));
      var ts2 = TimeSeries.Factory.FromGenerator(new DateTime(2021, 02, 01), new DateTime(2021, 04, 30), Period.Month, ts => 1);

      Assert.IsTrue(ts1 == frame["TS1"]);
      Assert.IsTrue(ts2 == frame["TS2"]);
    }

  }
}
