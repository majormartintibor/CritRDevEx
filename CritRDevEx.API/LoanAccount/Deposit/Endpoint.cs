using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Deposit;

public static class Endpoint
{
    public sealed record DepositToLoanAccount(Guid LoanAccountId, decimal Amount)
    {
        public sealed class DepositLoanAccountValidator : AbstractValidator<DepositToLoanAccount>
        {
            public DepositLoanAccountValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThan(0);
            }
        }
    }

    public const string DepositToLoanAccountEndpoint = "/api/loanAccount/deposit";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(DepositToLoanAccountEndpoint)]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) DepositToAccount(
        DepositToLoanAccount request,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == LoanAccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        events.Add(new MoneyDeposited(request.LoanAccountId, request.Amount));

        return (Results.Ok(), events, messages);
    }
}
