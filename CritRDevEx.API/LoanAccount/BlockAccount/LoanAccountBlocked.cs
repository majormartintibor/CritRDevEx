namespace CritRDevEx.API.LoanAccount.BlockAccount;

public sealed record LoanAccountBlocked(Guid LoanAccountId, DateTimeOffset BlockedAt) : LoanAccountEvent;
