using System;

namespace Terminal.Core.ModelSpace
{
  public class FutureModel
  {
    /// <summary>
    /// Expiration date
    /// </summary>
    public virtual DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Instrument
    /// </summary>
    public virtual IInstrumentModel Instrument { get; set; }
  }
}
