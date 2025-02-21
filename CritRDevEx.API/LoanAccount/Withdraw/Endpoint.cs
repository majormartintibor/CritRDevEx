using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Withdraw;

public static class Endpoint
{
    public sealed record WithdrawFromLoanAccount(Guid AccountId, decimal Amount)
    {
        public sealed class WithdrawLoanAccountValidator : AbstractValidator<WithdrawFromLoanAccount>
        {
            public WithdrawLoanAccountValidator()
            {
                RuleFor(x => x.AccountId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThan(0);
            }
        }
    }
    
    public const string WithdrawAccountEnpoint = "/api/loanAccount/withdraw";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(WithdrawAccountEnpoint)]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) WithdrawFromAccount(
        WithdrawFromLoanAccount request,
        [Required] Account account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == AccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        if ((account.Balance - request.Amount) < account.Limit)
            throw new InvalidOperationException("Withdrawal amount exceeds account limit");

        events.Add(new MoneyWithdrawn(request.AccountId, request.Amount));

        return (Results.Ok(), events, messages);
    }
}
