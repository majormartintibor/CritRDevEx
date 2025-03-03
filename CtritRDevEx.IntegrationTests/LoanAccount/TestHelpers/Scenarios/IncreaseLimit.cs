using Alba;
using static CritRDevEx.API.LoanAccount.Write.LimitIncrease.Endpoint;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Scenarios;

internal static class IncreaseLimit
{
    internal static Task<IScenarioResult> SendLimitIncreaseRequest(
        this IAlbaHost api,
        Guid accountId) =>
            api.Scenario(x =>
            {
                x.Post.Url(RequestLimitIncreaseEndpoint);
                x.Post.Json(new RequestLimitIncrease(accountId));

                x.IgnoreStatusCode();
            });
}
