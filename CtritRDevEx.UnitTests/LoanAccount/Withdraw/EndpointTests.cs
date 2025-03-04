using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.Write.Withdraw.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.Withdraw;

public class EndpointTests
{
    [Fact]
    public void WithdrawalSucceeds()
    {
        WithdrawFromLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var (result, _, _) = WithdrawFromAccount(command, account);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void WithdrawalEmitsMoneyWithdrawn()
    {
        WithdrawFromLoanAccountCommand command = new(Guid.NewGuid(), 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var (_, events, _) = WithdrawFromAccount(command, account);

        Assert.Single(events);
        Assert.IsType<MoneyWithdrawn>(events[0]);
    }

    [Fact]
    public void WithdrawalEmitsNoOutgoingMessages()
    {
        WithdrawFromLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var (_, _, outgoingMessages) = WithdrawFromAccount(command, account);

        Assert.Empty(outgoingMessages);
    }


    [Fact]
    public void WithdrawalAmountExceedsAccountLimitThrowsException()
    {
        WithdrawFromLoanAccountCommand command = new(default, 1000);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);
        
        Assert.Throws<InvalidOperationException>(() => WithdrawFromAccount(command, account));
    }

    [Fact]
    public void CannotWithdrawFromBlockedAccount()
    {
        WithdrawFromLoanAccountCommand command = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Blocked, default);

        Assert.Throws<InvalidOperationException>(() => WithdrawFromAccount(command, account));
    }
}
