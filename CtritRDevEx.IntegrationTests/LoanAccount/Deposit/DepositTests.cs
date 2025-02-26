using Alba;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
using CtritRDevEx.IntegrationTests.TestHelpers;
using JasperFx.Core;
using Marten.Events;

namespace CtritRDevEx.IntegrationTests.LoanAccount.Deposit;

public class DepositTests(AppFixture fixture) : IntegrationContext(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task DepositSucceeds()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 5000;        
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);

        IScenarioResult result = await _fixture.Host!.SendDepositRequest(accountId, amount);
        Assert.Equal(200, result.Context.Response.StatusCode);
    }

    [Fact]
    public async Task DepositIncreasesBalance()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 5000;
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);

        _ = await _fixture.Host!.SendDepositRequest(accountId, amount);
                
        await _fixture.Host!.WaitForNonStaleProjectionDataAsync(Wait.ForAsyncProjectionUpdateTime);
        var updated = await _fixture.Host!.GetLoanAccountDetails(accountId);
        Assert.NotNull(updated);
        Assert.Equal(amount, updated.Balance);
    }

    [Fact]
    public async Task DepositingToBlockedAccountFails()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 5000;
        await Store.BlockedAccount(accountId);

        IScenarioResult result = await _fixture.Host!.SendDepositRequest(accountId, amount);
        Assert.Equal(500, result.Context.Response.StatusCode);
    }
}
