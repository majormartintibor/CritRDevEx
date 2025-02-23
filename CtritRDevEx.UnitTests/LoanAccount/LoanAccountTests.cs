using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;
using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;

namespace CtritRDevEx.UnitTests.LoanAccount;

public class LoanAccountTests
{
    [Fact]
    public void Apply_WhenLoanAccountCreatedEvent_ShouldReturnAccountWithDebtorIdAndInitialLimit()
    {        
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        var debtorId = Guid.NewGuid();
        var initialLimit = 1000m;
        var loanAccountCreated = new LoanAccountCreated(debtorId, initialLimit);
        var @event = new Event<LoanAccountEvent>(loanAccountCreated);
        @event.Timestamp = DateTimeOffset.Now;

        var updatedAccount = initialAccount.Apply(@event);
       
        Assert.Equal(debtorId, updatedAccount.DebtorId);
        Assert.Equal(initialLimit, updatedAccount.Limit);
        Assert.Equal(0, updatedAccount.Balance);
        Assert.Equal(@event.Timestamp, updatedAccount.LastLimitEvaluationDate);
    }

    [Fact]
    public void Apply_WhenMoneyDepositedEvent_ShouldReturnAccountWithIncreasedBalance()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        var initialBalance = 1000m;
        var depositAmount = 500m;
        var moneyDeposited = new MoneyDeposited(default, depositAmount);
        var @event = new Event<LoanAccountEvent>(moneyDeposited);
        initialAccount = initialAccount with { Balance = initialBalance };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialBalance + depositAmount, updatedAccount.Balance);
    }

    [Fact]
    public void Apply_WhenMoneyWithdrawnEvent_ShouldReturnAccountWithDecreasedBalance()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        var initialBalance = -1000m;
        var withdrawAmount = 500m;
        var moneyWithdrawn = new MoneyWithdrawn(default, withdrawAmount);
        var @event = new Event<LoanAccountEvent>(moneyWithdrawn);
        initialAccount = initialAccount with { Balance = initialBalance };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialBalance - withdrawAmount, updatedAccount.Balance);
    }

    [Fact]
    public void Apply_WhenAccountBlockedEvent_ShouldReturnAccountWithBlockedStatus()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        var accountBlocked = new LoanAccountBlocked(default);
        var @event = new Event<LoanAccountEvent>(accountBlocked);

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(LoanAccountStatus.Blocked, updatedAccount.AccountStatus);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRequestedEvent_ShouldReturnAccountWithPendingLimitIncreaseRequest()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        var limitIncreaseRequested = new LimitIncreaseRequested(default);
        var @event = new Event<LoanAccountEvent>(limitIncreaseRequested);

        var updatedAccount = initialAccount.Apply(@event);

        Assert.True(updatedAccount.HasPendingLimitIncreaseRequest);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGrantedEvent_ShouldReturnAccountWithIncreasedLimit()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        initialAccount = initialAccount with { HasPendingLimitIncreaseRequest = true };
        var initialLimit = 1000m;
        var limitIncreaseAmount = 500m;
        var limitIncreaseGranted = new LimitIncreaseGranted(default, limitIncreaseAmount);
        var @event = new Event<LoanAccountEvent>(limitIncreaseGranted);
        @event.Timestamp = DateTimeOffset.Now;
        initialAccount = initialAccount with { Limit = initialLimit };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialLimit - limitIncreaseAmount, updatedAccount.Limit);
        Assert.Equal(@event.Timestamp, updatedAccount.LastLimitEvaluationDate);
        Assert.False(updatedAccount.HasPendingLimitIncreaseRequest);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRejectedEvent_ShouldReturnAccountWithUpdatedLastLimitEvaluationDate()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.LoanAccount();
        initialAccount = initialAccount with { HasPendingLimitIncreaseRequest = true };
        var limitIncreaseRejected = new LimitIncreaseRejected(default);
        var @event = new Event<LoanAccountEvent>(limitIncreaseRejected);
        @event.Timestamp = DateTimeOffset.Now;

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialAccount.Balance, updatedAccount.Balance);
        Assert.Equal(@event.Timestamp, updatedAccount.LastLimitEvaluationDate);
        Assert.False(updatedAccount.HasPendingLimitIncreaseRequest);
    }
}
