using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using CritRDevEx.API.LoanAccount.Read.PendingLimitIncrease;
using CritRDevEx.API.LoanAccount.Write.AuditLimitIncreaseRequest;
using Marten;
using Quartz;
using Wolverine;

namespace CritRDevEx.API.LoanAccount.Automations.LimitIncreaseProcessor;

/*
 * This is the top of the A-Frame, the processors job is to look at the todo list and the account
 * collect the data neeeded and push it into the business logic
 * hence there is no unit test for this class, testing belongs to the integration test level
 */
[DisallowConcurrentExecution]
public class PendingLimitIncreaseRequestProcessor(IDocumentStore documentStore, IMessageBus bus) : IJob
{
    private readonly IDocumentStore _documentStore = documentStore;
    private readonly IMessageBus _bus = bus;

    public async Task Execute(IJobExecutionContext context)
    {
        using var session = _documentStore.QuerySession();
        var pendingRequests = await session.Query<PendigLimitIncreaseRequest>()
                                           .Where(r => r.Status == LimitIncreaseRequestStatus.Pending)
                                           .ToListAsync();

        foreach (var request in pendingRequests)
        {
            var lifetimeDeposit = await GetLifetimeDepositAmount(request.Id, session);

            var command = new AuditLimitIncreaseCommandHandler.AuditLimitIncreaseCommand(request.Id, lifetimeDeposit);
            //invoke vs send:
            //send is fire and forget
            //invoke is fire and wait for response
            //we want to wait for the completion of the command handling           
            await _bus.InvokeAsync(command);
        }
    }

    private static async Task<decimal> GetLifetimeDepositAmount(Guid loanAccountId, IQuerySession session)
    {
        //should work fine if closing the books pattern is implemented
        //needs adjustment if snapshotting is used
        //var totalAmount = await session.Events.QueryRawEventDataOnly<MoneyDeposited>()
        //                                      .Where(e => e.LoanAccountId == loanAccountId)
        //                                      .SumAsync(e => e.Amount);

        //this should be more efficient than the code above
        var totalLifetimeDeposits = await session.Events.AggregateStreamAsync<TotalLifetimeDeposits>(loanAccountId);

        return totalLifetimeDeposits is null ? 0 : totalLifetimeDeposits!.Amount;
    }

    public sealed record TotalLifetimeDeposits(Guid Id, decimal Amount)
    {
        public TotalLifetimeDeposits() : this(Guid.Empty, 0)
        {
        }

        public TotalLifetimeDeposits Apply(LoanAccountEvent @event) =>
        @event switch
        {
            MoneyDeposited(Guid, decimal amount, DateTimeOffset) =>
                this with { Amount = Amount + amount },
            _ => this
        };
    }
}