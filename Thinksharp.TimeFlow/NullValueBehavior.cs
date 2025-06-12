using System;
using System.Diagnostics;

namespace Thinksharp.TimeFlow
{
  public enum SingleValueNullBehavior
  {
    /// <summary>
    /// If one value is null, the whole aggregation value becomes null.
    /// </summary>
    AggregationValueBecomesNull,

    /// <summary>
    /// If one value is null, the whole aggregation value becomes zero (0).
    /// </summary>
    AggregationValueBecomesZero,

    /// <summary>
    /// If one value is null, the single value becomes zero (0) and will be considered for aggregation (only relevant for average, min, max).
    /// </summary>
    SingleValueBecomesZero,

    [Obsolete("Use SingleValueWillBeIgnoredForAggregartion instead.")]
    SingleValueBecomesWillBeIgnoredForAggregartion,

    /// <summary>
    /// If one value is null, the single value will be ignored for aggregation (only relevant for average, min, max).
    /// </summary>
    SingleValueWillBeIgnoredForAggregation
  }
}