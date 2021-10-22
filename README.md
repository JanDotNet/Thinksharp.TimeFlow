# Thinksharp.TimeFlow

## Introduction

TimeFlow is a lightweight extendable library for working with time series with support for time zones and daylight saving. The core of the library is the **TimeSeries** type, which holds a list of time point / value pairs. Time points are represented as **DateTimeOffset**s. The time span between two time points (called frequency) is always identical for the whole time series. Values are represented as **nullable decimal**s.

The first and the last time point of a TimeSeries are exposed by the **Start** and **End** properties. The interval between time points is exposed as **Frequency** property.

TimeSeries objects are **immutable**. There are some methods for transforming TimeSeries, like **ReSample**, **Slice** or **Apply**.Each of these methods return a new immutable object.

A collection of named time series with the same frequency can be combined to one **TimeFrame**.

## Usage

Usage is explained in the [jupiter notebook](Notebooks\timeseries.ipynb).

## Modules



