using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.LimitIncrease.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.LimitIncrease;

public class EndpointTests
{
    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAccountIsBlocked_ThrowsInvalidOperationException()
    {
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Blocked, default);

        var request = new RequestLimitIncrease(default);

        Assert.Throws<InvalidOperationException>(() => ReceiveLimitIncreaseRequest(request, account));
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenLimitIncreaseRequestIsPending_ThrowsInvalidOperationException()
    {
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, default);
        account = account with { HasPendingLimitIncreaseRequest = true };
        var request = new RequestLimitIncrease(default);
        
        Assert.Throws<InvalidOperationException>(() => ReceiveLimitIncreaseRequest(request, account));
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenLastLimitEvaluationDateIsWithin30Days_ThrowsInvalidOperationException()
    {
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-29));
        var request = new RequestLimitIncrease(default);

        Assert.Throws<InvalidOperationException>(() => ReceiveLimitIncreaseRequest(request, account));
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAllConditionsAreMet_ReturnsOkResult()
    {
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-31));
        var request = new RequestLimitIncrease(default);

        var (result, _, _) = ReceiveLimitIncreaseRequest(request, account);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAllConditionsAreMet_ReturnsLimitIncreaseRequestedEvent()
    {
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-31));
        var request = new RequestLimitIncrease(default);

        var (_, events, _) = ReceiveLimitIncreaseRequest(request, account);

        Assert.Single(events);
        Assert.IsType<LimitIncreaseRequested>(events[0]);
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAllConditionsAreMet_ReturnsNoOutgoingMessages()
    {
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-31));
        var request = new RequestLimitIncrease(default);

        var (_, _, outgoingMessages) = ReceiveLimitIncreaseRequest(request, account);

        Assert.Empty(outgoingMessages);
    }
}
