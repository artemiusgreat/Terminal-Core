using System.Collections.Generic;

namespace Terminal.Core.Models
{
  public class ResponseModel<TK, TV>
  {
    /// <summary>
    /// Errors count
    /// </summary>
    public virtual int Count { get; set; }

    /// <summary>
    /// Current or next value to be set
    /// </summary>
    public virtual IList<(TK, TV)> Items { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ResponseModel() => Items = new List<(TK, TV)>();
  }
}
