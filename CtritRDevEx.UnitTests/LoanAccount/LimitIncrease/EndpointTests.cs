using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.Write.LimitIncrease.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.LimitIncrease;

public class EndpointTests
{
    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAllConditionsAreMet_ReturnsOkResult()
    {
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-31));
        var command = new RequestLimitIncreaseCommand(default);

        var (result, _, _) = ReceiveLimitIncreaseRequest(command, account);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAllConditionsAreMet_ReturnsLimitIncreaseRequestedEvent()
    {
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-31));
        var command = new RequestLimitIncreaseCommand(default);

        var (_, events, _) = ReceiveLimitIncreaseRequest(command, account);

        Assert.Single(events);
        Assert.IsType<LimitIncreaseRequested>(events[0]);
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAllConditionsAreMet_ReturnsNoOutgoingMessages()
    {
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-31));
        var command = new RequestLimitIncreaseCommand(default);

        var (_, _, outgoingMessages) = ReceiveLimitIncreaseRequest(command, account);

        Assert.Empty(outgoingMessages);
    }
}
