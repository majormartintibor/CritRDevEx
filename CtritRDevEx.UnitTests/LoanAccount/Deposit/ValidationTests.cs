using CritRDevEx.API.LoanAccount;
using Microsoft.AspNetCore.Http;
using static CritRDevEx.API.LoanAccount.Write.Deposit.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.Deposit;

public class ValidationTests
{
    [Fact]
    public void CannotDepositToBlockedAccount()
    {
        DepositToLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Blocked, default);

        var result = Validate(account);

        Assert.Equal(result.Status, StatusCodes.Status412PreconditionFailed);
        Assert.Equal("Account is blocked", result.Detail);
    }
}
