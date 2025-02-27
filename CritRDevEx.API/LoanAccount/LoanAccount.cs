using CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;
using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;
using Marten.Events.Aggregation;

namespace CritRDevEx.API.LoanAccount;

//Decision model for the account stream aka Aggregate
public sealed record LoanAccount(
    Guid Id,
    Guid DebtorId,
    decimal Limit,
    decimal Balance,
    LoanAccountStatus AccountStatus,
    DateTimeOffset LastLimitEvaluationDate,
    bool HasPendingLimitIncreaseRequest = false)
{
    public LoanAccount() 
        : this(default, default, default, default, LoanAccountStatus.Default, default)
    {        
    }

    public LoanAccount Apply(IEvent<LoanAccountEvent> @event) =>
        @event.Data switch
        {
            LoanAccountCreated(Guid debtorId, decimal initialLimit, DateTimeOffset createdAt) =>
                this with { DebtorId = debtorId, Limit = initialLimit, LastLimitEvaluationDate = createdAt },
            MoneyDeposited(Guid, decimal amount, DateTimeOffset) =>
                this with { Balance = Balance + amount },
            MoneyWithdrawn(Guid, decimal amount, DateTimeOffset) =>
                this with { Balance = Balance - amount },
            LoanAccountBlocked(Guid, DateTimeOffset) =>
                this with { AccountStatus = LoanAccountStatus.Blocked },
            LimitIncreaseRequested(Guid, DateTimeOffset) =>
                this with { HasPendingLimitIncreaseRequest = true },
            LimitIncreaseGranted(Guid, decimal limitIncreaseAmount, DateTimeOffset grantedAt) =>
                this with { Limit = Limit - limitIncreaseAmount, LastLimitEvaluationDate = grantedAt, HasPendingLimitIncreaseRequest = false },
            LimitIncreaseRejected(Guid, DateTimeOffset rejectedAt) =>
                this with { LastLimitEvaluationDate = rejectedAt, HasPendingLimitIncreaseRequest = false },
            _ => this
        };
}

public sealed class LoanAccountProjection
    : SingleStreamProjection<LoanAccount>
{
    public LoanAccount Apply(IEvent<LoanAccountEvent> @event, LoanAccount current)
        => current.Apply(@event);
}