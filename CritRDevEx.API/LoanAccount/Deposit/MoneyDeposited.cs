namespace CritRDevEx.API.LoanAccount.Deposit;

public sealed record MoneyDeposited(
        Guid AccountId,
        decimal Amount
    ) : LoanAccountEvent;
