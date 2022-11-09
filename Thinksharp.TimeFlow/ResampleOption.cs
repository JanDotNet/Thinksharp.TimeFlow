using System;
using System.Collections.Generic;
using System.Text;

namespace Thinksharp.TimeFlow
{
  public class ResampleOption
  {
    /// <summary>
    /// Gets or sets the resample type.
    /// </summary>
    public ResampleType ResampleType { get; set; } = ResampleType.Absolut;

    /// <summary>
    /// Gets or sets the behavior for single values within an list of values that are resampled.
    /// </summary>
    public SingleValueNullBehavior SingleValueIsNull { get; set; } = SingleValueNullBehavior.AggregationValueBecomesZero;
  } 
}
