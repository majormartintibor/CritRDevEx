using CritRDevEx.API.Clock;
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
        var createdAt = DateTimeProvider.UtcNow;
        var loanAccountCreated = new LoanAccountCreated(default, 1000, createdAt);
        var @event = new Event<LoanAccountCreated>(loanAccountCreated);        
        var sut = new LoanAccountHistoryTransformation();
        
        var result = sut.Transform(@event);        
        
        Assert.Equal($"Account has been created at: {createdAt} with initial limit of: {loanAccountCreated.IntialLimit}", result.Description);
    }

    [Fact]
    public void Transform_WhenMoneyDeposited_ShouldReturnAccountHistory()
    {
        var transactionDate = DateTimeProvider.UtcNow;
        var moneyDeposited = new MoneyDeposited(default, 500, transactionDate);
        var @event = new Event<MoneyDeposited>(moneyDeposited);        
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"{moneyDeposited.Amount} has been deposited at: {transactionDate}", result.Description);
    }

    [Fact]
    public void Transform_WhenMoneyWithdrawn_ShouldReturnAccountHistory()
    {
        var transactionDate = DateTimeProvider.UtcNow;
        var moneyWithdrawn = new MoneyWithdrawn(default, 500, transactionDate);
        var @event = new Event<MoneyWithdrawn>(moneyWithdrawn);        
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"{moneyWithdrawn.Amount} has been withdrawn at: {transactionDate}", result.Description);
    }

    [Fact]
    public void Transform_WhenAccountBlocked_ShouldReturnAccountHistory()
    {
        var blockedAt = DateTimeProvider.UtcNow;
        var accountBlocked = new LoanAccountBlocked(default, blockedAt);
        var @event = new Event<LoanAccountBlocked>(accountBlocked);        
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Account has been blocked at: {blockedAt}", result.Description);
    }

    [Fact]
    public void Transform_WhenLimitIncreaseRequested_ShouldReturnAccountHistory()
    {
        var requestedAt = DateTimeProvider.UtcNow;
        var limitIncreaseRequested = new LimitIncreaseRequested(default, requestedAt);
        var @event = new Event<LimitIncreaseRequested>(limitIncreaseRequested);        
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Limit increase has been requested at: {requestedAt}", result.Description);
    }

    [Fact]
    public void Transform_WhenLimitIncreaseGranted_ShouldReturnAccountHistory()
    {
        var createdAt = DateTimeProvider.UtcNow;
        var limitIncreaseGranted = new LimitIncreaseGranted(default, 500, createdAt);
        var @event = new Event<LimitIncreaseGranted>(limitIncreaseGranted);        
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Limit increase has been granted at: {createdAt} with increase of {limitIncreaseGranted.LimitIncreaseAmount}", result.Description);
    }

    [Fact]
    public void Transform_WhenLimitIncreaseRejected_ShouldReturnAccountHistory()
    {
        var rejectedAt = DateTimeProvider.UtcNow;
        var limitIncreaseRejected = new LimitIncreaseRejected(default, rejectedAt);
        var @event = new Event<LimitIncreaseRejected>(limitIncreaseRejected);       
        var sut = new LoanAccountHistoryTransformation();

        var result = sut.Transform(@event);

        Assert.Equal($"Limit increase has been rejected at: {rejectedAt}", result.Description);
    }
}
