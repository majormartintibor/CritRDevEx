using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;
using Marten.Events.Aggregation;

namespace CritRDevEx.API.LoanAccount;

//Decision model for the account stream
public sealed record LoanAccount(
    Guid LoanAccountId,
    Guid DebtorId,
    decimal Limit,
    decimal Balance,
    LoanAccountStatus AccountStatus,
    DateTimeOffset LastLimitEvaluationDate)
{
    public LoanAccount() 
        : this(default, default, default, default, LoanAccountStatus.Default, default)
    {        
    }

    public LoanAccount Apply(IEvent<LoanAccountEvent> @event) =>
        @event.Data switch
        {
            LoanAccountCreated(Guid debtorId, decimal initialLimit) =>
                this with { DebtorId = debtorId, Limit = initialLimit, LastLimitEvaluationDate = @event.Timestamp },
            MoneyDeposited(Guid, decimal amount) =>
                this with { Balance = Balance + amount },
            MoneyWithdrawn(Guid, decimal amount) =>
                this with { Balance = Balance - amount },
            LoanAccountBlocked(Guid) =>
                this with { AccountStatus = LoanAccountStatus.Blocked },
            LimitIncreaseGranted(Guid, decimal limitIncreaseAmount) =>
                this with { Limit = Limit - limitIncreaseAmount, LastLimitEvaluationDate = @event.Timestamp },
            LimitIncreaseRejected(Guid) =>
                this with { LastLimitEvaluationDate = @event.Timestamp },
            _ => this
        };
}

public sealed class LoanAccountProjection
    : SingleStreamProjection<LoanAccount>
{
    public LoanAccount Apply(IEvent<LoanAccountEvent> @event, LoanAccount current)
        => current.Apply(@event);
}