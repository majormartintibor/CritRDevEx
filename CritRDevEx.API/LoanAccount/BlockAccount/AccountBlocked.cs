namespace CritRDevEx.API.LoanAccount.BlockAccount;

public sealed record AccountBlocked(Guid AccountId) : LoanAccountEvent;
