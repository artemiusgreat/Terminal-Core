using System;
using Terminal.Core.Domains;
using Terminal.Core.Enums;

namespace Terminal.Core.Models
{
    public class OptionModel : ICloneable
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// The name of the underlying instrument
    /// </summary>
    public virtual string BaseName { get; set; }

    /// <summary>
    /// Strike price
    /// </summary>
    public virtual double? Strike { get; set; }

    /// <summary>
    /// Contract size
    /// </summary>
    public virtual double? Leverage { get; set; }

    /// <summary>
    /// Open interest
    /// </summary>
    public virtual double? OpenInterest { get; set; }

    /// <summary>
    /// CALL or PUT
    /// </summary>
    public virtual OptionSideEnum? Side { get; set; }

    /// <summary>
    /// Expiration date
    /// </summary>
    public virtual DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Instrument
    /// </summary>
    public virtual IInstrument Instrument { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public OptionModel()
    {
      Leverage = 100;
    }

    /// <summary>
    /// Clone
    /// </summary>
    public virtual object Clone() => MemberwiseClone() as OptionModel;
  }
}
