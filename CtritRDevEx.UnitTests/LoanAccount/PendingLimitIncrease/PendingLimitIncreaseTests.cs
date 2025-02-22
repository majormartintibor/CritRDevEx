using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.PendingLimitIncrease;

namespace CtritRDevEx.UnitTests.LoanAccount.PendingLimitIncrease;
public class PendingLimitIncreaseTests
{
    [Fact]
    public void Apply_WhenLimitIncreaseRequested_ShouldReturnRequestWithPendingStatus()
    {
        PendigLimitIncreaseRequest pendingLimitIncreaseRequest = new();
        LimitIncreaseRequested @event = new(default);
                
        var result = pendingLimitIncreaseRequest.Apply(@event);
                
        Assert.Equal(LimitIncreaseRequestStatus.Pending, result.Status);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseGranted_ShouldReturnRequestWithGrantedStatus()
    {
        PendigLimitIncreaseRequest pendingLimitIncreaseRequest = new();
        LimitIncreaseGranted @event = new(default, default);

        var result = pendingLimitIncreaseRequest.Apply(@event);

        Assert.Equal(LimitIncreaseRequestStatus.Granted, result.Status);
    }

    [Fact]
    public void Apply_WhenLimitIncreaseRejected_ShouldReturnRequestWithRejectedStatus()
    {
        PendigLimitIncreaseRequest pendingLimitIncreaseRequest = new();
        LimitIncreaseRejected @event = new(default);

        var result = pendingLimitIncreaseRequest.Apply(@event);

        Assert.Equal(LimitIncreaseRequestStatus.Rejected, result.Status);
    }
}
