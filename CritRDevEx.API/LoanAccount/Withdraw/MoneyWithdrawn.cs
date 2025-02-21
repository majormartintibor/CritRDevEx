namespace CritRDevEx.API.LoanAccount.Withdraw;

public sealed record MoneyWithdrawn(
        Guid AccountId,
        decimal Amount
    ) : LoanAccountEvent;
