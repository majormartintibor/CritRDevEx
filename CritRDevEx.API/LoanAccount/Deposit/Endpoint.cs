using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Deposit;

public static class Endpoint
{
    public sealed record DepositToLoanAccount(Guid AccountId, decimal Amount)
    {
        public sealed class DepositLoanAccountValidator : AbstractValidator<DepositToLoanAccount>
        {
            public DepositLoanAccountValidator()
            {
                RuleFor(x => x.AccountId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThan(0);
            }
        }
    }

    public const string DepositAccountEnpoint = "/api/loanAccount/deposit";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(DepositAccountEnpoint)]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) DepositToAccount(
        DepositToLoanAccount request,
        [Required] Account account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == AccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        events.Add(new MoneyDeposited(request.AccountId, request.Amount));

        return (Results.Ok(), events, messages);
    }
}
