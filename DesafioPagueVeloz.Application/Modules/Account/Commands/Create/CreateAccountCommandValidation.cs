using DesafioPagueVeloz.Persistense.Repositories;
using FluentValidation;

namespace DesafioPagueVeloz.Application;

public class CreateAccountCommandValidation : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidation()
    {
        RuleFor(x => x.ClientId).NotEmpty().WithMessage("ClientId is required");
        RuleFor(x => x.Balance).NotEmpty().WithMessage("Balance is required");
        RuleFor(x => x.CreditLimit).NotEmpty().WithMessage("CreditLimit is required");
        RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is required");
    }
}