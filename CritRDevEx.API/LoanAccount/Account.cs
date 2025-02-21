namespace CritRDevEx.API.LoanAccount;

public sealed record Account(
    Guid AccountId,
    Guid DebtorId,
    decimal Limit,
    decimal Balance,
    AccountStatus AccountStatus);
