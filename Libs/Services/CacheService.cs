using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Terminal.Core.Services
{
  /// <summary>
  /// Cache
  /// </summary>
  public class CacheService<TKey, TValue>
  {
    /// <summary>
    /// Logger instance
    /// </summary>
    public virtual IDictionary<TKey, TValue> Cache { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public CacheService() => Cache = new ConcurrentDictionary<TKey, TValue>();
  }
}
