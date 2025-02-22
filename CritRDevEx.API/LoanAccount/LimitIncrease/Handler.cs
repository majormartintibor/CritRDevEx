using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.LimitIncrease;

public static class AuditLimitIncreaseRequestHandler
{
    public sealed record AuditLimitIncreaseRequest(Guid AccountId, decimal LifetimeDeposits)
    {
        public sealed class AuditLimitIncreaseRequestValidator : AbstractValidator<AuditLimitIncreaseRequest>
        {
            public AuditLimitIncreaseRequestValidator()
            {
                RuleFor(x => x.AccountId).NotEmpty();
                RuleFor(x => x.LifetimeDeposits).GreaterThanOrEqualTo(0);
            }
        }
    }

    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(
    AuditLimitIncreaseRequest request,
    [Required] Account account)
    {
        var events = new Events();
        var messages = new OutgoingMessages();

        var eventType = 
            account.AccountStatus == AccountStatus.Blocked 
            || request.LifetimeDeposits < (Math.Abs(account.Limit) * 3)
                ? typeof(LimitIncreaseRejected)
                : typeof(LimitIncreaseGranted);

        events.Add(
            eventType == typeof(LimitIncreaseGranted)
            ? new LimitIncreaseGranted(account.AccountId, 10000)
            : new LimitIncreaseRejected(account.AccountId)
        );

        return (events, messages);
    }
}
