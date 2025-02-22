namespace CritRDevEx.API.LoanAccount.LimitIncrease;

public sealed record LimitIncreaseGranted(
    Guid LoanAccountId,
    decimal LimitIncreaseAmount) : LoanAccountEvent;