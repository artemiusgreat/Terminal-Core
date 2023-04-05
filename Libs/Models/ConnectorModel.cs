using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Core.EnumSpace;
using Terminal.Core.MessageSpace;
using Terminal.Core.ServiceSpace;
using Terminal.Core.ValidatorSpace;

namespace Terminal.Core.ModelSpace
{
  /// <summary>
  /// Interface that defines input and output processes
  /// </summary>
  public interface IConnectorModel : IBaseModel, IDisposable
  {
    /// <summary>
    /// Production or Development mode
    /// </summary>
    EnvironmentEnum Mode { get; set; }

    /// <summary>
    /// Account
    /// </summary>
    IAccountModel Account { get; set; }

    /// <summary>
    /// Incoming data event
    /// </summary>
    Action<TransactionMessage<IPointModel>> DataStream { get; set; }

    /// <summary>
    /// Send order event
    /// </summary>
    Action<TransactionMessage<ITransactionOrderModel>> OrderStream { get; set; }

    /// <summary>
    /// Restore state and initialize
    /// </summary>
    Task Connect();

    /// <summary>
    /// Save state and dispose
    /// </summary>
    Task Disconnect();

    /// <summary>
    /// Continue execution
    /// </summary>
    Task Subscribe();

    /// <summary>
    /// Suspend execution
    /// </summary>
    Task Unsubscribe();

    /// <summary>
    /// Get quote
    /// </summary>
    /// <param name="message"></param>
    Task<IPointModel> GetPoint(PointMessage message);

    /// <summary>
    /// Get quotes history
    /// </summary>
    /// <param name="message"></param>
    Task<IList<IPointModel>> GetPoints(PointMessage message);

    /// <summary>
    /// Get option chains
    /// </summary>
    /// <param name="message"></param>
    Task<IList<IPointModel>> GetOptions(OptionMessage message);

    /// <summary>
    /// Send new orders
    /// </summary>
    /// <param name="orders"></param>
    Task<IList<ITransactionOrderModel>> CreateOrders(params ITransactionOrderModel[] orders);

    /// <summary>
    /// Update orders
    /// </summary>
    /// <param name="orders"></param>
    Task<IList<ITransactionOrderModel>> UpdateOrders(params ITransactionOrderModel[] orders);

    /// <summary>
    /// Cancel orders
    /// </summary>
    /// <param name="orders"></param>
    Task<IList<ITransactionOrderModel>> DeleteOrders(params ITransactionOrderModel[] orders);
  }

  /// <summary>
  /// Implementation
  /// </summary>
  public abstract class ConnectorModel : BaseModel, IConnectorModel
  {
    /// <summary>
    /// Production or Sandbox
    /// </summary>
    public virtual EnvironmentEnum Mode { get; set; }

    /// <summary>
    /// Account
    /// </summary>
    public virtual IAccountModel Account { get; set; }

    /// <summary>
    /// Incoming data event
    /// </summary>
    public virtual Action<TransactionMessage<IPointModel>> DataStream { get; set; }

