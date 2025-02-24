namespace CritRDevEx.API.LoanAccount.CreateAccount;

public sealed record LoanAccountCreated(        
        Guid DebtorId,
        decimal IntialLimit,
        DateTimeOffset CreatedAt
    ) : LoanAccountEvent;
