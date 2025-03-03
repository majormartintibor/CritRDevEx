using CritRDevEx.API.LoanAccount.Read.Details;
using CritRDevEx.API.LoanAccount.Read.History;
using CritRDevEx.API.LoanAccount.Read.PendingLimitIncrease;
using Marten;
using Marten.Events.Projections;

namespace CritRDevEx.API.LoanAccount;

public static class Configuration
{
    public static void AddLoanAccountProjections(this StoreOptions options)
    {
        //Load the fresh state of the event stream on demand, no snapshotting by default.
        options.Projections.LiveStreamAggregation<Write.LoanAccount>();        
        //Immediate consistency, current state stored in a seperate table for high read performance.
        options.Projections.Add<PendingLimitIncreaseRequestProjection>(ProjectionLifecycle.Inline);
        //Eventual consistency, async daemon building the current state in the background.
        //Very performant for the write side.
        options.Projections.Add<LoanAccountHistoryTransformation>(ProjectionLifecycle.Async);
        options.Projections.Add<LoanAccountDetailProjection>(ProjectionLifecycle.Async);
    }
}
