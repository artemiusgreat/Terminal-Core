using FluentValidation;
using Terminal.Core.ModelSpace;

namespace Terminal.Core.ValidatorSpace
{
  /// <summary>
  /// Validation rules
  /// </summary>
  public class InstrumentFutureValidator : AbstractValidator<FutureModel>
  {
    public InstrumentFutureValidator()
    {
      RuleFor(o => o.ExpirationDate).NotEmpty();
    }
  }
}
