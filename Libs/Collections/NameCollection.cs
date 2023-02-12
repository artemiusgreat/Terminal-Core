using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Terminal.Core.EnumSpace;
using Terminal.Core.MessageSpace;
using Terminal.Core.ModelSpace;
using static System.Collections.Specialized.BitVector32;

namespace Terminal.Core.CollectionSpace
{
  /// <summary>
  /// Name based collection
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public interface INameCollection<TKey, TValue> : IDictionary<TKey, TValue> where TValue : IBaseModel
  {
    /// <summary>
    /// Observable item changes
    /// </summary>
    Action<ITransactionMessage<TValue>> ItemStream { get; set; }

    /// <summary>
    /// Observable items changes
    /// </summary>
    Action<ITransactionMessage<IDictionary<TKey, TValue>>> ItemsStream { get; set; }
  }

  /// <summary>
  /// Name based collection
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class NameCollection<TKey, TValue> : INameCollection<TKey, TValue> where TValue : IBaseModel
  {
    /// <summary>
    /// Internal collection
    /// </summary>
    public virtual IDictionary<TKey, TValue> Items { get; protected set; }

    /// <summary>
    /// Observable item changes
    /// </summary>
    public virtual Action<ITransactionMessage<TValue>> ItemStream { get; set; }

    /// <summary>
    /// Observable items changes
    /// </summary>
    public virtual Action<ITransactionMessage<IDictionary<TKey, TValue>>> ItemsStream { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public NameCollection()
    {
      Items = new ConcurrentDictionary<TKey, TValue>();
      ItemStream = o => { };
      ItemsStream = o => { };
    }

    /// <summary>
    /// Standard dictionary implementation
    /// </summary>
    public virtual int Count => Items.Count;
    public virtual bool IsReadOnly => Items.IsReadOnly;
    public virtual bool Contains(KeyValuePair<TKey, TValue> item) => Items.Contains(item);
    public virtual bool ContainsKey(TKey key) => Items.ContainsKey(key);
    public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
    public virtual bool TryGetValue(TKey key, out TValue value) => Items.TryGetValue(key, out value);
    public virtual ICollection<TKey> Keys => Items.Keys;
    public virtual ICollection<TValue> Values => Items.Values;
    public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    /// <summary>
    /// Get item by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual TValue this[TKey index]
    {
      get => TryGetValue(index, out TValue item) ? item : default;
      set => Add(index, value);
    }

    /// <summary>
    /// Add a pair to the dictionary
    /// </summary>
    /// <param name="item"></param>
    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
      Add(item.Key, item.Value);
    }

    /// <summary>
    /// Add item using specific index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public virtual void Add(TKey index, TValue item)
    {
      var previous = this[index];
      var action = previous is null ? ActionEnum.Create : ActionEnum.Update;

      Items[index] = item;

      SendItemMessage(item, previous, action);
      SendItemsMessage(action);
    }

    /// <summary>
    /// Clear collection
    /// </summary>
    public virtual void Clear()
    {
      Items.Clear();
      SendItemMessage(default, default, ActionEnum.Delete);
      SendItemsMessage(ActionEnum.Delete);
    }

    /// <summary>
    /// Remove item by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual bool Remove(TKey index)
    {
      if (Items.ContainsKey(index) is false)
      {
        return false;
      }

      var previous = Items[index];
      var response = Items.Remove(index);

      SendItemMessage(default, previous, ActionEnum.Delete);
      SendItemsMessage(ActionEnum.Delete);

      return response;
    }

    /// <summary>
    /// Remove a pair from the collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    /// <summary>
    /// Send item message
    /// </summary>
    /// <param name="next"></param>
    /// <param name="previous"></param>
    /// <param name="action"></param>
    protected virtual void SendItemMessage(TValue next, TValue previous, ActionEnum action)
    {
      var itemMessage = new TransactionMessage<TValue>
      {
        Next = next,
        Previous = previous,
        Action = action
      };

      ItemStream(itemMessage);
    }

    /// <summary>
    /// Send collection message
    /// </summary>
    /// <param name="action"></param>
    protected virtual void SendItemsMessage(ActionEnum action)
    {
      var collectionMessage = new TransactionMessage<IDictionary<TKey, TValue>>
      {
        Next = Items,
        Action = action
      };

      ItemsStream(collectionMessage);
    }
  }
}
