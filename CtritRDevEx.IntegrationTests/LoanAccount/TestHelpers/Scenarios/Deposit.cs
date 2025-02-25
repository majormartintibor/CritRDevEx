using Alba;
using static CritRDevEx.API.LoanAccount.Deposit.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;
internal static class Deposit
{
    internal static Task<IScenarioResult> SendDepositRequest(
        this IAlbaHost api,
        Guid accountId,
        decimal amount) =>
            api.Scenario(x =>
            {
                x.Post.Url(DepositToLoanAccountEndpoint);
                x.Post.Json(new DepositToLoanAccount(accountId, amount));

                x.IgnoreStatusCode();
            });
}
