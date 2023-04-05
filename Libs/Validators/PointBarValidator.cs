using FluentValidation;
using Terminal.Core.ModelSpace;

namespace Terminal.Core.ValidatorSpace
{
  /// <summary>
  /// Validation rules
  /// </summary>
  public class PointBarValidator : AbstractValidator<BarModel>
  {
    public PointBarValidator()
    {
      RuleFor(o => o.Low).NotEmpty();
      RuleFor(o => o.High).NotEmpty();
      RuleFor(o => o.Open).NotEmpty();
      RuleFor(o => o.Close).NotEmpty();
    }
  }
}
