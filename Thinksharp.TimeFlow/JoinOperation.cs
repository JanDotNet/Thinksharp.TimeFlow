namespace Thinksharp.TimeFlow
{
  using System;

  public class JoinOperation
  {
    private readonly Func<decimal?, decimal?, decimal?> applyFunction;

    private JoinOperation(Func<decimal?, decimal?, decimal?> applyFunction)
    {
      this.applyFunction = applyFunction;
    }

    /// <summary>
    ///   Adds the right to the left value. Null values results in null.
    /// </summary>
    public static JoinOperation AddNullResultsInNull { get; } = new JoinOperation((l, r) => l + r);

    /// <summary>
    ///   Adds the right to the left value. Null values will be replaced with 0.
    /// </summary>
    public static JoinOperation Add { get; } = new JoinOperation((l, r) => (l ?? 0) + (r ?? 0));

    /// <summary>
    ///   Subtracts the left from the right value. Null values results in null (null - 1 => null).
    /// </summary>
    public static JoinOperation SubtractNullResultsInNull { get; } = new JoinOperation((l, r) => l - r);

    /// <summary>
    ///   Subtracts the left from the right value. Null values will be replaced with 0 (null - 1 => -1).
    /// </summary>
    public static JoinOperation Subtract { get; } = new JoinOperation((l, r) => (l ?? 0) - (r ?? 0));

    /// <summary>
    ///   Subtracts the left from the right value. Null values results in null (null * 1 => null).
    /// </summary>
    public static JoinOperation MultiplyNullResultsInNull { get; } = new JoinOperation((l, r) => l * r);

    /// <summary>
    ///   Subtracts the left from the right value. Null values will be replaced with 0 (null * 1 => 0).
    /// </summary>
    public static JoinOperation Multiply { get; } = new JoinOperation((l, r) => (l ?? 0) * (r ?? 0));

    /// <summary>
    ///   Subtracts the left from the right value. Null values results in null (null / 1 => null).
    /// </summary>
    public static JoinOperation DivideNullResultsInNull { get; } = new JoinOperation((l, r) => r == null || r == 0M ? (decimal?) null : l / r);

    /// <summary>
    ///   Subtracts the left from the right value. Null values will be replaced with 0 (null / r => 0; 1 / null => null).
    /// </summary>
    public static JoinOperation Divide { get; } = new JoinOperation((l, r) => r == null || r == 0M ? (decimal?) null : (l ?? 0) / r);

    /// <summary>
    ///   Takes always the right value.
    /// </summary>
    public static JoinOperation TakeRight { get; } = new JoinOperation((l, r) => r);

    public decimal? Apply(decimal? left, decimal? right)
    {
      if (left == null && right == null)
      {
        return null;
      }

      return this.applyFunction(left, right);
    }
  }
}