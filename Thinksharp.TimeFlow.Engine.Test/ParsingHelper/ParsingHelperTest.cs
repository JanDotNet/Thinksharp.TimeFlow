using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Thinksharp.TimeFlow.Engine.Helper;

namespace Thinksharp.TimeFlow.Engine.Test.ParsingHelper
{
  [TestClass]
  public class ParsingHelperTest
  {
    [TestMethod]
    public void TestString()
    {
      AssertIsValid<string>("abc", "abc");
    }

    [TestMethod]
    public void TestInt()
    {
      AssertIsValid<int>(123, "123");
      AssertIsValid<int>(1000, "1E3");
      AssertIsValid<int>(1232342342, "1232342342");
      AssertIsValid<int?>(123, "123");
      AssertIsValid<int?>(1000, "1E3");
      AssertIsValid<int?>(1232342342, "1232342342");
      AssertIsValid<int?>(null, "null");
      AssertIsValid<int?>(null, "NULL");
      AssertInvalidFormat<int>("hello");
    }

    [TestMethod]
    public void TestLong()
    {
      AssertIsValid<long>(123, "123");
      AssertIsValid<long>(1000, "1E3");
      AssertIsValid<long>(1232342342, "1232342342");
      AssertIsValid<long?>(123, "123");
      AssertIsValid<long?>(1000, "1E3");
      AssertIsValid<long?>(1232342342, "1232342342");
      AssertIsValid<long?>(null, "null");
      AssertIsValid<long?>(null, "NULL");
      AssertInvalidFormat<long>("hello");
    }

    [TestMethod]
    public void TestDecimal()
    {
      AssertIsValid<decimal>(123.432M, "123.432");
      AssertIsValid<decimal>(0.001M, "1E-3");
      AssertIsValid<decimal>(1232342342.432M, "1232342342.432");
      AssertIsValid<decimal?>(123.432M, "123.432");
      AssertIsValid<decimal?>(0.001M, "1E-3");
      AssertIsValid<decimal?>(1232342342.432M, "1232342342.432");
      AssertIsValid<decimal?>(null, "null");
      AssertIsValid<decimal?>(null, "NULL");
      AssertInvalidFormat<decimal>("hello");
      AssertInvalidFormat<decimal?>("hello");
    }

    [TestMethod]
    public void TestDouble()
    {
      AssertIsValid<double>(123.432, "123.432");
      AssertIsValid<double>(0.001, "1E-3");
      AssertIsValid<double>(1232342342.432, "1232342342.432");
      AssertIsValid<double?>(123.432, "123.432");
      AssertIsValid<double?>(0.001, "1E-3");
      AssertIsValid<double?>(1232342342.432, "1232342342.432");
      AssertIsValid<double?>(null, "null");
      AssertIsValid<double?>(null, "NULL");
      AssertInvalidFormat<double>("hello");
      AssertInvalidFormat<double?>("hello");
    }

    [TestMethod]
    public void TestPeriod()
    {
      AssertIsValid<Period>(new Period(1, PeriodUnit.Hour), "h");
      AssertIsValid<Period>(new Period(15, PeriodUnit.Minute), "15 min");
      AssertIsValid<Period>(new Period(3, PeriodUnit.Day), "3d");
      AssertInvalidFormat<Period>("hello");
      AssertInvalidFormat<Period>("hello");
    }

    [TestMethod]
    public void TestDateTime()
    {
      AssertIsValid<DateTime>(new DateTime(2021, 01, 01), "2021-01-01");
      AssertIsValid<DateTime>(new DateTime(2021, 01, 01, 21, 21, 00), "2021-01-01 21:21");
      AssertInvalidFormat<AggregationType>("hello");
    }

    [TestMethod]
    public void TestDateTimeOffset()
    {
      AssertIsValid<DateTimeOffset>(new DateTimeOffset(2021, 01, 01, 0, 0, 0, 0, TimeSpan.FromHours(1)), "2021-01-01");
      AssertIsValid<DateTimeOffset>(new DateTimeOffset(2021, 01, 01, 21, 21, 00, TimeSpan.FromHours(1)), "2021-01-01 21:21");
      AssertInvalidFormat<AggregationType>("hello");
    }

    [TestMethod]
    public void TestEnum()
    {
      AssertIsValid<AggregationType>(AggregationType.Sum, "Sum");
      AssertIsValid<AggregationType>(AggregationType.Mean, "Mean");
      AssertInvalidFormat<AggregationType>("hello");
    }

    [TestMethod]
    public void TestNotSupportedType()
    {
      AssertInvalidType<ParsingHelperTest>("abc");
    }

    private static void AssertIsValid<TValue>(TValue expected, string strValue)
    {
      var result = strValue.Parse(typeof(TValue));
      Assert.AreEqual(ParsingResultType.Succeeded, result.Type);
      Assert.AreEqual(expected, result.Value);
    }

    private static void AssertInvalidFormat<TValue>(string strValue)
    {
      var result = strValue.Parse(typeof(TValue));
      Assert.AreEqual(ParsingResultType.ErrorInvalidFormat, result.Type);
      Assert.AreEqual(null, result.Value);
    }

    private static void AssertInvalidType<TValue>(string strValue)
    {
      var result = strValue.Parse(typeof(TValue));
      Assert.AreEqual(ParsingResultType.ErrorTypeNotSupported, result.Type);
      Assert.AreEqual(null, result.Value);
    }
  }
}
