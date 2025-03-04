﻿using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using CritRDevEx.API.LoanAccount.Read.Details;
using Marten.Events;

namespace CtritRDevEx.UnitTests.LoanAccount.Details;

public class LoanAccountDetailTests
{
    [Fact]
    public void Apply_WhenLoanAccountCreatedEvent_ShouldReturnAccountDetailWithInitialLimit()
    {
        var initialAccountDetail = new LoanAccountDetail();
        var initialLimit = -30000m;
        var loanAccountCreated = new LoanAccountCreated(default, initialLimit, default);
        var @event = new Event<LoanAccountEvent>(loanAccountCreated);

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(initialLimit, updatedAccountDetail.Limit);
        Assert.Equal(0, updatedAccountDetail.Balance);
        Assert.Equal(LoanAccountStatus.Default, updatedAccountDetail.AccountStatus);
    }

    [Fact]
    public void Apply_WhenMoneyDepositedEvent_ShouldReturnAccountDetailWithIncreasedBalance()
    {
        var initialAccountDetail = new LoanAccountDetail();
        var initialBalance = -30000m;
        var depositAmount = 500m;
        var moneyDeposited = new MoneyDeposited(default, depositAmount, default);
        var @event = new Event<LoanAccountEvent>(moneyDeposited);
        initialAccountDetail = initialAccountDetail with { Balance = initialBalance };

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(initialBalance + depositAmount, updatedAccountDetail.Balance);
    }

    [Fact]
    public void Apply_WhenMoneyWithdrawnEvent_ShouldReturnAccountDetailWithDecreasedBalance()
    {
        var initialAccountDetail = new LoanAccountDetail();
        var initialBalance = -30000m;
        var withdrawAmount = 500m;
        var moneyWithdrawn = new MoneyWithdrawn(default, withdrawAmount, default);
        var @event = new Event<LoanAccountEvent>(moneyWithdrawn);
        initialAccountDetail = initialAccountDetail with { Balance = initialBalance };

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(initialBalance - withdrawAmount, updatedAccountDetail.Balance);
    }

    [Fact]
    public void Apply_WhenLoanAccountBlockedEvent_ShouldReturnAccountDetailWithBlockedStatus()
    {
        var initialAccountDetail = new LoanAccountDetail();
        var loanAccountBlocked = new LoanAccountBlocked(default, default);
        var @event = new Event<LoanAccountEvent>(loanAccountBlocked);

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(LoanAccountStatus.Blocked, updatedAccountDetail.AccountStatus);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGrantedEvent_ShouldReturnAccountDetailWithDecreasedLimit()
    {
        var initialAccountDetail = new LoanAccountDetail();
        var initialLimit = -30000m;
        var limitIncreaseAmount = 500m;
        var limitIncreaseGranted = new LimitIncreaseGranted(default, limitIncreaseAmount, default);
        var @event = new Event<LoanAccountEvent>(limitIncreaseGranted);
        initialAccountDetail = initialAccountDetail with { Limit = initialLimit };

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(initialLimit - limitIncreaseAmount, updatedAccountDetail.Limit);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGrantedEvent_ShouldReturnAccountDetailWithLastLimitEvaluationDate()
    {
        var now = DateTimeProvider.UtcNow;

        var initialAccountDetail = new LoanAccountDetail();
        var initialLimit = -30000m;
        var limitIncreaseGranted = new LimitIncreaseGranted(default, default, now);
        var @event = new Event<LoanAccountEvent>(limitIncreaseGranted);
        initialAccountDetail = initialAccountDetail with { Limit = initialLimit };

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(now, updatedAccountDetail.LastLimitEvaluationDate);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRejectedEvent_ShouldReturnAccountDetailWithLastLimitEvaluationDate()
    {
        var now = DateTimeProvider.UtcNow;

        var initialAccountDetail = new LoanAccountDetail();       
        var limitIncreaseRejected = new LimitIncreaseRejected(default, now);
        var @event = new Event<LoanAccountEvent>(limitIncreaseRejected);        

        var updatedAccountDetail = initialAccountDetail.Apply(@event);

        Assert.Equal(now, updatedAccountDetail.LastLimitEvaluationDate);
    }
}
