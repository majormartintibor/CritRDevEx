using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.Write.Deposit.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.Deposit;

public class EndpointTests
{
    [Fact]
    public void DepositSucceeds()
    {
        DepositToLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);

        var (result, _, _) = DepositToAccount(command, account);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void DepositEmitsMoneyDeposited()
    {
        DepositToLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);
        
        var (_, events, _) = DepositToAccount(command, account);

        Assert.Single(events);
        Assert.IsType<MoneyDeposited>(events[0]);
    }

    [Fact]
    public void DepositEmitsNoOutgoingMessages()
    {
        DepositToLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);

        var (_, _, outgoingMessages) = DepositToAccount(command, account);

        Assert.Empty(outgoingMessages);
    }

    [Fact]
    public void CannotDepositToBlockedAccount()
    {
        DepositToLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Blocked, default);

        Assert.Throws<InvalidOperationException>(() => DepositToAccount(command, account));
    }
}
