using CritRDevEx.API.LoanAccount.History;
using CritRDevEx.API.LoanAccount.PendingLimitIncrease;
using Marten;
using Marten.Events.Projections;

namespace CritRDevEx.API.LoanAccount;

public static class Configuration
{
    public static void AddLoanAccountProjections(this StoreOptions options)
    {
        options.Projections.LiveStreamAggregation<LoanAccount>();
        options.Projections.Add<PendingLimitIncreaseRequestProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<LoanAccountHistoryTransformation>(ProjectionLifecycle.Async);
    }
}
