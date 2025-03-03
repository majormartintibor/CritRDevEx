using Alba;
using static CritRDevEx.API.LoanAccount.Write.Withdraw.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;

internal static class Withdraw
{
    internal static Task<IScenarioResult> SendWithdrawRequest(
        this IAlbaHost api,
        Guid accountId,
        decimal amount) =>
            api.Scenario(x =>
            {
                x.Post.Url(WithdrawFromLoanAccountEndpoint);
                x.Post.Json(new WithdrawFromLoanAccount(accountId, amount));

                x.IgnoreStatusCode();
            });
}
