using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinksharp.TimeFlow
{
  [TestClass]
  public class DateHelperTest
  {
    [TestMethod]
    public void TestGetHoursOfDay()
    {
      Assert.AreEqual(24, DateHelper.GetHoursOfDay(new DateTime(2021, 01, 01)));
      Assert.AreEqual(23, DateHelper.GetHoursOfDay(new DateTime(2021, 03, 28)));
      Assert.AreEqual(25, DateHelper.GetHoursOfDay(new DateTime(2021, 10, 31)));
    }

    [TestMethod]
    public void TestGetQuarterYear()
    {
      Assert.AreEqual(1, DateHelper.GetQuarterYear(new DateTime(2021, 01, 01)));
      Assert.AreEqual(1, DateHelper.GetQuarterYear(new DateTime(2021, 03, 31, 23, 59, 59)));

      Assert.AreEqual(2, DateHelper.GetQuarterYear(new DateTime(2021, 04, 01)));
      Assert.AreEqual(2, DateHelper.GetQuarterYear(new DateTime(2021, 06, 30, 23, 59, 59)));

      Assert.AreEqual(3, DateHelper.GetQuarterYear(new DateTime(2021, 07, 01)));
      Assert.AreEqual(3, DateHelper.GetQuarterYear(new DateTime(2021, 09, 30, 23, 59, 59)));

      Assert.AreEqual(4, DateHelper.GetQuarterYear(new DateTime(2021, 10, 01)));
      Assert.AreEqual(4, DateHelper.GetQuarterYear(new DateTime(2021, 12, 31, 23, 59, 59)));
    }    
  }
}
