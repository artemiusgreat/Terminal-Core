namespace Terminal.Core.ModelSpace
{
  public class BarModel : BaseModel
  {
    /// <summary>
    /// Lowest price of the bar
    /// </summary>
    public virtual double? Low { get; set; }

    /// <summary>
    /// Highest price of the bar
    /// </summary>
    public virtual double? High { get; set; }

    /// <summary>
    /// Open price of the bar
    /// </summary>
    public virtual double? Open { get; set; }

    /// <summary>
    /// Close price of the bar
    /// </summary>
    public virtual double? Close { get; set; }
  }
}
