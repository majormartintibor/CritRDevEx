namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record LimitIncreaseRejected(
        Guid LoanAccountId,
        DateTimeOffset RejectedAt
    ) : LoanAccountEvent;