    /// <summary>
    /// Send order event
    /// </summary>
    public virtual Action<TransactionMessage<ITransactionOrderModel>> OrderStream { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ConnectorModel()
    {
      Mode = EnvironmentEnum.Paper;

      Account = new AccountModel();

      DataStream = o => { };
      OrderStream = o => { };
    }

    /// <summary>
    /// Restore state and initialize
    /// </summary>
    public abstract Task Connect();

    /// <summary>
    /// Continue execution
    /// </summary>
    public abstract Task Subscribe();

    /// <summary>
    /// Save state and dispose
    /// </summary>
    public abstract Task Disconnect();

    /// <summary>
    /// Unsubscribe from data streams
    /// </summary>
    public abstract Task Unsubscribe();

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose() => Disconnect();

    /// <summary>
    /// Get quote
    /// </summary>
    /// <param name="message"></param>
    public virtual Task<IPointModel> GetPoint(PointMessage message)
    {
      return Task.FromResult(null as IPointModel);
    }

    /// <summary>
    /// Get quotes history
    /// </summary>
    /// <param name="message"></param>
    public virtual Task<IList<IPointModel>> GetPoints(PointMessage message)
    {
      return Task.FromResult(null as IList<IPointModel>);
    }

    /// <summary>
    /// Get option chains
    /// </summary>
    /// <param name="message"></param>
    public virtual Task<IList<IPointModel>> GetOptions(OptionMessage message)
    {
      return Task.FromResult(null as IList<IPointModel>);
    }

    /// <summary>
    /// Send new orders
    /// </summary>
    /// <param name="orders"></param>
    public virtual Task<IList<ITransactionOrderModel>> CreateOrders(params ITransactionOrderModel[] orders)
    {
      return Task.FromResult(null as IList<ITransactionOrderModel>);
    }

    /// <summary>
    /// Update orders
    /// </summary>
    /// <param name="orders"></param>
    public virtual Task<IList<ITransactionOrderModel>> UpdateOrders(params ITransactionOrderModel[] orders)
    {
      return Task.FromResult(null as IList<ITransactionOrderModel>);
    }

    /// <summary>
    /// Cancel orders
    /// </summary>
    /// <param name="orders"></param>
    public virtual Task<IList<ITransactionOrderModel>> DeleteOrders(params ITransactionOrderModel[] orders)
    {
      return Task.FromResult(null as IList<ITransactionOrderModel>);
    }

    /// <summary>
    /// Ensure all properties have correct values
    /// </summary>
    /// <param name="orders"></param>
    protected virtual IList<ValidationFailure> ValidateOrders(params ITransactionOrderModel[] orders)
    {
      var errors = new List<ValidationFailure>();
      var orderRules = InstanceService<TransactionOrderPriceValidator>.Instance;
      var instrumentRules = InstanceService<InstrumentCollectionValidator>.Instance;

      foreach (var order in orders)
      {
        errors.AddRange(orderRules.Validate(order).Errors);
        errors.AddRange(instrumentRules.Validate(order.Instrument).Errors);
        errors.AddRange(order.Orders.SelectMany(o => orderRules.Validate(o).Errors));
        errors.AddRange(order.Orders.SelectMany(o => instrumentRules.Validate(o.Instrument).Errors));
      }

      foreach (var error in errors)
      {
        InstanceService<LogService>.Instance.Log.Error(error.ErrorMessage);
      }

      return errors;
    }

    /// <summary>
    /// Define open price based on order
    /// </summary>
    /// <param name="nextOrder"></param>
    protected virtual IList<ITransactionOrderModel> GetOpenPrices(ITransactionOrderModel nextOrder)
    {
      var openPrice = nextOrder.Price;
      var pointModel = nextOrder.Instrument.Points.Last();

      if (openPrice is null)
      {
        switch (nextOrder.Side)
        {
          case OrderSideEnum.Buy: openPrice = pointModel.Ask; break;
          case OrderSideEnum.Sell: openPrice = pointModel.Bid; break;
        }
      }

      return new List<ITransactionOrderModel>
      {
        new OrderModel
        {
          Price = openPrice,
          Volume = nextOrder.Volume,
          Time = nextOrder.Time
        }
      };
    }

    /// <summary>
    /// Update points
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    protected virtual IPointModel UpdatePoints(IPointModel point)
    {
      var instrument = Account.Instruments[point.Name];
      var estimates = Account.ActivePositions.Select(o => o.Value.GainLossEstimate).ToList();

      point.Account = Account;
      point.Instrument = instrument;
      point.TimeFrame = instrument.TimeFrame;

      instrument.Points.Add(point);
      instrument.PointGroups.Add(point, instrument.TimeFrame, true);

      return point;
    }
  }
}
