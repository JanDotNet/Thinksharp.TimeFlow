# Thinksharp.TimeFlow

[![NuGet](https://img.shields.io/nuget/v/Thinksharp.TimeFlow.svg)](https://www.nuget.org/packages/Thinksharp.TimeFlow/) 
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MSBFDUU5UUQZL)

## Introduction

TimeFlow is a lightweight extendable library for working with time series with support for time zones and daylight saving. The core of the library is the **TimeSeries** type, which holds a list of time point / value pairs. Time points are represented as **DateTimeOffset**s. The time span between two time points (called frequency) is always identical for the whole time series. Values are represented as **nullable decimal**s.

The first and the last time points of a TimeSeries are exposed by the **Start** and **End** properties. The interval between time points is exposed as **Frequency** property.

TimeSeries objects are **immutable**. There are some methods for transforming TimeSeries, like **ReSample**, **Slice** or **Apply**. Each of these methods return a new immutable object.

A collection of named time series with the same frequency can be combined to one **TimeFrame**.

## Installation

### Usage in .Net Projects

ThinkSharp.TimeFlow can be installed via [Nuget](https://www.nuget.org/packages/Thinksharp.TimeFlow)

    Install-Package Thinksharp.TimeFlow

### Usage

#### Time Series Creation

Time series can be created using the available methods of the static TimeSeries.Factory property. The factory is of type ITimeSeriesFactory, which can be extended via extension methods. Therefore third party libraries may provide advanced factory methods or user can write it's own use-case specific ones.
TimeSeries.Factory.FromValue

    // FromValue creates a time series with a defined value for each time point:
    var ts = TimeSeries.Factory.FromValue(10,
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Period.Day);
     
    ts


|time	|value|
| ------------- |:-------------:| 
|2021-01-01 00:00:00Z|	10
|2021-01-02 00:00:00Z|	10
|2021-01-03 00:00:00Z|	10
|2021-01-04 00:00:00Z|	10
|2021-01-05 00:00:00Z|	10

#### TimeSeries.Factory.FromValues

    // FromValues creates a time series from an enumerable of values:
    var values = new decimal?[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
    var ts = TimeSeries.Factory.FromValues(values,
        new DateTime(2021, 01, 01), // start
        Period.Day);

    ts

|time|	value|
| ------------- |:-------------:| 
|2021-01-01 00:00:00Z|  1
|2021-01-02 00:00:00Z|  2
|2021-01-03 00:00:00Z|  3
|2021-01-04 00:00:00Z|  4
|2021-01-05 00:00:00Z|  5
|2021-01-06 00:00:00Z|  6
|2021-01-07 00:00:00Z|  7
|2021-01-08 00:00:00Z|  8
|2021-01-09 00:00:00Z|  9

#### TimeSeries.Factory.FromGenerator

    // FromGenerator creates a time series with a value created as function of the time point:
    var ts = TimeSeries.Factory.FromGenerator(
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Period.Day,
        tp => tp.Day); // tp: time point as DateTimeOffset

    ts

|time|	value|
| ------------- | ------------- |
|01/01/2021 00:00:00 +01:00|1|
|02/01/2021 00:00:00 +01:00|2|
|03/01/2021 00:00:00 +01:00|3|
|04/01/2021 00:00:00 +01:00|4|
|05/01/2021 00:00:00 +01:00|5|

####Time Series Transformation

Combining Time Series via Operators

    // time series can be combined using default operators:
    var a = TimeSeries.Factory.FromValue(10,
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Period.Day);

    var b = TimeSeries.Factory.FromGenerator(
        new DateTime(2021, 01, 01), // start   
        new DateTime(2021, 01, 05), // end 
        Period.Day,
        tp => tp.Day);

    var tf = new TimeFrame();
    tf["a"] = a;
    tf["b"] = b;
    tf["a + b"] = a + b;
    tf["a - b"] = a - b;
    tf["a * b"] = a * b;
    tf["a / b"] = a / b;
    tf["a * 12"] = a * 12;
    tf

|Date|a|b|a + b|a - b|a * b|a / b|a * 12|
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
|01/01/2021 00:00:00 +01:00|10|1|11|9|10|10|120|
|02/01/2021 00:00:00 +01:00|10|2|12|8|20|5|120|
|03/01/2021 00:00:00 +01:00|10|3|13|7|30|3,3333333333333333333333333333|120|
|04/01/2021 00:00:00 +01:00|10|4|14|6|40|2,5|120|
|05/01/2021 00:00:00 +01:00|10|5|15|5|50|2|120|


Combining Time Series via Methods

    var a = TimeSeries.Factory.FromValue(1,
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Period.Day);

    var b = TimeSeries.Factory.FromValue(2,
      new DateTime(2021, 01, 03), // start
      new DateTime(2021, 01, 07), // end
      Period.Day);

    var tf = new TimeFrame();
    tf["a"] = a;
    tf["b"] = b;
    tf["apply * 2"] = a.Apply(value => value * 2);

    // join left produces a time series with the same time points as the left time series.
    // note that nulls will be evaluated to null
    tf["JoinLeft r + l"] = a.JoinLeft(b, (l, r) => l + r);
    // Use pre defined JoinOperation to ignore nulls
    tf["JoinLeft JoinOperation.Add"] = a.JoinLeft(b, JoinOperation.Add);   

    // join full combines both time series
    // note that nulls will be evaluated to null
    tf["JoinFull r + l"] = a.JoinFull(b, (left, right) => left + right); 
    // Use pre defined JoinOperation to ignore nulls
    tf["JoinFull JoinOperation.Add"] = a.JoinFull(b, JoinOperation.Add);

    tf

|Date|a|b|apply * 2|JoinLeft r + l|JoinLeft JoinOperation.Add|JoinFull r + l|JoinFull JoinOperation.Add|
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
|01/01/2021 00:00:00 +01:00|1||2||1||1|
|02/01/2021 00:00:00 +01:00|1||2||1||1|
|03/01/2021 00:00:00 +01:00|1|2|2|3|3|3|3|
|04/01/2021 00:00:00 +01:00|1|2|2|3|3|3|3|
|05/01/2021 00:00:00 +01:00|1|2|2|3|3|3|3|
|06/01/2021 00:00:00 +01:00||2|||||2|
|07/01/2021 00:00:00 +01:00||2|||||2|
    
#### Slicing

    var ts = TimeSeries.Factory.FromGenerator(
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 01, 05), // end
        Period.Day,
        tp => tp.Day); // 1, 2, 3, 4, 5

    var tf = new TimeFrame();
    // sliceing by index / count
    tf["ts"] = ts;
    // slicing by index / count
    tf["Slice(0, 2)"] = ts.Slice(0, 2);
    // slicing by time range
    tf["Slice(2.1, 4.1)"] = ts.Slice(new DateTime(2021, 01, 02), new DateTime(2021, 01, 04));

    tf

|Date|ts|Slice(0, 2)|Slice(2.1, 4.1)|
| ------------- | ------------- | ------------- | ------------- |
|01/01/2021 00:00:00 +01:00|1|1||
|02/01/2021 00:00:00 +01:00|2|2|2|
|03/01/2021 00:00:00 +01:00|3||3|
|04/01/2021 00:00:00 +01:00|4||4|
|05/01/2021 00:00:00 +01:00|5|||
	

#### Re-sampling

    var ts = TimeSeries.Factory.FromValue(1,
        new DateTime(2021, 01, 01), // start
        new DateTime(2021, 12, 31), // end
        Period.Day);

    // down sampling
    var tf = new TimeFrame();
    tf["Resample Sum"] = ts.ReSample(Period.Month, AggregationType.Sum);
    tf["Resample Mean"] = ts.ReSample(Period.Month, AggregationType.Mean); 
    tf

|Date|Resample Sum|Resample Mean|
| ------------- | ------------- | ------------- |
|01/01/2021 00:00:00 +01:00|31|1|
|01/02/2021 00:00:00 +01:00|28|1|
|01/03/2021 00:00:00 +01:00|31|1|
|01/04/2021 00:00:00 +02:00|30|1|
|01/05/2021 00:00:00 +02:00|31|1|
|01/06/2021 00:00:00 +02:00|30|1|
|01/07/2021 00:00:00 +02:00|31|1|
|01/08/2021 00:00:00 +02:00|31|1|
|01/09/2021 00:00:00 +02:00|30|1|
|01/10/2021 00:00:00 +02:00|31|1|
|01/11/2021 00:00:00 +01:00|30|1|
|01/12/2021 00:00:00 +01:00|31|1|

    // up-sampling
    var tf = new TimeFrame();
    tf["Resample Hour Sum"] = ts.ReSample(Period.Hour, AggregationType.Sum);
    tf["Resample Hour Mean"] = ts.ReSample(Period.Hour, AggregationType.Mean);
    tf

|Date|Resample Hour Sum|Resample Hour Mean|
| ------------- | ------------- | ------------- |
|01/01/2021 00:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 01:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 02:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 03:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 04:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 05:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 06:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 07:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 08:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 09:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 10:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 11:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 12:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 13:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 14:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 15:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 16:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 17:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 18:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 19:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 20:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 21:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 22:00:00 +01:00|0,0416666666666666666666666667|1|
|01/01/2021 23:00:00 +01:00|0,0416666666666666666666666667|1|
|02/01/2021 00:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 01:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 02:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 03:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 04:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 05:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 06:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 07:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 08:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 09:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 10:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 11:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 12:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 13:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 14:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 15:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 16:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 17:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 18:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 19:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 20:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 21:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 22:00:00 +01:00|0,0833333333333333333333333333|2|
|02/01/2021 23:00:00 +01:00|0,0833333333333333333333333333|2|
|03/01/2021 00:00:00 +01:00|0,125|3|
|03/01/2021 01:00:00 +01:00|0,125|3|
|03/01/2021 02:00:00 +01:00|0,125|3|
|03/01/2021 03:00:00 +01:00|0,125|3|
|03/01/2021 04:00:00 +01:00|0,125|3|
|03/01/2021 05:00:00 +01:00|0,125|3|
|03/01/2021 06:00:00 +01:00|0,125|3|
|03/01/2021 07:00:00 +01:00|0,125|3|
|03/01/2021 08:00:00 +01:00|0,125|3|
|03/01/2021 09:00:00 +01:00|0,125|3|
|03/01/2021 10:00:00 +01:00|0,125|3|
|03/01/2021 11:00:00 +01:00|0,125|3|
|03/01/2021 12:00:00 +01:00|0,125|3|
|03/01/2021 13:00:00 +01:00|0,125|3|
|03/01/2021 14:00:00 +01:00|0,125|3|
|03/01/2021 15:00:00 +01:00|0,125|3|
|03/01/2021 16:00:00 +01:00|0,125|3|
|03/01/2021 17:00:00 +01:00|0,125|3|
|03/01/2021 18:00:00 +01:00|0,125|3|
|03/01/2021 19:00:00 +01:00|0,125|3|
|03/01/2021 20:00:00 +01:00|0,125|3|
|03/01/2021 21:00:00 +01:00|0,125|3|
|03/01/2021 22:00:00 +01:00|0,125|3|
|03/01/2021 23:00:00 +01:00|0,125|3|
|04/01/2021 00:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 01:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 02:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 03:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 04:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 05:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 06:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 07:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 08:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 09:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 10:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 11:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 12:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 13:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 14:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 15:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 16:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 17:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 18:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 19:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 20:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 21:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 22:00:00 +01:00|0,1666666666666666666666666667|4|
|04/01/2021 23:00:00 +01:00|0,1666666666666666666666666667|4|
|05/01/2021 00:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 01:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 02:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 03:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 04:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 05:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 06:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 07:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 08:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 09:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 10:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 11:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 12:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 13:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 14:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 15:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 16:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 17:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 18:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 19:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 20:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 21:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 22:00:00 +01:00|0,2083333333333333333333333333|5|
|05/01/2021 23:00:00 +01:00|0,2083333333333333333333333333|5|


### Usage in LINQPad

[Thinksharp.TimeFlow.LinqPad](https://github.com/JanDotNet/Thinksharp.TimeFlow.LinqPad) provides chart and raw data visualization extensions for LINQPad.

### Usage in .Net Interactive (Jupiter Notebooks)

Reference Nuget **Package Thinksharp.TimeFlow** and **Thinksharp.TimeFlow.Interactive** as well as **XPlot.Plotly** if you want to use plotting abilities.

    #r "nuget: Thinksharp.TimeFlow"
    #r "nuget: Thinksharp.TimeFlow.Interactive"
    #r "nuget: XPlot.Plotly"
    #r "nuget: XPlot.Plotly.Interactive"

## License

ThinkSharp.TimeFlow is released under [The MIT license (MIT)](LICENSE)

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JanDotNet/ThinkSharp.TimeFlow/tags). 
    
   
## Donation

If you like ThinkSharp.TimeFlow and use it in your project(s), feel free to give me a cup of coffee :) 
