using System;
using System.Collections.ObjectModel;

namespace Terminal.Core.ModelSpace
{
  /// <summary>
  /// Definition
  /// </summary>
  public interface IIndicator<TInput, TOutput> : IPointModel where TInput : IPointModel
  {
    /// <summary>
    /// Calculate indicator values
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    TOutput Calculate(ObservableCollection<TInput> collection);
  }

  /// <summary>
  /// Implementation
  /// </summary>
  public class IndicatorModel<TInput, TOutput> : PointModel, IIndicator<TInput, TOutput> where TInput : IPointModel
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public IndicatorModel()
    {
      Bar = new BarModel();
      Name = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Calculate indicator values
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public virtual TOutput Calculate(ObservableCollection<TInput> collection) => default;
  }
}
