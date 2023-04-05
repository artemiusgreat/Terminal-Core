using System;
using System.Collections.Generic;

namespace Terminal.Core.ModelSpace
{
  /// <summary>
  /// Definition
  /// </summary>
  public interface IPointModel : IBaseModel
  {
    /// <summary>
    /// Bid
    /// </summary>
    double? Bid { get; set; }

    /// <summary>
    /// Ask
    /// </summary>
    double? Ask { get; set; }

    /// <summary>
    /// Size of the bid on the current tick
    /// </summary>
    double? BidSize { get; set; }

    /// <summary>
    /// Size of the ask on the current tick
    /// </summary>
    double? AskSize { get; set; }

    /// <summary>
    /// Last price or value
    /// </summary>
    double? Last { get; set; }

    /// <summary>
    /// Time stamp
    /// </summary>
    DateTime? Time { get; set; }

    /// <summary>
    /// Aggregation period for the quotes
    /// </summary>
    TimeSpan? TimeFrame { get; set; }

    /// <summary>
    /// Reference to the complex data point
    /// </summary>
    BarModel Bar { get; set; }

    /// <summary>
    /// Reference to the account
    /// </summary>
    IAccountModel Account { get; set; }

    /// <summary>
    /// Reference to the instrument
    /// </summary>
    IInstrumentModel Instrument { get; set; }

    /// <summary>
    /// Values from related series synced with the current bar, e.g. averaged indicator calculations for the charts
    /// </summary>
    IDictionary<string, IPointModel> Series { get; set; }
  }

  /// <summary>
  /// Implementation
  /// </summary>
  public class PointModel : BaseModel, IPointModel
  {
    /// <summary>
    /// Bid
    /// </summary>
    public virtual double? Bid { get; set; }

    /// <summary>
    /// Ask
    /// </summary>
    public virtual double? Ask { get; set; }

    /// <summary>
    /// Volume of the bid 
    /// </summary>
    public virtual double? BidSize { get; set; }

    /// <summary>
    /// Volume of the ask
    /// </summary>
    public virtual double? AskSize { get; set; }

    /// <summary>
    /// Last price or value
    /// </summary>
    public virtual double? Last { get; set; }

    /// <summary>
    /// Time stamp
    /// </summary>
    public virtual DateTime? Time { get; set; }

    /// <summary>
    /// Aggregation period for the quotes
    /// </summary>
    public virtual TimeSpan? TimeFrame { get; set; }

    /// <summary>
    /// Reference to the account
    /// </summary>
    public virtual IAccountModel Account { get; set; }

    /// <summary>
    /// Reference to the complex data point
    /// </summary>
    public virtual BarModel Bar { get; set; }

    /// <summary>
    /// Reference to the instrument
    /// </summary>
    public virtual IInstrumentModel Instrument { get; set; }

    /// <summary>
    /// Values from related series synced with the current data point, e.g. averaged indicator calculations for the charts
    /// </summary>
    public virtual IDictionary<string, IPointModel> Series { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public PointModel()
    {
      Time = DateTime.Now;

      Bar = new BarModel();
      Account = new AccountModel();
      Instrument = new InstrumentModel();
      Series = new Dictionary<string, IPointModel>();
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
      var clone = base.Clone() as IPointModel;

      clone.Bar = Bar.Clone() as BarModel;

      return clone;
    }
  }
}
