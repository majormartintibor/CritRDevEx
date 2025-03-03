namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record LimitIncreaseRequested(
    Guid LoanAccountId,
    DateTimeOffset RequestedAt) : LoanAccountEvent;
