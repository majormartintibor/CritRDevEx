using Alba;
using CritRDevEx.API.LoanAccount.Read.Details;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
internal static class GetDetails
{
    private static Task<IScenarioResult> QueryLoanAccountDetails(
        this IAlbaHost api,
        Guid accountId       
    ) =>
        api.Scenario(x =>
        {
            x.Get.Url("/api/loanAccount/detail/" + accountId);

            x.IgnoreStatusCode();
        });

    public static async Task<LoanAccountDetail> GetLoanAccountDetails(
        this IAlbaHost api,
        Guid accountId
    )
    {
        var result = await api.QueryLoanAccountDetails(accountId);

        var updated = await result.ReadAsJsonAsync<LoanAccountDetail>();       

        return updated;
    }
}
