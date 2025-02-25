using Alba;
using CritRDevEx.API.LoanAccount.History;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;

internal static class GetHistory
{
    private static Task<IScenarioResult> CallLoanAccountHistoryGetEndpoint(
        this IAlbaHost api,
        Guid accountId
    ) =>
        api.Scenario(x =>
        {
            x.Get.Url("/api/loanAccount/history/" + accountId);

            x.IgnoreStatusCode();
        });

    public static async Task<LoanAccountHistory> GetLoanAccountHistory(
        this IAlbaHost api,
        Guid accountId
        )
    {
        var result = await api.CallLoanAccountHistoryGetEndpoint(accountId);

        return await result.ReadAsJsonAsync<LoanAccountHistory>();
    }
}
