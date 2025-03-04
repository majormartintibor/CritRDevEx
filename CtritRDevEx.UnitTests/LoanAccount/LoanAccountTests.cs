using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using Marten.Events;

namespace CtritRDevEx.UnitTests.LoanAccount;

public class LoanAccountTests
{
    [Fact]
    public void Apply_WhenLoanAccountCreatedEvent_ShouldReturnAccountWithDebtorIdAndInitialLimit()
    {        
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();
        var debtorId = Guid.NewGuid();
        var initialLimit = 1000m;
        var createdAt = DateTimeOffset.Now;
        var loanAccountCreated = new LoanAccountCreated(debtorId, initialLimit, createdAt);
        var @event = new Event<LoanAccountEvent>(loanAccountCreated);        

        var updatedAccount = initialAccount.Apply(@event);
       
        Assert.Equal(debtorId, updatedAccount.DebtorId);
        Assert.Equal(initialLimit, updatedAccount.Limit);
        Assert.Equal(0, updatedAccount.Balance);
        Assert.Equal(createdAt, updatedAccount.LastLimitEvaluationDate);
    }

    [Fact]
    public void Apply_WhenMoneyDepositedEvent_ShouldReturnAccountWithIncreasedBalance()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();
        var initialBalance = 1000m;
        var depositAmount = 500m;        
        var moneyDeposited = new MoneyDeposited(default, depositAmount, default);
        var @event = new Event<LoanAccountEvent>(moneyDeposited);
        initialAccount = initialAccount with { Balance = initialBalance };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialBalance + depositAmount, updatedAccount.Balance);
    }

    [Fact]
    public void Apply_WhenMoneyWithdrawnEvent_ShouldReturnAccountWithDecreasedBalance()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();
        var initialBalance = -1000m;
        var withdrawAmount = 500m;        
        var moneyWithdrawn = new MoneyWithdrawn(default, withdrawAmount, default);
        var @event = new Event<LoanAccountEvent>(moneyWithdrawn);
        initialAccount = initialAccount with { Balance = initialBalance };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialBalance - withdrawAmount, updatedAccount.Balance);
    }

    [Fact]
    public void Apply_WhenAccountBlockedEvent_ShouldReturnAccountWithBlockedStatus()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();        
        var accountBlocked = new LoanAccountBlocked(default, default);
        var @event = new Event<LoanAccountEvent>(accountBlocked);

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(LoanAccountStatus.Blocked, updatedAccount.AccountStatus);        
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRequestedEvent_ShouldReturnAccountWithPendingLimitIncreaseRequest()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();
        var limitIncreaseRequested = new LimitIncreaseRequested(default, default);
        var @event = new Event<LoanAccountEvent>(limitIncreaseRequested);

        var updatedAccount = initialAccount.Apply(@event);

        Assert.True(updatedAccount.HasPendingLimitIncreaseRequest);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGrantedEvent_ShouldReturnAccountWithIncreasedLimit()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();
        initialAccount = initialAccount with { HasPendingLimitIncreaseRequest = true };
        var initialLimit = 1000m;
        var limitIncreaseAmount = 500m;
        var grantedAt = DateTimeOffset.Now;
        var limitIncreaseGranted = new LimitIncreaseGranted(default, limitIncreaseAmount, grantedAt);
        var @event = new Event<LoanAccountEvent>(limitIncreaseGranted);        
        initialAccount = initialAccount with { Limit = initialLimit };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialLimit - limitIncreaseAmount, updatedAccount.Limit);
        Assert.Equal(grantedAt, updatedAccount.LastLimitEvaluationDate);
        Assert.False(updatedAccount.HasPendingLimitIncreaseRequest);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRejectedEvent_ShouldReturnAccountWithUpdatedLastLimitEvaluationDate()
    {
        var initialAccount = new CritRDevEx.API.LoanAccount.Write.LoanAccount();
        initialAccount = initialAccount with { HasPendingLimitIncreaseRequest = true };
        var rejectedAt = DateTimeOffset.Now;
        var limitIncreaseRejected = new LimitIncreaseRejected(default, rejectedAt);
        var @event = new Event<LoanAccountEvent>(limitIncreaseRejected);        

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialAccount.Balance, updatedAccount.Balance);
        Assert.Equal(rejectedAt, updatedAccount.LastLimitEvaluationDate);
        Assert.False(updatedAccount.HasPendingLimitIncreaseRequest);
    }
}
