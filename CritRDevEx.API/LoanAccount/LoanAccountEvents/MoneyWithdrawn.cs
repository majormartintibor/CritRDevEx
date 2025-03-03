namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record MoneyWithdrawn(
        Guid LoanAccountId,
        decimal Amount,
        DateTimeOffset TransactionDate
    ) : LoanAccountEvent;
