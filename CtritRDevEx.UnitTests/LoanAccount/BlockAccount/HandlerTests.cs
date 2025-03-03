using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using static CritRDevEx.API.LoanAccount.Write.BlockAccount.BlockLoanAccountHandler;

namespace CtritRDevEx.UnitTests.LoanAccount.BlockAccount;

public class HandlerTests
{
    [Fact]
    public void BlockAccountEmitsAccountBlocked()
    {
        BlockLoanAccount request = new(default);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);

        var (events, _) = Handle(request, account);

        Assert.Single(events);
        Assert.IsType<LoanAccountBlocked>(events[0]);
    }

    [Fact]
    public void BlockAccountEmitsNoOutgoingMessages()
    {
        BlockLoanAccount request = new(default);
        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Default, default);

        var (_, outgoingMessages) = Handle(request, account);

        Assert.Empty(outgoingMessages);
    }

    [Fact]
    public void CannotBlockAlreadyBlockedAccount()
    {
        BlockLoanAccount request = new(default);

        CritRDevEx.API.LoanAccount.Write.LoanAccount account = new(default, default, 1000, 500, LoanAccountStatus.Blocked, default);

        Assert.Throws<InvalidOperationException>(() => Handle(request, account));
    }
}
