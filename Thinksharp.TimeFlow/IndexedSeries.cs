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
    
    private bool allowMutation = false;

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
        _start = this.sortedValues.First().Key;
        _end = this.sortedValues.Last().Key;
      }
      else
      {
        _start = default(TKey);
        _end = default(TKey);
      }

      _isEmpty = this.sortedValues.Count == 0;
    }

    public TValue this[TKey key] 
    {
      get => this.seriesDictionary.TryGetValue(key, out var value) ? value : default(TValue);
      set 
      {
        if (!allowMutation)
          throw new NotSupportedException($"IT is not allowed to change {nameof(IndexedSeries<TKey, TValue>)} because it is an immutable data structure"); 
        
        this.seriesDictionary[key] = value;
        UpdateSortedValues();
      }
    }
    public IndexedSeriesItem<TKey, TValue> this[int index] => sortedValues[index];

    public TKey Start => _start;
    public TKey End => _end;
    public bool IsEmpty => _isEmpty;
    public int Count => this.sortedValues.Count;

    public IEnumerable<TKey> Keys => this.seriesDictionary.Keys;

    public IEnumerable<TValue> Values => this.seriesDictionary.Values;

    public bool ContainsKey(TKey key) => this.seriesDictionary.ContainsKey(key);

    public IEnumerator<IndexedSeriesItem<TKey, TValue>> GetEnumerator() => this.sortedValues.GetEnumerator();

    public bool TryGetValue(TKey key, out TValue value) => this.seriesDictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => this.sortedValues.GetEnumerator();
    
    /// <summary>
    /// Enables mutation for in-place operations. Should only be used by derived classes during controlled mutations.
    /// </summary>
    protected void EnableMutation()
    {
      allowMutation = true;
    }
    
    /// <summary>
    /// Disables mutation after in-place operations complete.
    /// </summary>
    protected void DisableMutation()
    {
      allowMutation = false;
    }
    
    /// <summary>
    /// Updates the internal data structures during in-place operations.
    /// </summary>
    protected void UpdateInPlace(IEnumerable<IndexedSeriesItem<TKey, TValue>> newSortedSeries)
    {
      if (!allowMutation)
        throw new InvalidOperationException("Mutation is not enabled. Call EnableMutation() first.");
        
      this.sortedValues.Clear();
      this.seriesDictionary.Clear();
      
      if (newSortedSeries is IList<IndexedSeriesItem<TKey, TValue>> sortedSeriesList)
      {
        foreach (var item in sortedSeriesList)
        {
          this.sortedValues.Add(item);
        }
      }
      else
      {
        foreach (var item in newSortedSeries)
        {
          this.sortedValues.Add(item);
        }
      }
      
      foreach (var item in this.sortedValues)
      {
        this.seriesDictionary[item.Key] = item.Value;
      }

      UpdateProperties();
    }
    
    /// <summary>
    /// Updates the sorted values from the dictionary during in-place operations.
    /// </summary>
    private void UpdateSortedValues()
    {
      this.sortedValues.Clear();
      foreach (var kvp in this.seriesDictionary.OrderBy(x => x.Key))
      {
        this.sortedValues.Add(new IndexedSeriesItem<TKey, TValue>(kvp.Key, kvp.Value));
      }
      UpdateProperties();
    }
    
    /// <summary>
    /// Updates Start, End, and IsEmpty properties.
    /// </summary>
    private void UpdateProperties()
    {
      if (this.sortedValues.Count > 0)
      {
        SetStart(this.sortedValues.First().Key);
        SetEnd(this.sortedValues.Last().Key);
      }
      else
      {
        SetStart(default(TKey));
        SetEnd(default(TKey));
      }

      SetIsEmpty(this.sortedValues.Count == 0);
    }
    
    // These methods allow updating the readonly properties during in-place operations
    private void SetStart(TKey value) => SetProperty(ref _start, value);
    private void SetEnd(TKey value) => SetProperty(ref _end, value);
    private void SetIsEmpty(bool value) => SetProperty(ref _isEmpty, value);
    
    private void SetProperty<T>(ref T field, T value)
    {
      field = value;
    }
    
    private TKey _start;
    private TKey _end;
    private bool _isEmpty;
  }
}