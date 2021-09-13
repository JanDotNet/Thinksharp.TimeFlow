# Thinksharp.TimeFlow

## Introduction

TimeFlow is a lightweight extendable library for working with time series with support for time zones and daylight saving. The core of the library is the **TimeSeries** type, which holds a list of time point / value pairs. Time points are represented as **DateTimeOffset**s and the time span between two time points (called frequency) is always identical for the whole time series. The values are represented as **nullable decimal**s.

The first and the last time point of a TimeSeries are exposed by the **Start** and **End** properties. The interval between the time points is defined via the **Frequency** property.

TimeSeries objects are **immutable**. There are some methods for transforming TimeSeries, like **ReSample**, **Slice** or **Apply**. Each of these methods return a new immutable object.

A collection of named time series with the same frequency can be combined to one **TimeFrame**.

## Usage

### Creating Time Series

Time series can be created using the available methods of the static **TimeSeries.Factory** property.

**FromValue** creates a time series with a defined value for each time point:

```csharp
    var ts1 = TimeSeries.Factory.FromValue(10, 
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Frequency.Days);

    // ts1:
    // TimePoint with offset           Value
    // 2021-01-01 00:00:00  +01:00     10
    // 2021-01-02 00:00:00  +01:00     10
    // 2021-01-03 00:00:00  +01:00     10
    // 2021-01-04 00:00:00  +01:00     10
    // 2021-01-05 00:00:00  +01:00     10
```

**FromGenerator** creates a time series with a value created as function of the time point:

    var ts1 = TimeSeries.Factory.FromGenerator(
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Frequency.Days,
        tp => tp.Day); // tp: time point as DateTimeOffset

      // ts1:
      // TimePoint with offset           Value
      // 2021-01-01 00:00:00  +01:00     1
      // 2021-01-02 00:00:00  +01:00     2
      // 2021-01-03 00:00:00  +01:00     3
      // 2021-01-04 00:00:00  +01:00     4
      // 2021-01-05 00:00:00  +01:00     5

The factory is of type ITimeSeriesFactory, which can be extended via extension methods. Therefore third party libraries may provide advanced factory methods or user can write it's own use-case specific ones.


### Combining Time Series

TimeSeries can be combined using default operators or with more options via special functions:

      var ts1 = TimeSeries.Factory.FromValue(10,
          new DateTime(2021, 01, 01), // start
          new DateTime(2021, 01, 05), // end
          Frequency.Days);

      var ts2 = TimeSeries.Factory.FromValue(10,
          new DateTime(2021, 01, 01), // start
          new DateTime(2021, 01, 05), // end
          Frequency.Days);

      // combining 2 time series via operators
      var ts3 = ts1 + ts2; // 20, 20, 20, 20, 20
      var ts4 = ts1 * ts2; // 100, 100, 100, 100, 100
      var ts5 = ts1 - ts2; // 0, 0, 0, 0, 0
      var ts6 = ts1 / ts2; // 1, 1, 1, 1, 1

      // apply a function to each value
      var ts7 = ts1 * 13; // 130, 130, 130, 130, 130 
    
There are also methods for combining time series which may be used for advanced usage:

    var ts1 = TimeSeries.Factory.FromValue(1,
          new DateTime(2021, 01, 01), // start
          new DateTime(2021, 01, 05), // end
          Frequency.Days); // 1, 1, 1, 1, 1

      var ts2 = TimeSeries.Factory.FromValue(2,
          new DateTime(2021, 01, 03), // start
          new DateTime(2021, 01, 07), // end
          Frequency.Days); // 2, 2, 2, 2, 2

      var ts3 = ts1.Apply(value => value * 2); // 2, 2, 2, 2, 2 (equivalent to ts1 * 2)

      // join left produces a time series with the same time points as the left time series.
      // note that nulls will be evaluated to null
      var ts4 = ts1.JoinLeft(ts2, (left, right) => left + right); // null, null, 3, 3, 3
      
      // Use pre defined JoinOperation to ignore nulls
      var ts5 = ts1.JoinLeft(ts2, JoinOperation.Add);             // 1, 1, 3, 3, 3

      // join full combines both time series
      // note that nulls will be evaluated to null
      var ts6 = ts1.JoinFull(ts2, (left, right) => left + right); // null, null, 3, 3, 3, null, null

      // Use pre defined JoinOperation to ignore nulls
      var ts7 = ts1.JoinFull(ts2, JoinOperation.Add); // 1, 1, 3, 3, 3, 2, 2 (equvalent to ts1 + ts2)
    
    
Time series can be sliced: 

    var ts1 = TimeSeries.Factory.FromGenerator(
       new DateTime(2021, 01, 01), // start
       new DateTime(2021, 01, 05), // end
       Frequency.Days,
       tp => tp.Day); // 1, 2, 3, 4, 5

     // sliceing by index / count
     var ts2 = ts1.Slice(0, 2); // 1, 2

     // slicing by time range
     var ts3 = ts1.Slice(new DateTime(2021, 01, 02), new DateTime(2021, 01, 04)); // 2, 3, 4
    
Time Series can be resampled which means that the frequency of the time series changes. Frequency may be expressed in milliseconds, seconds, minutes, hours, days, month or years.


    var ts1 = TimeSeries.Factory.FromValue(1,
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 31), // end
        Frequency.Days); // 1, 1, 1, 1, 1

      // down sampling
      var ts2 = ts1.ReSample(Frequency.Months, AggregationType.Sum);     // 2021-01-01 00:00   31
      var ts3 = ts1.ReSample(Frequency.Months, AggregationType.Mean);    // 2021-01-01 00:00   1

      // up sampling
      var ts4 = ts1.ReSample(Frequency.Hours, AggregationType.Sum);
      // 2021-01-01 00:00   0.0416666
      // 2021-01-01 01:00   0.0416666
      // 2021-01-01 02:00   0.0416666
      // ...
      // 2021-01-31 21:00   0.0416666
      // 2021-01-31 22:00   0.0416666
      // 2021-01-31 23:00   0.0416666

      var ts5 = ts1.ReSample(Frequency.Hours, AggregationType.Mean);
      // 2021-01-01 00:00   1
      // 2021-01-01 01:00   1
      // 2021-01-01 02:00   1
      // ...
      // 2021-01-31 21:00   1
      // 2021-01-31 22:00   1
      // 2021-01-31 23:00   1
    
    
    
