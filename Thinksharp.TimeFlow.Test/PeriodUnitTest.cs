using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Thinksharp.TimeFlow
{
  [TestClass]
  public class PeriodUnitUnitTest
  {
    [TestMethod]
    public void Operators_eq()
    {
      Assert.IsTrue(PeriodUnit.Millisecond == PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Second == PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Minute == PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Hour == PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Day == PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Month == PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Year == PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year == null);
      Assert.IsFalse(null == PeriodUnit.Year);
      Assert.IsTrue((PeriodUnit)null == (PeriodUnit)null);
    }

    [TestMethod]
    public void Operators_neq()
    {
      Assert.IsTrue(PeriodUnit.Millisecond != PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Millisecond != PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Millisecond != PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Millisecond != PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Millisecond != PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Millisecond != PeriodUnit.Year);
                                          
      Assert.IsTrue(PeriodUnit.Second != PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Second != PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Second != PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Second != PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Second != PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Second != PeriodUnit.Year);
                                           
      Assert.IsTrue(PeriodUnit.Minute != PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Minute != PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Minute != PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Minute != PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Minute != PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Minute != PeriodUnit.Year);
                                           
      Assert.IsTrue(PeriodUnit.Hour != PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Hour != PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Hour != PeriodUnit.Minute);      
      Assert.IsTrue(PeriodUnit.Hour != PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Hour != PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Hour != PeriodUnit.Year);
                                           
      Assert.IsTrue(PeriodUnit.Day != PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Day != PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Day != PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Day != PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Day != PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Day != PeriodUnit.Year);
                                         
      Assert.IsTrue(PeriodUnit.Month != PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Month != PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Month != PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Month != PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Month != PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Month != PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Year != PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Year != PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Year != PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Year != PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Year != PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Year != PeriodUnit.Month);

      Assert.IsTrue(PeriodUnit.Year != null);
      Assert.IsTrue(null != PeriodUnit.Year);
      Assert.IsFalse((PeriodUnit)null != (PeriodUnit)null);
    }

    [TestMethod]
    public void Operators_gt()
    {
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Millisecond > PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Second > PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Second > PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Second > PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Second > PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Second > PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Second > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Second > PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Minute > PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Minute > PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Minute > PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Minute > PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Minute > PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Minute > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Minute > PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Hour > PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Hour > PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Hour > PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Hour > PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Hour > PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Hour > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Hour > PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Day > PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Day > PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Day > PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Day > PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Day > PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Day > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Day > PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Month > PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Month > PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Month > PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Month > PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Month > PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Month > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Month > PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Year > PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Year > PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Year > PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Year > PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Year > PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Year > PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Year > PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year > null);
      Assert.IsFalse(null > PeriodUnit.Year);
      Assert.IsFalse((PeriodUnit)null > (PeriodUnit)null);
    }

    [TestMethod]
    public void Operators_gte()
    {
      Assert.IsTrue(PeriodUnit.Millisecond >= PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Millisecond >= PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Millisecond >= PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Millisecond >= PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Millisecond >= PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Millisecond >= PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Millisecond >= PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Second >= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Second >= PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Second >= PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Second >= PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Second >= PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Second >= PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Second >= PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Minute >= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Minute >= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Minute >= PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Minute >= PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Minute >= PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Minute >= PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Minute >= PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Hour >= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Hour >= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Hour >= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Hour >= PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Hour >= PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Hour >= PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Hour >= PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Day >= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Day >= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Day >= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Day >= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Day >= PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Day >= PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Day >= PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Month >= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Month >= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Month >= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Month >= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Month >= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Month >= PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Month >= PeriodUnit.Year);

      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Year >= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year >= null);
      Assert.IsFalse(null >= PeriodUnit.Year);
      Assert.IsTrue((PeriodUnit)null >= (PeriodUnit)null);
    }

    [TestMethod]
    public void Operators_lt()
    {
      Assert.IsFalse(PeriodUnit.Millisecond < PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Millisecond < PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Millisecond < PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Millisecond < PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Millisecond < PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Millisecond < PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Millisecond < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Second < PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Second < PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Second < PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Second < PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Second < PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Second < PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Second < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Minute < PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Minute < PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Minute < PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Minute < PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Minute < PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Minute < PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Minute < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Hour < PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Hour < PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Hour < PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Hour < PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Hour < PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Hour < PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Hour < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Day < PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Day < PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Day < PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Day < PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Day < PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Day < PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Day < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Month < PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Month < PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Month < PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Month < PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Month < PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Month < PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Month < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Month);
      Assert.IsFalse(PeriodUnit.Year < PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year < null);
      Assert.IsFalse(null < PeriodUnit.Year);
      Assert.IsFalse((PeriodUnit)null < (PeriodUnit)null);
    }

    [TestMethod]
    public void Operators_lte()
    {
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Millisecond <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Second <= PeriodUnit.Millisecond);
      Assert.IsTrue(PeriodUnit.Second <= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Second <= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Second <= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Second <= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Second <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Second <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Minute <= PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Minute <= PeriodUnit.Second);
      Assert.IsTrue(PeriodUnit.Minute <= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Minute <= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Minute <= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Minute <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Minute <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Hour <= PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Hour <= PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Hour <= PeriodUnit.Minute);
      Assert.IsTrue(PeriodUnit.Hour <= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Hour <= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Hour <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Hour <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Day <= PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Day <= PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Day <= PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Day <= PeriodUnit.Hour);
      Assert.IsTrue(PeriodUnit.Day <= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Day <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Day <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Month <= PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Month <= PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Month <= PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Month <= PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Month <= PeriodUnit.Day);
      Assert.IsTrue(PeriodUnit.Month <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Month <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year <= PeriodUnit.Millisecond);
      Assert.IsFalse(PeriodUnit.Year <= PeriodUnit.Second);
      Assert.IsFalse(PeriodUnit.Year <= PeriodUnit.Minute);
      Assert.IsFalse(PeriodUnit.Year <= PeriodUnit.Hour);
      Assert.IsFalse(PeriodUnit.Year <= PeriodUnit.Day);
      Assert.IsFalse(PeriodUnit.Year <= PeriodUnit.Month);
      Assert.IsTrue(PeriodUnit.Year <= PeriodUnit.Year);

      Assert.IsFalse(PeriodUnit.Year <= null);
      Assert.IsFalse(null <= PeriodUnit.Year);
      Assert.IsTrue((PeriodUnit)null <= (PeriodUnit)null);
    }

  }
}
