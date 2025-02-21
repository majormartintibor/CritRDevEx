namespace CritRDevEx.API.LoanAccount.LimitIncrease;

public sealed record LimitIncreaseGranted(
    Guid AccountId,
    decimal LimitIncreaseAmount) : LoanAccountEvent;