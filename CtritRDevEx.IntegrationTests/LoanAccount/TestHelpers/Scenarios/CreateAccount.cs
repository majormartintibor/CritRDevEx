using Alba;
using static CritRDevEx.API.LoanAccount.Write.CreateAccount.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;

internal static class CreateAccount
{
    internal static Task<IScenarioResult> CreateLoanAccount(
        this IAlbaHost api) =>
            api.Scenario(x =>
            {
                x.Post.Url(CreateLoanAccountEndpoint);
                x.Post.Json(new CreateLoanAccountCommand(Guid.NewGuid()));

                x.IgnoreStatusCode();
            });

    internal static Task<IScenarioResult> CreateLoanAccount(
        this IAlbaHost api,
        Guid debtorId) =>
            api.Scenario(x =>
            {
                x.Post.Url(CreateLoanAccountEndpoint);
                x.Post.Json(new CreateLoanAccountCommand(debtorId));

                x.IgnoreStatusCode();
            });    
}
