namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record MoneyDeposited(
        Guid LoanAccountId,
        decimal Amount,
        DateTimeOffset TransactionDate
    ) : LoanAccountEvent;
