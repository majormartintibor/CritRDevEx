using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.PendingLimitIncrease;
using Marten;
using Quartz;
using Wolverine;
using static CritRDevEx.API.LoanAccount.LimitIncrease.AuditLimitIncreaseRequestHandler;

namespace CritRDevEx.API.LoanAccount.LimitIncreaseProcessor;

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
            var lifetimeDeposit = await GetLifetimeDepositAmount(request.AccountId, session);

            var command = new AuditLimitIncreaseRequest(request.AccountId, lifetimeDeposit);
            //invoke vs send:
            //send is fire and forget
            //invoke is fire and wait for response
            //we want to wait for the completion of the command handling
            //this is in-process communication, GOF mediator pattern
            await _bus.InvokeAsync(command);
        }
    }

    private static async Task<decimal> GetLifetimeDepositAmount(Guid accountId, IQuerySession session)
    {
        var totalAmount = await session.Events.QueryRawEventDataOnly<MoneyDeposited>()
                                              .Where(e => e.AccountId == accountId)
                                              .SumAsync(e => e.Amount);

        return totalAmount;
    }
}