using System;
using System.Collections.Generic;
using Terminal.Core.EnumSpace;
using Terminal.Core.MessageSpace;

namespace Terminal.Core.ModelSpace
{
  /// <summary>
  /// Generic order model
  /// </summary>
  public interface ITransactionOrderModel : ITransactionModel
  {
    /// <summary>
    /// Side
    /// </summary>
    OrderSideEnum? Side { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    OrderTypeEnum? Type { get; set; }

    /// <summary>
    /// Time in force
    /// </summary>
    OrderTimeSpanEnum? TimeSpan { get; set; }

    /// <summary>
    /// List of related orders in the hierarchy
    /// </summary>
    IList<ITransactionOrderModel> Orders { get; set; }

    /// <summary>
    /// Order events
    /// </summary>
    Action<TransactionMessage<ITransactionOrderModel>> OrderStream { get; set; }
  }

  /// <summary>
  /// Generic order model
  /// </summary>
  public class OrderModel : TransactionModel, ITransactionOrderModel
  {
    /// <summary>
    /// Side
    /// </summary>
    public virtual OrderSideEnum? Side { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public virtual OrderTypeEnum? Type { get; set; }

    /// <summary>
    /// Time in force
    /// </summary>
    public virtual OrderTimeSpanEnum? TimeSpan { get; set; }

    /// <summary>
    /// List of related orders in the hierarchy
    /// </summary>
    public virtual IList<ITransactionOrderModel> Orders { get; set; }

    /// <summary>
    /// Order events
    /// </summary>
    public virtual Action<TransactionMessage<ITransactionOrderModel>> OrderStream { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public OrderModel()
    {
      Orders = new List<ITransactionOrderModel>();
      OrderStream = o => { };
    }
  }
}
