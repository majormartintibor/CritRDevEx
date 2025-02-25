using Alba;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
using JasperFx.Core;

namespace CtritRDevEx.IntegrationTests.LoanAccount.LimitIncrease;

public class LimitIncreaseEndpointTests(AppFixture fixture) : IntegrationContext(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task FailsWhenAccountIsBlocked()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        await Store.BlockedAccount(accountId);

        IScenarioResult result = await _fixture.Host!.SendLimitIncreaseRequest(accountId);

        Assert.Equal(500, result.Context.Response.StatusCode);
    }

    [Fact]
    public async Task FailsWhenHasPendingLimitIncreaseRequest()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        await Store.AccountWithPendingLimitIncreaseRequest(accountId);

        IScenarioResult result = await _fixture.Host!.SendLimitIncreaseRequest(accountId);

        Assert.Equal(500, result.Context.Response.StatusCode);
    }

    [Fact]
    public async Task FailsWhenLastLimitEvaluationDateIsLessThanThirtyDaysOld()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        await Store.AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(accountId);

        IScenarioResult result = await _fixture.Host!.SendLimitIncreaseRequest(accountId);

        Assert.Equal(500, result.Context.Response.StatusCode);        
    }

    [Fact]
    public async Task SucceedsWhenLastLimitEvaluationDateIsMoreThanThirtyDaysOld()
    {
        var accountId = CombGuidIdGeneration.NewGuid();        
        await Store.AccountWithLastLimitEvaluationDateOlderThanThirtyDaysExist(accountId);

        IScenarioResult result = await _fixture.Host!.SendLimitIncreaseRequest(accountId);

        Assert.Equal(200, result.Context.Response.StatusCode);
    }
}