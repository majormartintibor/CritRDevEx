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
        WithdrawFromLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var (result, _, _) = WithdrawFromAccount(request, account);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void WithdrawalEmitsMoneyWithdrawn()
    {
        WithdrawFromLoanAccount request = new(Guid.NewGuid(), 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var (_, events, _) = WithdrawFromAccount(request, account);

        Assert.Single(events);
        Assert.IsType<MoneyWithdrawn>(events[0]);
    }

    [Fact]
    public void WithdrawalEmitsNoOutgoingMessages()
    {
        WithdrawFromLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var (_, _, outgoingMessages) = WithdrawFromAccount(request, account);

        Assert.Empty(outgoingMessages);
    }


    [Fact]
    public void WithdrawalAmountExceedsAccountLimitThrowsException()
    {
        WithdrawFromLoanAccount request = new(default, 1000);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);
        
        Assert.Throws<InvalidOperationException>(() => WithdrawFromAccount(request, account));
    }

    [Fact]
    public void CannotWithdrawFromBlockedAccount()
    {
        WithdrawFromLoanAccount request = new(default, 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Blocked, default);

        Assert.Throws<InvalidOperationException>(() => WithdrawFromAccount(request, account));
    }
}
