namespace CritRDevEx.API.LoanAccount.LimitIncrease;

public sealed record LimitIncreaseRejected(
        Guid LoanAccountId
    ) : LoanAccountEvent;
