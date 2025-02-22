using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;
using Marten.Events.Aggregation;

namespace CritRDevEx.API.LoanAccount;

//Decision model for the account stream
public sealed record Account(
    Guid AccountId,
    Guid DebtorId,
    decimal Limit,
    decimal Balance,
    AccountStatus AccountStatus,
    DateTimeOffset LastLimitEvaluationDate)
{
    public Account() 
        : this(default, default, default, default, AccountStatus.Default, default)
    {        
    }

    public Account Apply(IEvent<LoanAccountEvent> @event) =>
        @event.Data switch
        {
            LoanAccountCreated(Guid debtorId, decimal initialLimit) =>
                this with { DebtorId = debtorId, Limit = initialLimit, LastLimitEvaluationDate = @event.Timestamp },
            MoneyDeposited(Guid, decimal amount) =>
                this with { Balance = Balance + amount },
            MoneyWithdrawn(Guid, decimal amount) =>
                this with { Balance = Balance - amount },
            AccountBlocked(Guid) =>
                this with { AccountStatus = AccountStatus.Blocked },
            LimitIncreaseGranted(Guid, decimal limitIncreaseAmount) =>
                this with { Limit = Limit - limitIncreaseAmount, LastLimitEvaluationDate = @event.Timestamp },
            LimitIncreaseRejected(Guid) =>
                this with { LastLimitEvaluationDate = @event.Timestamp },
            _ => this
        };
}

public sealed class AccountProjection
    : SingleStreamProjection<Account>
{
    public Account Apply(IEvent<LoanAccountEvent> @event, Account current)
        => current.Apply(@event);
}