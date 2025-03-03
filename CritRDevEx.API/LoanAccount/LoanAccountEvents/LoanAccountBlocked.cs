namespace CritRDevEx.API.LoanAccount.LoanAccountEvents;

public sealed record LoanAccountBlocked(Guid LoanAccountId, DateTimeOffset BlockedAt) : LoanAccountEvent;
