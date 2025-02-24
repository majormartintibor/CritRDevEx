using Alba;
using static CritRDevEx.API.LoanAccount.CreateAccount.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;

internal static class CreateAccount
{
    internal static Task<IScenarioResult> CreateLoanAccount(
        this IAlbaHost api) =>
            api.Scenario(x =>
            {
                x.Post.Url(CreateLoanAccountEndpoint);
                x.Post.Json(new CreateLoanAccount(Guid.NewGuid()));

                x.StatusCodeShouldBeOk();
            });

    internal static Task<IScenarioResult> CreateLoanAccount(
        this IAlbaHost api,
        Guid debtorId) =>
            api.Scenario(x =>
            {
                x.Post.Url(CreateLoanAccountEndpoint);
                x.Post.Json(new CreateLoanAccount(debtorId));

                x.StatusCodeShouldBeOk();
            });

    internal static async Task<Guid> CreatedAccount(
        this IAlbaHost api)
    {
        var result = await api.CreateLoanAccount();

        return await result.ReadAsJsonAsync<Guid>();
    }
    internal static async Task<Guid> CreatedAccount(
        this IAlbaHost api,
        Guid debtorId)
    {
        var result = await api.CreateLoanAccount(debtorId);

        return await result.ReadAsJsonAsync<Guid>();
    }
}
