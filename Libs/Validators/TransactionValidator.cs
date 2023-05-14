using FluentValidation;
using Terminal.Core.Models;

namespace Terminal.Core.Validators
{
  /// <summary>
  /// Validation rules
  /// </summary>
  public class TransactionValidator : AbstractValidator<TransactionModel>
  {
    public TransactionValidator()
    {
      RuleFor(o => o.Id).NotEmpty();
      RuleFor(o => o.Time).NotEmpty();
      RuleFor(o => o.Price).NotEmpty();
      RuleFor(o => o.Volume).NotEmpty();
      RuleFor(o => o.Instrument).NotEmpty().SetValidator(new InstrumentValidator());
    }
  }
}
