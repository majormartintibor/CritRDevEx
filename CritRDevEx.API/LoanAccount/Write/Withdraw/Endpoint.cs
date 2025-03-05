using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

    public static ProblemDetails Validate(WithdrawFromLoanAccountCommand command, LoanAccount account)
    {
        if (account.AccountStatus == LoanAccountStatus.Blocked)        
            return new ProblemDetails { Detail = "Account is blocked", Status = StatusCodes.Status412PreconditionFailed };

        if (account.Balance - command.Amount < account.Limit)
            return new ProblemDetails { Detail = "Withdrawal amount exceeds account limit", Status = StatusCodes.Status412PreconditionFailed };
        
        return WolverineContinue.NoProblems;
    }

    //todo: consider moving id's to route
    public const string WithdrawFromLoanAccountEndpoint = "/api/loanAccount/withdraw";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(WithdrawFromLoanAccountEndpoint), EmptyResponse]
    [AggregateHandler]
    public static MoneyWithdrawn WithdrawFromAccount(
        WithdrawFromLoanAccountCommand command,
        [Required] LoanAccount account)
            => new(account.Id, command.Amount, DateTimeProvider.UtcNow);    
}
