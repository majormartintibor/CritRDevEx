using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.Deposit.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.Deposit;

public class EndpointTests
{
    [Fact]
    public void DepositSucceeds()
    {
        DepositToLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);

        var (result, _, _) = DepositToAccount(request, account);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void DepositEmitsMoneyDeposited()
    {
        DepositToLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);
        
        var (_, events, _) = DepositToAccount(request, account);

        Assert.Single(events);
        Assert.IsType<MoneyDeposited>(events[0]);
    }

    [Fact]
    public void DepositEmitsNoOutgoingMessages()
    {
        DepositToLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);

        var (_, _, outgoingMessages) = DepositToAccount(request, account);

        Assert.Empty(outgoingMessages);
    }

    [Fact]
    public void CannotDepositToBlockedAccount()
    {
        DepositToLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Blocked, default);

        Assert.Throws<InvalidOperationException>(() => DepositToAccount(request, account));
    }
}
