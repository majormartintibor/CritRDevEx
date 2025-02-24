using CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.PendingLimitIncrease;

namespace CtritRDevEx.UnitTests.LoanAccount.PendingLimitIncrease;
public class PendingLimitIncreaseTests
{
    [Fact]
    public void Apply_WhenLimitIncreaseRequested_ShouldReturnRequestWithPendingStatus()
    {
        PendigLimitIncreaseRequest pendingLimitIncreaseRequest = new();
        LimitIncreaseRequested @event = new(default, default);
                
        var result = pendingLimitIncreaseRequest.Apply(@event);
                
        Assert.Equal(LimitIncreaseRequestStatus.Pending, result.Status);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGranted_ShouldReturnRequestWithNoRequestStatus()
    {
        PendigLimitIncreaseRequest pendingLimitIncreaseRequest = new();
        LimitIncreaseGranted @event = new(default, default, default);

        var result = pendingLimitIncreaseRequest.Apply(@event);

        Assert.Equal(LimitIncreaseRequestStatus.NoRequest, result.Status);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRejected_ShouldReturnRequestWithNoRequestStatus()
    {
        PendigLimitIncreaseRequest pendingLimitIncreaseRequest = new();
        LimitIncreaseRejected @event = new(default, default);

        var result = pendingLimitIncreaseRequest.Apply(@event);

        Assert.Equal(LimitIncreaseRequestStatus.NoRequest, result.Status);
    }
}
