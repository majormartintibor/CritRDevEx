namespace CritRDevEx.API.LoanAccount.LimitIncrease;

public sealed record LimitIncreaseRequested(
    Guid LoanAccountId) : LoanAccountEvent;
