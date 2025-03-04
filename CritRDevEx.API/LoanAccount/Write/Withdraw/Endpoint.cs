using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.Withdraw;

public static class Endpoint
{
    public sealed record WithdrawFromLoanAccountCommand(Guid LoanAccountId, decimal Amount)
    {
        public sealed class WithdrawLoanAccountCommandValidator : AbstractValidator<WithdrawFromLoanAccountCommand>
        {
            public WithdrawLoanAccountCommandValidator()
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
        WithdrawFromLoanAccountCommand command,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == LoanAccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        if (account.Balance - command.Amount < account.Limit)
            throw new InvalidOperationException("Withdrawal amount exceeds account limit");

        events.Add(new MoneyWithdrawn(command.LoanAccountId, command.Amount, DateTimeProvider.UtcNow));

        return (Results.Ok(), events, messages);
    }
}
