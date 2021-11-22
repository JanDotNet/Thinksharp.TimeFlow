using System;
using System.Diagnostics;

namespace Thinksharp.TimeFlow
{
  [DebuggerDisplay("{Key} -> {Value}")]
  public class IndexedSeriesItem<TIndex, TValue>
  {
    public IndexedSeriesItem(TIndex key, TValue value)
    {
      this.Key = key;
      this.Value = value;
    }
    public TIndex Key { get; set; }

    public TValue Value { get; set; }
  }
}
