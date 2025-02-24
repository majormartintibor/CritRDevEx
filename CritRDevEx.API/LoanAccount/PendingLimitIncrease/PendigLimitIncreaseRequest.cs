using CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using Marten.Events.Aggregation;

namespace CritRDevEx.API.LoanAccount.PendingLimitIncrease;

public sealed record PendigLimitIncreaseRequest(
    Guid Id, 
    LimitIncreaseRequestStatus Status)
{
    public PendigLimitIncreaseRequest() 
        : this(Guid.Empty, LimitIncreaseRequestStatus.NoRequest)
    {        
    }

    public PendigLimitIncreaseRequest Apply(LoanAccountEvent @event) =>
        @event switch
        {
            LimitIncreaseRequested(Guid, DateTimeOffset) =>
                this with { Status = LimitIncreaseRequestStatus.Pending },

            LimitIncreaseGranted(Guid, decimal, DateTimeOffset) =>
                this with { Status =  LimitIncreaseRequestStatus.NoRequest },

            LimitIncreaseRejected(Guid, DateTimeOffset) =>
                this with { Status = LimitIncreaseRequestStatus.NoRequest },

            _ => this
        };
}

public sealed class PendingLimitIncreaseRequestProjection 
    : SingleStreamProjection<PendigLimitIncreaseRequest>
{
    public PendigLimitIncreaseRequest Apply(LoanAccountEvent @event, PendigLimitIncreaseRequest current)
        => current.Apply(@event);
}
