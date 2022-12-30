using System;
using System.Collections.Generic;
using System.Text;

namespace Thinksharp.TimeFlow
{
  public class TimeSeriesIntervall
  {
    public TimeSeriesIntervall(decimal? value, DateTimeOffset start, DateTimeOffset end)
    {
      Start = start;
      End = end;
      Value = value;
    }
    public decimal? Value { get; }
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
  }
}
