namespace CritRDevEx.API.LoanAccount.Deposit;

public sealed record MoneyDeposited(
        Guid LoanAccountId,
        decimal Amount
    ) : LoanAccountEvent;
