using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using JasperFx.Core;
using Marten.Events;
using Marten.Events.Projections;

namespace CritRDevEx.API.LoanAccount.Read.History;

public sealed record LoanAccountHistory(Guid Id, Guid LoanAccountId, string Description);

public sealed class LoanAccountHistoryTransformation : EventProjection
{
    public LoanAccountHistory Transform(IEvent<LoanAccountCreated> input)
    {
        var (_, initialLimit, createdAt) = input.Data;

        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"Account has been created at: {createdAt} with initial limit of: {initialLimit}");
    }

    public LoanAccountHistory Transform(IEvent<MoneyWithdrawn> input)
    {
        var (_, amount, transactionDate) = input.Data;

        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"{amount} has been withdrawn at: {transactionDate}");
    }

    public LoanAccountHistory Transform(IEvent<MoneyDeposited> input)
    {
        var (_, amount, transactionDate) = input.Data;

        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"{amount} has been deposited at: {transactionDate}");
    }

    public LoanAccountHistory Transform(IEvent<LoanAccountBlocked> input)
    {
        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"Account has been blocked at: {input.Data.BlockedAt}");
    }

    public LoanAccountHistory Transform(IEvent<LimitIncreaseRequested> input)
    {
        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"Limit increase has been requested at: {input.Data.RequestedAt}");
    }

    public LoanAccountHistory Transform(IEvent<LimitIncreaseGranted> input)
    {
        var (_, amount, grantedAt) = input.Data;

        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"Limit increase has been granted at: {grantedAt} with increase of {amount}");
    }

    public LoanAccountHistory Transform(IEvent<LimitIncreaseRejected> input)
    {
        var (_, rejectedAt) = input.Data;

        return new LoanAccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            $"Limit increase has been rejected at: {rejectedAt}");
    }
}
