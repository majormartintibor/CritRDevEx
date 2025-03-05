using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.Deposit;

public static class Endpoint
{
    public sealed record DepositToLoanAccountCommand(Guid LoanAccountId, decimal Amount)
    {
        public sealed class DepositLoanAccountCommandValidator : AbstractValidator<DepositToLoanAccountCommand>
        {
            public DepositLoanAccountCommandValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThan(0);
            }
        }
    }

    public static ProblemDetails Validate(LoanAccount account)
    {
        if (account.AccountStatus == LoanAccountStatus.Blocked)
            return new ProblemDetails { Detail = "Account is blocked", Status = StatusCodes.Status412PreconditionFailed };

        return WolverineContinue.NoProblems;
    }

    public const string DepositToLoanAccountEndpoint = "/api/loanAccount/deposit";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(DepositToLoanAccountEndpoint)]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) DepositToAccount(
        DepositToLoanAccountCommand command,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];        

        events.Add(new MoneyDeposited(account.Id, command.Amount, DateTimeProvider.UtcNow));

        return (Results.Ok(), events, messages);
    }
}
