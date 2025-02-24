using Alba;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Fixtures;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;

namespace CtritRDevEx.IntegrationTests.LoanAccount.LimitIncrease;

public class FailsWhenLastLimitEvaluationDateIsLessThanThirtyDays(AppFixture fixture)
    : GivenAccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task RunTest()
    {
        IScenarioResult result = await _fixture.Host!.SendLimitIncreaseRequest(AccountId);
        Assert.Equal(500, result.Context.Response.StatusCode);
    }
}
