using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using static CritRDevEx.API.LoanAccount.LimitIncrease.AuditLimitIncreaseRequestHandler;

namespace CtritRDevEx.UnitTests.LoanAccount.LimitIncrease;

public class HandlerTests
{
    [Fact]
    public void Handle_WhenAccountIsBlocked_ShouldReturnLimitIncreaseRejectedEvent()
    {        
        var account = new Account
        {
            AccountStatus = AccountStatus.Blocked,
            Limit = -1000
        };
        var request = new AuditLimitIncreaseRequest(default, 1000);
                
        var (events, _) = Handle(request, account);
                
        Assert.Single(events);
        Assert.IsType<LimitIncreaseRejected>(events.First());
    }

    [Fact]
    public void Handle_WhenLifetimeDepositsIsLessThanThreeTimesLimit_ShouldReturnLimitIncreaseRejectedEvent()
    {        
        var account = new Account
        {
            AccountStatus = AccountStatus.Default,
            Limit = -1000
        };
        var request = new AuditLimitIncreaseRequest(default, 2000);
        
        var (events, _) = Handle(request, account);
        
        Assert.Single(events);
        Assert.IsType<LimitIncreaseRejected>(events.First());
    }

    [Fact]
    public void Handle_WhenLifetimeDepositsIsThreeTimesLimit_ShouldReturnLimitIncreaseGrantedEvent()
    {
        var account = new Account
        {
            AccountStatus = AccountStatus.Default,
            Limit = -1000
        };
        var request = new AuditLimitIncreaseRequest(default, 3000);

        var (events, _) = Handle(request, account);

        Assert.Single(events);
        Assert.IsType<LimitIncreaseGranted>(events.First());
    }
}
