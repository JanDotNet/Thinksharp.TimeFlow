using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Thinksharp.TimeFlow
{
  [TestClass]
  public class PeriodTest
  {
    [TestMethod]
    public void Operators_eq()
    {
      Assert.IsTrue(Period.Day == Period.Day);
      Assert.IsTrue(Period.Hour == Period.Hour);
      Assert.IsTrue(Period.QuarterHour == Period.QuarterHour);
      Assert.IsFalse(Period.Day == Period.Hour);
      Assert.IsFalse(Period.Day == Period.QuarterHour);
    }

    [TestMethod]
    public void Operators_neq()
    {
      Assert.IsTrue(Period.Day != Period.Hour);
      Assert.IsTrue(Period.Day != Period.QuarterHour);
      Assert.IsFalse(Period.Day != Period.Day);
      Assert.IsFalse(Period.Hour != Period.Hour);
      Assert.IsFalse(Period.QuarterHour != Period.QuarterHour);
    }

    [TestMethod]
    public void Operators_gt()
    {
      Assert.IsTrue(Period.Day > Period.Hour);
      Assert.IsTrue(Period.Day > Period.QuarterHour);
      Assert.IsFalse(Period.Day > Period.Day);
      Assert.IsFalse(Period.Day > Period.Month);
      Assert.IsFalse(Period.Day > Period.QuarterYear);
      Assert.IsFalse(Period.Day > Period.Year);
    }

    [TestMethod]
    public void Operators_gte()
    {
      Assert.IsTrue(Period.Day >= Period.Hour);
      Assert.IsTrue(Period.Day >= Period.QuarterHour);
      Assert.IsTrue(Period.Day >= Period.Day);
      Assert.IsFalse(Period.Day >= Period.Month);
      Assert.IsFalse(Period.Day >= Period.QuarterYear);
      Assert.IsFalse(Period.Day >= Period.Year);
    }

    [TestMethod]
    public void Operators_lt()
    {
      Assert.IsFalse(Period.Day < Period.Hour);
      Assert.IsFalse(Period.Day < Period.QuarterHour);
      Assert.IsFalse(Period.Day < Period.Day);
      Assert.IsTrue(Period.Day < Period.Month);
      Assert.IsTrue(Period.Day < Period.QuarterYear);
      Assert.IsTrue(Period.Day < Period.Year);
    }

    [TestMethod]
    public void Operators_lte()
    {
      Assert.IsFalse(Period.Day <= Period.Hour);
      Assert.IsFalse(Period.Day <= Period.QuarterHour);
      Assert.IsTrue(Period.Day <= Period.Day);
      Assert.IsTrue(Period.Day <= Period.Month);
      Assert.IsTrue(Period.Day <= Period.QuarterYear);
      Assert.IsTrue(Period.Day <= Period.Year);
    }

    [TestMethod]
    public void AddFreq_Hour_20210328()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 28));
      var daylightSavingSwitch = false;
      for (int h = 0; h <= 23; h++)
      {
        if (!daylightSavingSwitch)
        {
          Assert.AreEqual(h, d.Hour);
          Assert.AreEqual(TimeSpan.FromHours(1), d.Offset);
        }
        else
        {
          Assert.AreEqual(h + 1, d.Hour == 0 ? 24 : d.Hour);
          Assert.AreEqual(TimeSpan.FromHours(2), d.Offset);
        }

        if (h == 1)
        {
          daylightSavingSwitch = true;
        }

        d = Period.Hour.AddPeriod(d);
      }
    }

    [TestMethod]
    public void AddFreq_Hour_20211031()
    {
      var d = new DateTimeOffset(new DateTime(2021, 10, 31));
      var daylightSavingSwitch = false;
      for (int h = 0; h <= 25; h++)
      {
        if (!daylightSavingSwitch)
        {
          Assert.AreEqual(h, d.Hour);
          Assert.AreEqual(TimeSpan.FromHours(2), d.Offset);
        }
        else
        {
          Assert.AreEqual(h - 1, d.Hour == 0 ? 24 : d.Hour);
          Assert.AreEqual(TimeSpan.FromHours(1), d.Offset);
        }

        if (h == 2)
        {
          daylightSavingSwitch = true;
        }

        d = Period.Hour.AddPeriod(d);
      }
    }

    [TestMethod]
    public void AddFreq_Day_20210328()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 28));
      var dn = Period.Day.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 29)), dn);
    }

    [TestMethod]
    public void AddFreq_Day_20210301()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 01));
      var dn = Period.Day.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 02)), dn);
    }

    [TestMethod]
    public void AddFreq_Day_20211031()
    {
      var d = new DateTimeOffset(new DateTime(2021, 10, 31));
      var dn = Period.Day.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 11, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_202103()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 01));
      var dn = d + Period.Month;

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_20210303()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 03));
      var dn = Period.Month + d;

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 03)), dn);
    }

    [TestMethod]
    public void SubtractFreq_Month_20210303()
    {
      var d = new DateTimeOffset(new DateTime(2021, 04, 03));
      var dn = d - Period.Month;

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 03)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_20210303_05_23_11()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 03, 05, 23, 11));
      var dn = Period.Month.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 03, 05, 23, 11)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_20210501()
    {
      var d = new DateTimeOffset(new DateTime(2021, 05, 01));
      var dn = Period.Month.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 06, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_202110()
    {
      var d = new DateTimeOffset(new DateTime(2021, 10, 01));
      var dn = Period.Month.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 11, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_QuarterYears_20210101()
    {
      var d = new DateTimeOffset(new DateTime(2021, 01, 01));
      var dn = Period.QuarterYear.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_QuarterYears_20210401()
    {
      var d = new DateTimeOffset(new DateTime(2021, 04, 01));
      var dn = Period.QuarterYear.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 07, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_QuarterYears_20210701()
    {
      var d = new DateTimeOffset(new DateTime(2021, 07, 01));
      var dn = Period.QuarterYear.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 10, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Year1_20210101()
    {
      var d = new DateTimeOffset(new DateTime(2021, 01, 01));
      var dn = Period.Year.AddPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2022, 01, 01)), dn);
    }

    [TestMethod]
    public void SubtractFreq_Year1_20210101()
    {
      var d = new DateTimeOffset(new DateTime(2022, 01, 01));
      var dn = Period.Year.SubtractPeriod(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 01)), dn);
    }

    [TestMethod]
    public void TestParse()
    {
      var actual = Period.Parse("4 h");
      var expected = new Period(4, PeriodUnit.Hour);
      Assert.AreEqual(expected, actual);

      actual = Period.Parse("h");
      expected = new Period(1, PeriodUnit.Hour);
      Assert.AreEqual(expected, actual);

      actual = Period.Parse("55 yr");
      expected = new Period(55, PeriodUnit.Year);
      Assert.AreEqual(expected, actual);

      Assert.ThrowsException<FormatException>(() => Period.Parse("TEST"));
    }
  }
}
