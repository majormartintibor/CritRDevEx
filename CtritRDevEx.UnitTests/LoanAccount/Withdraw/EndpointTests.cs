using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using static CritRDevEx.API.LoanAccount.Write.Withdraw.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.Withdraw;

public class EndpointTests
{
    [Fact]
    public void WithdrawalEmitsMoneyWithdrawn()
    {
        WithdrawFromLoanAccountCommand command = new(Guid.NewGuid(), 100);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, -1000, -500, LoanAccountStatus.Default, default);

        var @event = WithdrawFromAccount(command, account);
       
        Assert.IsType<MoneyWithdrawn>(@event);
        Assert.Equal(100, @event.Amount);
    }    
}
