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
    /// If one value is null, the single value becomes zero (0) and will be considered for aggregation (only relevant for average).
    /// </summary>
    SingleValueBecomesZero,

    /// <summary>
    /// If one value is null, the single value will be ignored for aggregation (only relevant for average).
    /// </summary>
    SingleValueBecomesWillBeIgnoredForAggregartion,
  }
}