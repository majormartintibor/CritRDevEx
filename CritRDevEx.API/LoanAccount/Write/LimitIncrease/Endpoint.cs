using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.LimitIncrease;

public static class Endpoint
{
    public sealed record RequestLimitIncreaseCommand(Guid LoanAccountId)
    {
        public sealed class RequestLimitIncreaseCommandValidator : AbstractValidator<RequestLimitIncreaseCommand>
        {
            public RequestLimitIncreaseCommandValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
            }
        }
    }

    public static ProblemDetails Validate(LoanAccount account)
    {
        if (account.AccountStatus == LoanAccountStatus.Blocked)
            return new ProblemDetails { Detail = "Account is blocked", Status = StatusCodes.Status412PreconditionFailed };

        if (account.HasPendingLimitIncreaseRequest)
            return new ProblemDetails { Detail = "Limit increase request is already pending", Status = StatusCodes.Status412PreconditionFailed };
        
        if (account.LastLimitEvaluationDate > DateTimeOffset.UtcNow.AddDays(-30))
            return new ProblemDetails { Detail = "Limit increase can be requested only once in 30 days", Status = StatusCodes.Status412PreconditionFailed };
        
        return WolverineContinue.NoProblems;
    }

    public const string RequestLimitIncreaseEndpoint = "/api/loanAccount/requestLimitIncrease";

    [Tags(Tag.LoanAccount)]
    [WolverinePost(RequestLimitIncreaseEndpoint)]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) ReceiveLimitIncreaseRequest(
        RequestLimitIncreaseCommand command,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];        

        events.Add(new LimitIncreaseRequested(account.Id, DateTimeProvider.UtcNow));

        return (Results.Ok(), events, messages);
    }
}
