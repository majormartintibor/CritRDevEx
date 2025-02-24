namespace CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;

public sealed record LimitIncreaseGranted(
    Guid LoanAccountId,
    decimal LimitIncreaseAmount,
    DateTimeOffset GrantedAt) : LoanAccountEvent;