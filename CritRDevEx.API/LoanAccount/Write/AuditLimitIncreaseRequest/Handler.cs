using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.AuditLimitIncreaseRequest;

public static class AuditLimitIncreaseCommandHandler
{
    public sealed record AuditLimitIncreaseCommand(Guid LoanAccountId, decimal LifetimeDeposits)
    {
        public sealed class AuditLimitIncreaseCommandValidator : AbstractValidator<AuditLimitIncreaseCommand>
        {
            public AuditLimitIncreaseCommandValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
                RuleFor(x => x.LifetimeDeposits).GreaterThanOrEqualTo(0);
            }
        }
    }

    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(
    AuditLimitIncreaseCommand command,
    [Required] LoanAccount account)
    {
        var events = new Events();
        var messages = new OutgoingMessages();

        var eventType =
            account.AccountStatus == LoanAccountStatus.Blocked
            || command.LifetimeDeposits < Math.Abs(account.Limit) * 3
                ? typeof(LimitIncreaseRejected)
                : typeof(LimitIncreaseGranted);

        events.Add(
            eventType == typeof(LimitIncreaseGranted)
            ? new LimitIncreaseGranted(account.Id, 10000, DateTimeProvider.UtcNow)
            : new LimitIncreaseRejected(account.Id, DateTimeProvider.UtcNow)
        );

        return (events, messages);
    }
}
