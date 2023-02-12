using Terminal.Core.EnumSpace;

namespace Terminal.Core.MessageSpace
{
  public class TransactionMessage<T>
  {
    /// <summary>
    /// Event type
    /// </summary>
    public virtual ActionEnum Action { get; set; }

    /// <summary>
    /// Current or next value to be set
    /// </summary>
    public virtual T Next { get; set; }

    /// <summary>
    /// Previous value
    /// </summary>
    public virtual T Previous { get; set; }
  }
}
