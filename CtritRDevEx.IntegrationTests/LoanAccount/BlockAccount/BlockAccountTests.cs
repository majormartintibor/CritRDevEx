using CritRDevEx.API.LoanAccount;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
using CtritRDevEx.IntegrationTests.TestHelpers;
using JasperFx.Core;
using Marten.Events;
using Wolverine;
using static CritRDevEx.API.LoanAccount.BlockAccount.BlockLoanAccountHandler;

namespace CtritRDevEx.IntegrationTests.LoanAccount.BlockAccount;

public class BlockAccountTests(AppFixture fixture) : IntegrationContext(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task BlockingAccountSucceeds()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);
        BlockLoanAccount message = new(accountId);

        await _fixture.Host!.InvokeAsync(message);

        await _fixture.Host!.WaitForNonStaleProjectionDataAsync(Wait.ForAsyncProjectionUpdateTime);
        var updated = await _fixture.Host!.GetLoanAccountDetails(accountId);
        Assert.NotNull(updated);
        Assert.Equal(LoanAccountStatus.Blocked, updated.AccountStatus);
    }

    [Fact]
    public async Task BlockingAnAlreadyBlockedAccountFails()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        await Store.BlockedAccount(accountId);
        BlockLoanAccount message = new(accountId);

        _ = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _fixture.Host!.InvokeAsync(message));
    }
}
