namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record LimitIncreaseGranted(
    Guid LoanAccountId,
    decimal LimitIncreaseAmount,
    DateTimeOffset GrantedAt) : LoanAccountEvent;