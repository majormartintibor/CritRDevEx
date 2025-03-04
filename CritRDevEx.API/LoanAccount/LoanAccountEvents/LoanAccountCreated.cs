namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record LoanAccountCreated(
        Guid DebtorId,
        decimal IntialLimit,
        DateTimeOffset CreatedAt
    ) : LoanAccountEvent;
