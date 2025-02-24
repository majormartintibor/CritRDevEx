namespace CritRDevEx.API.LoanAccount.Withdraw;

public sealed record MoneyWithdrawn(
        Guid LoanAccountId,
        decimal Amount,
        DateTimeOffset TransactionDate
    ) : LoanAccountEvent;
