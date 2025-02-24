using CritRDevEx.API.Clock;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.LimitIncrease;

public static class Endpoint
{
    public sealed record RequestLimitIncrease(Guid LoanAccountId)
    {
        public sealed class RequestLimitIncreaseValidator : AbstractValidator<RequestLimitIncrease>
        {
            public RequestLimitIncreaseValidator()
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
        RequestLimitIncrease request,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == LoanAccountStatus.Blocked)
            throw new InvalidOperationException("Account is blocked");

        if (account.HasPendingLimitIncreaseRequest)
            throw new InvalidOperationException("Limit increase request is already pending");

        //how did copilot know this rule?
        if (account.LastLimitEvaluationDate > DateTimeOffset.UtcNow.AddDays(-30))
            throw new InvalidOperationException("Limit increase can be requested only once in 30 days");

        events.Add(new LimitIncreaseRequested(request.LoanAccountId, DateTimeProvider.UtcNow));
        return (Results.Ok(), events, messages);
    }
}
