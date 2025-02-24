using CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;
using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;
using Marten.Events.Aggregation;

namespace CritRDevEx.API.LoanAccount.Details;

public sealed record LoanAccountDetail(
        Guid Id,
        decimal Limit,
        decimal Balance,
        LoanAccountStatus AccountStatus)
{
    public LoanAccountDetail() 
        : this(Guid.Empty, default, default, LoanAccountStatus.Default)
    {
    }

    public LoanAccountDetail Apply(IEvent<LoanAccountEvent> @event) =>
        @event.Data switch
        {
            LoanAccountCreated(Guid, decimal initialLimit, DateTimeOffset) =>
                this with { Limit = initialLimit },
            MoneyDeposited(Guid, decimal amount, DateTimeOffset) =>
                this with { Balance = Balance + amount },
            MoneyWithdrawn(Guid, decimal amount, DateTimeOffset) =>
                this with { Balance = Balance - amount },
            LoanAccountBlocked(Guid, DateTimeOffset) =>
                this with { AccountStatus = LoanAccountStatus.Blocked },
            LimitIncreaseGranted(Guid, decimal limitIncreaseAmount, DateTimeOffset) =>
                this with { Limit = Limit - limitIncreaseAmount },
            _ => this
        };
}

public sealed class LoanAccountDetailProjection
    : SingleStreamProjection<LoanAccountDetail>
{
    public LoanAccountDetail Apply(IEvent<LoanAccountEvent> @event, LoanAccountDetail current)
        => current.Apply(@event);
}
