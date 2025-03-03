using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using CritRDevEx.API.LoanAccount.Write.AuditLimitIncreaseRequest;
using static CritRDevEx.API.LoanAccount.Write.AuditLimitIncreaseRequest.AuditLimitIncreaseRequestHandler;

namespace CtritRDevEx.UnitTests.LoanAccount.AuditLimitIncreaseRequest;

public class HandlerTests
{
    [Fact]
    public void Handle_WhenAccountIsBlocked_ShouldReturnLimitIncreaseRejectedEvent()
    {
        var account = new CritRDevEx.API.LoanAccount.Write.LoanAccount
        {
            AccountStatus = LoanAccountStatus.Blocked,
            Limit = -1000
        };
        var request = new AuditLimitIncreaseRequestHandler.AuditLimitIncreaseRequest(default, 1000);

        var (events, _) = Handle(request, account);

        Assert.Single(events);
        Assert.IsType<LimitIncreaseRejected>(events.First());
    }

    [Fact]
    public void Handle_WhenLifetimeDepositsIsLessThanThreeTimesLimit_ShouldReturnLimitIncreaseRejectedEvent()
    {
        var account = new CritRDevEx.API.LoanAccount.Write.LoanAccount
        {
            AccountStatus = LoanAccountStatus.Default,
            Limit = -1000
        };
        var request = new AuditLimitIncreaseRequestHandler.AuditLimitIncreaseRequest(default, 2000);

        var (events, _) = Handle(request, account);

        Assert.Single(events);
        Assert.IsType<LimitIncreaseRejected>(events.First());
    }

    [Fact]
    public void Handle_WhenLifetimeDepositsIsThreeTimesLimit_ShouldReturnLimitIncreaseGrantedEvent()
    {
        var account = new CritRDevEx.API.LoanAccount.Write.LoanAccount
        {
            AccountStatus = LoanAccountStatus.Default,
            Limit = -1000
        };
        var request = new AuditLimitIncreaseRequestHandler.AuditLimitIncreaseRequest(default, 3000);

        var (events, _) = Handle(request, account);

        Assert.Single(events);
        Assert.IsType<LimitIncreaseGranted>(events.First());
    }
}
