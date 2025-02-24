using Alba;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;
using CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
using JasperFx.Core;
using static CritRDevEx.API.LoanAccount.CreateAccount.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.CreateAccount;

public class CreateAccountTests(AppFixture fixture) : IntegrationContext(fixture)
{
    private readonly AppFixture _fixture = fixture;

    [Fact]
    public async Task CreateAccount_Succeeds()
    {
        var initial = await _fixture.Host!.CreateLoanAccount();

        var response = initial.ReadAsJson<Guid>();
        Assert.NotEqual(Guid.Empty, response);
    }

    [Fact]
    public async Task CreateAccountFails_WhenDebtorHasExistingAccount()
    {
        var debtorId = CombGuidIdGeneration.NewGuid();
        await Store.AccountForDebtorExists(debtorId);        

        IScenarioResult result = await _fixture.Host!.CreateLoanAccount(debtorId);

        Assert.Equal(412, result.Context.Response.StatusCode);
    }
}
