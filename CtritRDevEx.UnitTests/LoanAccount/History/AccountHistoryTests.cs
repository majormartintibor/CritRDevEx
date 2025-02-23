using CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;
using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.History;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using Marten.Events;

namespace CtritRDevEx.UnitTests.LoanAccount.History;

public class AccountHistoryTests
{
    [Fact]
    public void Transform_WhenLoanAccountCreated_ShouldReturnAccountHistory()
    {        
        var loanAccountCreated = new LoanAccountCreated(default, 1000);
        var @event = new Event<LoanAccountCreated>(loanAccountCreated);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();
        
        var result = sut.Transform(@event);        
        
        Assert.Equal($"Account has been created at: {@event.Timestamp} with initial limit of: {loanAccountCreated.IntialLimit}", result.Description);
    }

    [Fact]
    public void Transform_WhenMoneyDeposited_ShouldReturnAccountHistory()
    {
        var moneyDeposited = new MoneyDeposited(default, 500);
        var @event = new Event<MoneyDeposited>(moneyDeposited);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"{moneyDeposited.Amount} has been deposited at: {@event.Timestamp}", result.Description);
    }

    [Fact]
    public void Transform_WhenMoneyWithdrawn_ShouldReturnAccountHistory()
    {
        var moneyWithdrawn = new MoneyWithdrawn(default, 500);
        var @event = new Event<MoneyWithdrawn>(moneyWithdrawn);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"{moneyWithdrawn.Amount} has been withdrawn at: {@event.Timestamp}", result.Description);
    }

    [Fact]
    public void Transform_WhenAccountBlocked_ShouldReturnAccountHistory()
    {
        var accountBlocked = new LoanAccountBlocked(default);
        var @event = new Event<LoanAccountBlocked>(accountBlocked);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Account has been blocked at: {@event.Timestamp}", result.Description);
    }

    [Fact]
    public void Transform_WhenLimitIncreaseRequested_ShouldReturnAccountHistory()
    {
        var limitIncreaseRequested = new LimitIncreaseRequested(default);
        var @event = new Event<LimitIncreaseRequested>(limitIncreaseRequested);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Limit increase has been requested at: {@event.Timestamp}", result.Description);
    }

    [Fact]
    public void Transform_WhenLimitIncreaseGranted_ShouldReturnAccountHistory()
    {
        var limitIncreaseGranted = new LimitIncreaseGranted(default, 500);
        var @event = new Event<LimitIncreaseGranted>(limitIncreaseGranted);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Limit increase has been granted at: {@event.Timestamp} with increase of {limitIncreaseGranted.LimitIncreaseAmount}", result.Description);
    }

    [Fact]
    public void Transform_WhenLimitIncreaseRejected_ShouldReturnAccountHistory()
    {
        var limitIncreaseRejected = new LimitIncreaseRejected(default);
        var @event = new Event<LimitIncreaseRejected>(limitIncreaseRejected);
        @event.Timestamp = DateTimeOffset.Now;
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Limit increase has been rejected at: {@event.Timestamp}", result.Description);
    }
}
