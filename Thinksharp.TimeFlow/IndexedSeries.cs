using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Thinksharp.TimeFlow
{
  public abstract class IndexedSeries<TKey, TValue> : IReadOnlyList<IndexedSeriesItem<TKey, TValue>>
  {
    protected readonly IDictionary<TKey, TValue> seriesDictionary = new Dictionary<TKey, TValue>();
    protected readonly IList<IndexedSeriesItem<TKey, TValue>> sortedValues = new List<IndexedSeriesItem<TKey, TValue>>();

    protected IndexedSeries(IEnumerable<IndexedSeriesItem<TKey, TValue>> sortedSeries)
    {
      if (sortedSeries is IList<IndexedSeriesItem<TKey, TValue>> sortedSeriesList)
      {
        this.sortedValues = sortedSeriesList;
      }
      else
      {
        this.sortedValues = sortedSeries.ToList();
      }
      
      this.seriesDictionary = this.sortedValues.ToDictionary(x => x.Key, x => x.Value);

      if (this.sortedValues.Count > 0)
      {
        this.Start = this.sortedValues.First().Key;
        this.End = this.sortedValues.Last().Key;
      }
      else
      {
        this.Start = default(TKey);
        this.End = default(TKey);
      }

      this.IsEmpty = this.sortedValues.Count == 0;
    }

    public TValue this[TKey key] 
    {
      get => this.seriesDictionary.TryGetValue(key, out var value) ? value : default(TValue);
      set => throw new NotSupportedException($"IT is not allowed to change {nameof(IndexedSeries<TKey, TValue>)} because it is an immutable data structure"); 
    }
    public IndexedSeriesItem<TKey, TValue> this[int index] => sortedValues[index];

    public TKey Start { get; }
    public TKey End { get; }
    public bool IsEmpty { get; }
    public int Count => this.sortedValues.Count;

    public IEnumerable<TKey> Keys => this.seriesDictionary.Keys;

    public IEnumerable<TValue> Values => this.seriesDictionary.Values;

    public bool ContainsKey(TKey key) => this.seriesDictionary.ContainsKey(key);

    public IEnumerator<IndexedSeriesItem<TKey, TValue>> GetEnumerator() => this.sortedValues.GetEnumerator();

    public bool TryGetValue(TKey key, out TValue value) => this.seriesDictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => this.sortedValues.GetEnumerator();
  }
}