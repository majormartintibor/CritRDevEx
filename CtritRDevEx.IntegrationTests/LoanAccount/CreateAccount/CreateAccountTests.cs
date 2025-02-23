using static CritRDevEx.API.LoanAccount.CreateAccount.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.CreateAccount;

public class CreateAccountTests(AppFixture fixture) : IntegrationContext(fixture)
{
    [Fact]
    public async Task CreateAccount_Succeeds()
    {
        CreateLoanAccount request = new(Guid.NewGuid());

        // Log a new incident first
        var initial = await Scenario(x =>
        {
            x.Post.Json(request).ToUrl(CreateLoanAccountEndpoint);
            x.StatusCodeShouldBe(200);
        });

        var response = initial.ReadAsJson<Guid>();

        Assert.NotEqual(Guid.Empty, response);
    }
}
