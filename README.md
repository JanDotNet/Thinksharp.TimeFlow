# Thinksharp.TimeFlow

## Introduction

TimeFlow is a lightweight extendable library for working with time series. The core of the library is the **TimeSeries** type, which holds a list of time point / value pairs. Time points are represented as **DateTimeOffset**s and values as **nullable decimal**s.

The first and the last time point of a TimeSeries are exposed by the **Start** and **End** properties. The interval between the time points is defined via the **Frequency**.

TimeSeries objects are **immutable**. There are some methods for transforming TimeSeries, like **ReSample**, **Slice** or **Apply**. Each of these methods return a new immutable object.

The **TimeFrame** holds a collection of named time series with same  frequency. 

## Usage

### Creating Time Series

Time series can be created using the available methods of the static **TimeSeries.Factory** property.

**FromValue** creates a time series with a defined value for each time point:

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

**FromGenerator** creates a time series with a value created as function of the time point:

    var ts1 = TimeSeries.Factory.FromGeneratir(
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
    var ts4 = ts1 - ts2; // 0, 0, 0, 0, 0
    var ts5 = ts1 / ts2; // 1, 1, 1, 1, 1

    // apply a function to each value
    var ts6 = ts1 * 13; // 130, 130, 130, 130, 130   