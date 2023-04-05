using System;
using Terminal.Core.EnumSpace;

namespace Terminal.Core.ModelSpace
{
  public class OptionModel
  {
    /// <summary>
    /// Contract size
    /// </summary>
    public virtual double? Leverage { get; set; }

    /// <summary>
    /// Open interest
    /// </summary>
    public virtual double? OpenInterest { get; set; }

    /// <summary>
    /// Strike price
    /// </summary>
    public virtual double? Strike { get; set; }

    /// <summary>
    /// The name of the underlying instrument
    /// </summary>
    public virtual string Symbol { get; set; }

    /// <summary>
    /// CALL or PUT
    /// </summary>
    public virtual OptionSideEnum? Side { get; set; }

    /// <summary>
    /// Expiration date
    /// </summary>
    public virtual DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Reference to the complex data point
    /// </summary>
    public virtual BarModel Bar { get; set; }

    /// <summary>
    /// Instrument
    /// </summary>
    public virtual IInstrumentModel Instrument { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public OptionModel()
    {
      Leverage = 100;
      Bar = new BarModel();
    }
  }
}
