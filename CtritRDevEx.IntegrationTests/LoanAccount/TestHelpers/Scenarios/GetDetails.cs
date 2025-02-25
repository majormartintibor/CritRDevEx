using Alba;
using CritRDevEx.API.LoanAccount.Details;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
internal static class GetDetails
{
    public static Task<IScenarioResult> GetLoanAccountDetails(
        this IAlbaHost api,
        Guid inspectionId       
    ) =>
        api.Scenario(x =>
        {
            x.Get.Url("/api/loanAccount/detail/" + inspectionId);

            x.IgnoreStatusCode();
        });

    public static async Task<LoanAccountDetail> LoanAccountDetailsShouldBe(
        this IAlbaHost api,
        LoanAccountDetail loanAccount        
    )
    {
        var result = await api.GetLoanAccountDetails(loanAccount.Id);

        var updated = await result.ReadAsJsonAsync<LoanAccountDetail>();       

        return updated;
    }
}
