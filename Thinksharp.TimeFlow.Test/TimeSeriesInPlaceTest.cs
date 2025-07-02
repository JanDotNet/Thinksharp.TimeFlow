namespace Thinksharp.TimeFlow
{
  using System;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TimeSeriesInPlaceTest
  {
    [TestMethod]
    public void TestApply_InPlace_ModifiesOriginalInstance()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.Apply(x => x * 2, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts.All(x => x.Value == 20), "All values should be doubled");
    }

    [TestMethod]
    public void TestApply_InPlace_False_CreatesNewInstance()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.Apply(x => x * 2, inPlace: false);

      // Assert
      Assert.AreNotSame(originalRef, result, "Should return a different instance");
      Assert.IsTrue(ts.All(x => x.Value == 10), "Original should be unchanged");
      Assert.IsTrue(result.All(x => x.Value == 20), "Result should have doubled values");
    }

    [TestMethod]
    public void TestApply_DefaultBehavior_CreatesNewInstance()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.Apply(x => x * 2); // No inPlace parameter

      // Assert
      Assert.AreNotSame(originalRef, result, "Should return a different instance by default");
      Assert.IsTrue(ts.All(x => x.Value == 10), "Original should be unchanged");
      Assert.IsTrue(result.All(x => x.Value == 20), "Result should have doubled values");
    }

    [TestMethod]
    public void TestApplyValues_InPlace_ModifiesOriginalInstance()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.ApplyValues(x => x + 5, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts.All(x => x.Value == 15), "All values should be incremented by 5");
    }

    [TestMethod]
    public void TestJoinLeft_InPlace_ModifiesOriginalInstance()
    {
      // Arrange
      var ts1 = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(3, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts1;

      // Act
      var result = ts1.JoinLeft(ts2, (left, right) => left + right, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts1.All(x => x.Value == 13), "All values should be sum of left and right");
    }

    [TestMethod]
    public void TestJoinLeft_WithJoinOperation_InPlace_ModifiesOriginalInstance()
    {
      // Arrange
      var ts1 = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts1;

      // Act
      var result = ts1.JoinLeft(ts2, JoinOperation.Multiply, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts1.All(x => x.Value == 20), "All values should be multiplied");
    }

    [TestMethod]
    public void TestSlice_InPlace_ModifiesOriginalInstance()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 10, Period.Day);
      var originalRef = ts;
      var originalCount = ts.Count;

      // Act
      var result = ts.Slice(2, 5, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.AreEqual(5, ts.Count, "Should have 5 elements after slice");
      Assert.AreNotEqual(originalCount, ts.Count, "Count should have changed");
      Assert.AreEqual(new DateTime(2023, 1, 3), ts.Start.DateTime, "Start should be third day");
      Assert.AreEqual(new DateTime(2023, 1, 7), ts.End.DateTime, "End should be seventh day");
    }

    [TestMethod]
    public void TestConvenienceMethods_AddInPlace()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.AddInPlace(5);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts.All(x => x.Value == 15), "All values should be incremented by 5");
    }

    [TestMethod]
    public void TestConvenienceMethods_MultiplyInPlace()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.MultiplyInPlace(3);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts.All(x => x.Value == 30), "All values should be multiplied by 3");
    }

    [TestMethod]
    public void TestConvenienceMethods_MethodChaining()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.AddInPlace(5).MultiplyInPlace(2).SubtractInPlace(10);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts.All(x => x.Value == 20), "Should apply all operations: (10+5)*2-10 = 20");
    }

    [TestMethod]
    public void TestConvenienceMethods_AddInPlace_WithTimeSeries()
    {
      // Arrange
      var ts1 = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(7, new DateTime(2023, 1, 1), 5, Period.Day);
      var originalRef = ts1;

      // Act
      var result = ts1.AddInPlace(ts2);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.IsTrue(ts1.All(x => x.Value == 17), "All values should be sum: 10 + 7 = 17");
    }

    [TestMethod]
    public void TestInPlace_WithNullValues()
    {
      // Arrange
      var values = new decimal?[] { 10, null, 20, null, 30 };
      var ts = TimeSeries.Factory.FromValues(values, new DateTime(2023, 1, 1), Period.Day);
      var originalRef = ts;

      // Act
      var result = ts.Apply(x => x.HasValue ? x * 2 : null, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.AreEqual(20, ts[new DateTimeOffset(new DateTime(2023, 1, 1))]);
      Assert.AreEqual(null, ts[new DateTimeOffset(new DateTime(2023, 1, 2))]);
      Assert.AreEqual(40, ts[new DateTimeOffset(new DateTime(2023, 1, 3))]);
      Assert.AreEqual(null, ts[new DateTimeOffset(new DateTime(2023, 1, 4))]);
      Assert.AreEqual(60, ts[new DateTimeOffset(new DateTime(2023, 1, 5))]);
    }

    [TestMethod]
    public void TestInPlace_PreservesFrequencyAndTimeZone()
    {
      // Arrange
      var ts = TimeSeries.Factory.FromValue(10, new DateTime(2023, 1, 1), 5, Period.Hour);
      var originalFrequency = ts.Frequency;
      var originalTimeZone = ts.TimeZone;

      // Act
      ts.Apply(x => x * 2, inPlace: true);

      // Assert
      Assert.AreEqual(originalFrequency, ts.Frequency, "Frequency should be preserved");
      Assert.AreEqual(originalTimeZone, ts.TimeZone, "TimeZone should be preserved");
    }

    [TestMethod]
    public void TestInPlace_EmptyTimeSeries()
    {
      // Arrange
      var ts = TimeSeries.Factory.Empty();
      var originalRef = ts;

      // Act
      var result = ts.Apply(x => x * 2, inPlace: true);

      // Assert
      Assert.AreSame(originalRef, result, "Should return the same instance");
      Assert.AreEqual(0, ts.Count, "Empty time series should remain empty");
      Assert.IsTrue(ts.IsEmpty, "Should still be empty");
    }
  }
}