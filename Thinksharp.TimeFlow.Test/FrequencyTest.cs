using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinksharp.TimeFlow
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class FrequencyTest
  {
    [TestMethod]
    public void Operators_eq()
    {
      Assert.IsTrue(Frequency.Days == Frequency.Days);
      Assert.IsTrue(Frequency.Hours == Frequency.Hours);
      Assert.IsTrue(Frequency.QuarterHours == Frequency.QuarterHours);
      Assert.IsFalse(Frequency.Days == Frequency.Hours);
      Assert.IsFalse(Frequency.Days == Frequency.QuarterHours);
    }

    [TestMethod]
    public void Operators_neq()
    {
      Assert.IsTrue(Frequency.Days != Frequency.Hours);
      Assert.IsTrue(Frequency.Days != Frequency.QuarterHours);
      Assert.IsFalse(Frequency.Days != Frequency.Days);
      Assert.IsFalse(Frequency.Hours != Frequency.Hours);
      Assert.IsFalse(Frequency.QuarterHours != Frequency.QuarterHours);
    }

    [TestMethod]
    public void Operators_gt()
    {
      Assert.IsTrue(Frequency.Days > Frequency.Hours);
      Assert.IsTrue(Frequency.Days > Frequency.QuarterHours);
      Assert.IsFalse(Frequency.Days > Frequency.Days);
      Assert.IsFalse(Frequency.Days > Frequency.Months);
      Assert.IsFalse(Frequency.Days > Frequency.QuarterYears);
      Assert.IsFalse(Frequency.Days > Frequency.Years);
    }

    [TestMethod]
    public void Operators_gte()
    {
      Assert.IsTrue(Frequency.Days >= Frequency.Hours);
      Assert.IsTrue(Frequency.Days >= Frequency.QuarterHours);
      Assert.IsTrue(Frequency.Days >= Frequency.Days);
      Assert.IsFalse(Frequency.Days >= Frequency.Months);
      Assert.IsFalse(Frequency.Days >= Frequency.QuarterYears);
      Assert.IsFalse(Frequency.Days >= Frequency.Years);
    }

    [TestMethod]
    public void Operators_lt()
    {
      Assert.IsFalse(Frequency.Days < Frequency.Hours);
      Assert.IsFalse(Frequency.Days < Frequency.QuarterHours);
      Assert.IsFalse(Frequency.Days < Frequency.Days);
      Assert.IsTrue(Frequency.Days < Frequency.Months);
      Assert.IsTrue(Frequency.Days < Frequency.QuarterYears);
      Assert.IsTrue(Frequency.Days < Frequency.Years);
    }

    [TestMethod]
    public void Operators_lte()
    {
      Assert.IsFalse(Frequency.Days <= Frequency.Hours);
      Assert.IsFalse(Frequency.Days <= Frequency.QuarterHours);
      Assert.IsTrue(Frequency.Days <= Frequency.Days);
      Assert.IsTrue(Frequency.Days <= Frequency.Months);
      Assert.IsTrue(Frequency.Days <= Frequency.QuarterYears);
      Assert.IsTrue(Frequency.Days <= Frequency.Years);
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

        d = Frequency.Hours.AddFreq(d);
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

        d = Frequency.Hours.AddFreq(d);
      }
    }

    [TestMethod]
    public void AddFreq_Day_20210328()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 28));
      var dn = Frequency.Days.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 29)), dn);
    }

    [TestMethod]
    public void AddFreq_Day_20210301()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 01));
      var dn = Frequency.Days.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 02)), dn);
    }

    [TestMethod]
    public void AddFreq_Day_20211031()
    {
      var d = new DateTimeOffset(new DateTime(2021, 10, 31));
      var dn = Frequency.Days.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 11, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_202103()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 01));
      var dn = d + Frequency.Months;

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_20210303()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 03));
      var dn = Frequency.Months + d;

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 03)), dn);
    }

    [TestMethod]
    public void SubtractFreq_Month_20210303()
    {
      var d = new DateTimeOffset(new DateTime(2021, 04, 03));
      var dn = d - Frequency.Months;

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 03, 03)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_20210303_05_23_11()
    {
      var d = new DateTimeOffset(new DateTime(2021, 03, 03, 05, 23, 11));
      var dn = Frequency.Months.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 03, 05, 23, 11)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_20210501()
    {
      var d = new DateTimeOffset(new DateTime(2021, 05, 01));
      var dn = Frequency.Months.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 06, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Month_202110()
    {
      var d = new DateTimeOffset(new DateTime(2021, 10, 01));
      var dn = Frequency.Months.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 11, 01)), dn);
    }
    
    [TestMethod]
    public void AddFreq_QuarterYears_20210101()
    {
      var d = new DateTimeOffset(new DateTime(2021, 01, 01));
      var dn = Frequency.QuarterYears.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 04, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_QuarterYears_20210401()
    {
      var d = new DateTimeOffset(new DateTime(2021, 04, 01));
      var dn = Frequency.QuarterYears.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 07, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_QuarterYears_20210701()
    {
      var d = new DateTimeOffset(new DateTime(2021, 07, 01));
      var dn = Frequency.QuarterYears.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 10, 01)), dn);
    }

    [TestMethod]
    public void AddFreq_Year1_20210101()
    {
      var d = new DateTimeOffset(new DateTime(2021, 01, 01));
      var dn = Frequency.Years.AddFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2022, 01, 01)), dn);
    }

    [TestMethod]
    public void SubtractFreq_Year1_20210101()
    {
      var d = new DateTimeOffset(new DateTime(2022, 01, 01));
      var dn = Frequency.Years.SubtractFreq(d);

      Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 01, 01)), dn);
    }
  }
}
