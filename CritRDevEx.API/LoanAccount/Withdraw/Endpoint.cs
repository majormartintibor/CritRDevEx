using CritRDevEx.API.Clock;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Withdraw;

public static class Endpoint
{
    public sealed record WithdrawFromLoanAccount(Guid LoanAccountId, decimal Amount)
    {
        public sealed class WithdrawLoanAccountValidator : AbstractValidator<WithdrawFromLoanAccount>
        {
            public WithdrawLoanAccountValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThan(0);
            }
        }
    }
    
    public const string WithdrawFromLoanAccountEndpoint = "/api/loanAccount/withdraw";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(WithdrawFromLoanAccountEndpoint)]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) WithdrawFromAccount(
        WithdrawFromLoanAccount request,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == LoanAccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        if ((account.Balance - request.Amount) < account.Limit)
            throw new InvalidOperationException("Withdrawal amount exceeds account limit");

        events.Add(new MoneyWithdrawn(request.LoanAccountId, request.Amount, DateTimeProvider.UtcNow));

        return (Results.Ok(), events, messages);
    }
}
