using CritRDevEx.API.LoanAccount;
using Microsoft.AspNetCore.Http;
using static CritRDevEx.API.LoanAccount.Write.LimitIncrease.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.LimitIncrease;

public class ValidationTests
{
    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenAccountIsBlocked_ThrowsInvalidOperationException()
    {
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Blocked, default);
        
        var result = Validate(account);

        Assert.Equal(result.Status, StatusCodes.Status412PreconditionFailed);
        Assert.Equal("Account is blocked", result.Detail);
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenLimitIncreaseRequestIsPending_ThrowsInvalidOperationException()
    {
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, default);
        account = account with { HasPendingLimitIncreaseRequest = true };

        var result = Validate(account);

        Assert.Equal(result.Status, StatusCodes.Status412PreconditionFailed);
        Assert.Equal("Limit increase request is already pending", result.Detail);
    }

    [Fact]
    public void ReceiveLimitIncreaseRequest_WhenLastLimitEvaluationDateIsWithin30Days_ThrowsInvalidOperationException()
    {
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, default, default, LoanAccountStatus.Default, DateTimeOffset.UtcNow.AddDays(-29));

        var result = Validate(account);

        Assert.Equal(result.Status, StatusCodes.Status412PreconditionFailed);
        Assert.Equal("Limit increase can be requested only once in 30 days", result.Detail);
    }
}
