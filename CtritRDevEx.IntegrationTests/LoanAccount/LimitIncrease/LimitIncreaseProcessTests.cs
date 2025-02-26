using CritRDevEx.API.LoanAccount;
using CritRDevEx.API.LoanAccount.Details;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
using CtritRDevEx.IntegrationTests.TestHelpers;
using JasperFx.Core;
using Marten.Events;

namespace CtritRDevEx.IntegrationTests.LoanAccount.LimitIncrease;

public class LimitIncreaseProcessTests(AppFixture fixture) : IntegrationContext(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task LimitIncreaseProcessCompletesSuccessfully()
    {
        var accountId = CombGuidIdGeneration.NewGuid();
        var amount = 100000;
        await Store.AccountWithLastLimitEvaluationDateOlderThanThirtyDaysExist(accountId);
        await Store.AddDeposit(accountId, amount);

        _ = await _fixture.Host!.SendLimitIncreaseRequest(accountId);

        //could be more sophisticated, e.g: polling if the event exists
        await Task.Delay(Wait.ForProcessorCycle);
        await _fixture.Host!.WaitForNonStaleProjectionDataAsync(Wait.ForAsyncProjectionUpdateTime);
        LoanAccountDetail expected = new(accountId, -40000, 100000, LoanAccountStatus.Default);        
        LoanAccountDetail updated = await _fixture.Host!.GetLoanAccountDetails(expected.Id);
        Assert.NotNull(updated);
        Assert.Equal(expected, updated);
    }
}
