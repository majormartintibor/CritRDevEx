using Alba;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;
using JasperFx.Core;

namespace CtritRDevEx.IntegrationTests.LoanAccount.Withdraw;

public class WithdrawTests(AppFixture fixture) : IntegrationContext(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task WithdrawSucceeds()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 5000;
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);

        IScenarioResult result = await _fixture.Host!.SendWithdrawRequest(accountId, amount);
        Assert.Equal(200, result.Context.Response.StatusCode);
    }

    [Fact]
    public async Task WithdrawReducesBalance()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 5000;
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);

        _ = await _fixture.Host!.SendWithdrawRequest(accountId, amount);

        var updated = await _fixture.Host!.GetLoanAccountDetails(accountId);
        Assert.Equal(-5000, updated.Balance);
    }

    [Fact]
    public async Task WithdrawFromBlockedAccountFails()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 5000;
        await Store.BlockedAccount(accountId);

        IScenarioResult result = await _fixture.Host!.SendWithdrawRequest(accountId, amount);
        Assert.Equal(500, result.Context.Response.StatusCode);
    }

    [Fact]
    public async Task OverdrawingFails()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 500000;
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);

        IScenarioResult result = await _fixture.Host!.SendWithdrawRequest(accountId, amount);
        Assert.Equal(500, result.Context.Response.StatusCode);
    }
}
