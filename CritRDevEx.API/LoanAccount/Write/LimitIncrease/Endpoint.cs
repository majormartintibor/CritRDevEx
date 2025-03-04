using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
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

        if (account.AccountStatus == LoanAccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        if (account.HasPendingLimitIncreaseRequest)
            throw new InvalidOperationException("Limit increase request is already pending");

        if (account.LastLimitEvaluationDate > DateTimeOffset.UtcNow.AddDays(-30))
            throw new InvalidOperationException("Limit increase can be requested only once in 30 days");

        events.Add(new LimitIncreaseRequested(command.LoanAccountId, DateTimeProvider.UtcNow));
        return (Results.Ok(), events, messages);
    }
}
