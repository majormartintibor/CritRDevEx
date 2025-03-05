using CritRDevEx.API.LoanAccount;
using Microsoft.AspNetCore.Http;
using static CritRDevEx.API.LoanAccount.Write.Withdraw.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.Withdraw;

public class ValidationTests
{
    [Fact]
    public void WithdrawalAmountExceedsAccountLimitReturnsProblemDetails()
    {
        WithdrawFromLoanAccountCommand command = new(default, 1000);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var result = Validate(command, account);

        Assert.Equal(result.Status, StatusCodes.Status412PreconditionFailed);
        Assert.Equal("Withdrawal amount exceeds account limit", result.Detail);
    }

    [Fact]
    public void WithdrawFromBlockedAccountLimitReturnsProblemDetails()
    {
        WithdrawFromLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Blocked, default);

        var result = Validate(command, account);

        Assert.Equal(result.Status, StatusCodes.Status412PreconditionFailed);
        Assert.Equal("Account is blocked", result.Detail);
    }
}
