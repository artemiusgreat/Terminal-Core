using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Core.Services;
using Terminal.Core.Validators;

namespace Terminal.Core.Domains
{
  public interface IConnector : IDisposable
  {
    /// <summary>
    /// Production or Development mode
    /// </summary>
    EnvironmentEnum Mode { get; set; }

    /// <summary>
    /// Account
    /// </summary>
    IAccount Account { get; set; }

    /// <summary>
    /// Incoming data event
    /// </summary>
    Action<StateModel<PointModel>> DataStream { get; set; }

    /// <summary>
    /// Send order event
    /// </summary>
    Action<StateModel<OrderModel>> OrderStream { get; set; }

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
    Task<PointModel> GetPoint(PointQueryModel message);

    /// <summary>
    /// Get quotes history
    /// </summary>
    /// <param name="message"></param>
    Task<IList<PointModel>> GetPoints(PointQueryModel message);

    /// <summary>
    /// Get option chains
    /// </summary>
    /// <param name="message"></param>
    Task<IList<OptionModel>> GetOptions(OptionQueryModel message);

    /// <summary>
    /// Send new orders
    /// </summary>
    /// <param name="orders"></param>
    Task<ResponseModel<OrderModel, IList<ValidationFailure>>> CreateOrders(params OrderModel[] orders);

    /// <summary>
    /// Update orders
    /// </summary>
    /// <param name="orders"></param>
    Task<ResponseModel<OrderModel, IList<ValidationFailure>>> UpdateOrders(params OrderModel[] orders);

    /// <summary>
    /// Cancel orders
    /// </summary>
    /// <param name="orders"></param>
    Task<ResponseModel<OrderModel, IList<ValidationFailure>>> DeleteOrders(params OrderModel[] orders);
  }

  /// <summary>
  /// Implementation
  /// </summary>
  public abstract class Connector : IConnector
  {
    /// <summary>
    /// Production or Sandbox
    /// </summary>
    public virtual EnvironmentEnum Mode { get; set; }

    /// <summary>
    /// Account
    /// </summary>
    public virtual IAccount Account { get; set; }

    /// <summary>
    /// Incoming data event
    /// </summary>
    public virtual Action<StateModel<PointModel>> DataStream { get; set; }

    /// <summary>
    /// Send order event
    /// </summary>
    public virtual Action<StateModel<OrderModel>> OrderStream { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Connector()
    {
      Mode = EnvironmentEnum.Paper;

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
    public virtual Task<PointModel> GetPoint(PointQueryModel message)
    {
      return Task.FromResult(null as PointModel);
    }

    /// <summary>
    /// Get quotes history
    /// </summary>
    /// <param name="message"></param>
    public virtual Task<IList<PointModel>> GetPoints(PointQueryModel message)
    {
      return Task.FromResult(Array.Empty<PointModel>() as IList<PointModel>);
    }

    /// <summary>
    /// Get option chains
    /// </summary>
    /// <param name="message"></param>
    public virtual Task<IList<OptionModel>> GetOptions(OptionQueryModel message)
    {
      return Task.FromResult(Array.Empty<OptionModel>() as IList<OptionModel>);
    }

    /// <summary>
    /// Send new orders
    /// </summary>
    /// <param name="orders"></param>
    public virtual Task<ResponseModel<OrderModel, IList<ValidationFailure>>> CreateOrders(params OrderModel[] orders)
    {
      return Task.FromResult(new ResponseModel<OrderModel, IList<ValidationFailure>>());
    }

    /// <summary>
    /// Update orders
    /// </summary>
    /// <param name="orders"></param>
    public virtual Task<ResponseModel<OrderModel, IList<ValidationFailure>>> UpdateOrders(params OrderModel[] orders)
    {
      return Task.FromResult(new ResponseModel<OrderModel, IList<ValidationFailure>>());
    }

    /// <summary>
    /// Cancel orders
    /// </summary>
    /// <param name="orders"></param>
    public virtual Task<ResponseModel<OrderModel, IList<ValidationFailure>>> DeleteOrders(params OrderModel[] orders)
    {
      return Task.FromResult(new ResponseModel<OrderModel, IList<ValidationFailure>>());
    }

    /// <summary>
    /// Set missing order properties
    /// </summary>
    /// <param name="orders"></param>
    protected virtual IList<OrderModel> CorrectOrders(params OrderModel[] orders)
    {
      foreach (var nextOrder in orders)
      {
        nextOrder.Type ??= OrderTypeEnum.Market;
        nextOrder.TimeSpan ??= OrderTimeSpanEnum.GTC;
        nextOrder.Transaction ??= new TransactionModel();
        nextOrder.Transaction.Time ??= DateTime.Now;
        nextOrder.Transaction.Price ??= GetOpenPrice(nextOrder);
        nextOrder.Transaction.Status ??= OrderStatusEnum.None;
        nextOrder.Transaction.Operation ??= OperationEnum.In;
      }

      return orders;
    }

    /// <summary>
    /// Ensure all properties have correct values
    /// </summary>
    /// <param name="orders"></param>
    protected virtual ResponseModel<OrderModel, IList<ValidationFailure>> ValidateOrders(params OrderModel[] orders)
    {
      var orderRules = InstanceService<OrderPriceValidator>.Instance;
      var response = new ResponseModel<OrderModel, IList<ValidationFailure>>();

      foreach (var order in orders)
      {
        var errors = new List<ValidationFailure>();

        errors.AddRange(orderRules.Validate(order).Errors);
        errors.AddRange(order.Orders.SelectMany(o => orderRules.Validate(o).Errors));

        response.Count += errors.Count;
        response.Items.Add((order, errors));
      }

      return response;
    }

    /// <summary>
    /// Define open price based on order
    /// </summary>
    /// <param name="nextOrder"></param>
    protected virtual double? GetOpenPrice(OrderModel nextOrder)
    {
      var pointModel = nextOrder?.Transaction?.Instrument?.Points?.LastOrDefault();

      if (pointModel is not null)
      {
        switch (nextOrder?.Side)
        {
          case OrderSideEnum.Buy: return pointModel.Ask;
          case OrderSideEnum.Sell: return pointModel.Bid;
        }
      }

      return null;
    }

    /// <summary>
    /// Update points
    /// </summary>
    /// <param name="point"></param>
    protected virtual IList<IAccount> CorrectAccounts(params IAccount[] accounts)
    {
      foreach (var account in accounts)
      {
        account.InitialBalance = account.Balance;
      }

      return accounts;
    }

    /// <summary>
    /// Update points
    /// </summary>
    /// <param name="point"></param>
    protected virtual IList<PointModel> CorrectPoints(params PointModel[] points)
    {
      foreach (var point in points)
      {
        var instrument = Account.Instruments[point.Instrument.Name];
        var estimates = Account.ActivePositions.Select(o => o.Value.GainLossEstimate).ToList();

        point.Instrument = instrument;
        point.TimeFrame = instrument.TimeFrame;

        instrument.Points.Add(point);
        instrument.PointGroups.Add(point, instrument.TimeFrame, true);
      }

      return points;
    }
  }
}
