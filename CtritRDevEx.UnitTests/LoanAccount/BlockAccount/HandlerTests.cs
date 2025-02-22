using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.BlockAccount;
using static CritRDevEx.API.LoanAccount.BlockAccount.BlockAccountHandler;

namespace CtritRDevEx.UnitTests.LoanAccount.BlockAccount;

public class HandlerTests
{
    [Fact]
    public void BlockAccountEmitsAccountBlocked()
    {
        BlockAccountHandler.BlockAccount request = new(default);
        Account account = new(default, default, 1000, 500, AccountStatus.Default, default);

        var (events, _) = Handle(request, account);

        Assert.Single(events);
        Assert.IsType<AccountBlocked>(events[0]);
    }

    [Fact]
    public void BlockAccountEmitsNoOutgoingMessages()
    {
        BlockAccountHandler.BlockAccount request = new(default);
        Account account = new(default, default, 1000, 500, AccountStatus.Default, default);

        var (_, outgoingMessages) = Handle(request, account);

        Assert.Empty(outgoingMessages);
    }

    [Fact]
    public void CannotBlockAlreadyBlockedAccount()
    {
        BlockAccountHandler.BlockAccount request = new(default);

        Account account = new(default, default, 1000, 500, AccountStatus.Blocked, default);

        Assert.Throws<InvalidOperationException>(() => Handle(request, account));
    }
}
