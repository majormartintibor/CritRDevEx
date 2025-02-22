using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;

namespace CtritRDevEx.UnitTests.LoanAccount;

public class AccountTests
{
    [Fact]
    public void Apply_WhenLoanAccountCreatedEvent_ShouldReturnAccountWithDebtorIdAndInitialLimit()
    {        
        var initialAccount = new Account();
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
        var initialAccount = new Account();
        var initialBalance = 1000m;
        var depositAmount = 500m;
        var moneyDeposited = new MoneyDeposited(Guid.NewGuid(), depositAmount);
        var @event = new Event<LoanAccountEvent>(moneyDeposited);
        initialAccount = initialAccount with { Balance = initialBalance };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialBalance + depositAmount, updatedAccount.Balance);
    }

    [Fact]
    public void Apply_WhenMoneyWithdrawnEvent_ShouldReturnAccountWithDecreasedBalance()
    {
        var initialAccount = new Account();
        var initialBalance = -1000m;
        var withdrawAmount = 500m;
        var moneyWithdrawn = new MoneyWithdrawn(Guid.NewGuid(), withdrawAmount);
        var @event = new Event<LoanAccountEvent>(moneyWithdrawn);
        initialAccount = initialAccount with { Balance = initialBalance };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialBalance - withdrawAmount, updatedAccount.Balance);
    }

    [Fact]
    public void Apply_WhenAccountBlockedEvent_ShouldReturnAccountWithBlockedStatus()
    {
        var initialAccount = new Account();
        var accountBlocked = new AccountBlocked(Guid.NewGuid());
        var @event = new Event<LoanAccountEvent>(accountBlocked);

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(AccountStatus.Blocked, updatedAccount.AccountStatus);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGrantedEvent_ShouldReturnAccountWithDecreasedLimit()
    {
        var initialAccount = new Account();
        var initialLimit = 1000m;
        var limitIncreaseAmount = 500m;
        var limitIncreaseGranted = new LimitIncreaseGranted(Guid.NewGuid(), limitIncreaseAmount);
        var @event = new Event<LoanAccountEvent>(limitIncreaseGranted);
        @event.Timestamp = DateTimeOffset.Now;
        initialAccount = initialAccount with { Limit = initialLimit };

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialLimit - limitIncreaseAmount, updatedAccount.Limit);
        Assert.Equal(@event.Timestamp, updatedAccount.LastLimitEvaluationDate);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRejectedEvent_ShouldReturnAccountWithUpdatedLastLimitEvaluationDate()
    {
        var initialAccount = new Account();
        var limitIncreaseRejected = new LimitIncreaseRejected(Guid.NewGuid());
        var @event = new Event<LoanAccountEvent>(limitIncreaseRejected);
        @event.Timestamp = DateTimeOffset.Now;

        var updatedAccount = initialAccount.Apply(@event);

        Assert.Equal(initialAccount.Balance, updatedAccount.Balance);
        Assert.Equal(@event.Timestamp, updatedAccount.LastLimitEvaluationDate);
    }
}
