using CritRDevEx.API.LoanAccount.LimitIncrease;
using Marten.Events.Aggregation;

namespace CritRDevEx.API.LoanAccount.PendingLimitIncrease;

public sealed record PendigLimitIncreaseRequest(
    Guid LoanAccountId, 
    LimitIncreaseRequestStatus Status)
{
    public PendigLimitIncreaseRequest() 
        : this(Guid.Empty, LimitIncreaseRequestStatus.Pending)
    {        
    }

    public PendigLimitIncreaseRequest Apply(LoanAccountEvent @event) =>
        @event switch
        {
            LimitIncreaseRequested(Guid) =>
                this with { Status = LimitIncreaseRequestStatus.Pending },

            LimitIncreaseGranted(Guid, decimal) =>
                this with { Status =  LimitIncreaseRequestStatus.Granted },

            LimitIncreaseRejected(Guid) =>
                this with { Status = LimitIncreaseRequestStatus.Rejected },

            _ => this
        };
}

public sealed class PendingLimitIncreaseRequestProjection 
    : SingleStreamProjection<PendigLimitIncreaseRequest>
{
    public PendigLimitIncreaseRequest Apply(LoanAccountEvent @event, PendigLimitIncreaseRequest current)
        => current.Apply(@event);
}
