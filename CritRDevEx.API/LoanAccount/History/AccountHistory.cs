using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using CritRDevEx.API.LoanAccount.Withdraw;
using JasperFx.Core;
using Marten.Events;
using Marten.Events.Projections;

namespace CritRDevEx.API.LoanAccount.History;

public sealed record AccountHistory(Guid Id, Guid AccountId, string Description);

public sealed class LoanAccountHistoryTransformation : EventProjection
{
    public AccountHistory Transform(IEvent<LoanAccountCreated> input)
    {              
        var (_, initialLimit) = input.Data;

        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"Account has been created at: {input.Timestamp} with initial limit of: {initialLimit}");
    }

    public AccountHistory Transform(IEvent<MoneyWithdrawn> input)
    {        
        var (_, amount) = input.Data;

        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"{amount} has been withdrawn at: {input.Timestamp}");
    }

    public AccountHistory Transform(IEvent<MoneyDeposited> input)
    {
        var(_, amount) = input.Data;

        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"{amount} has been deposited at: {input.Timestamp}");
    }

    public AccountHistory Transform(IEvent<AccountBlocked> input)
    {
        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"Account has been blocked at: {input.Timestamp}");
    }

    public AccountHistory Transform(IEvent<LimitIncreaseRequested> input)
    {
        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"Limit increase has been requested at: {input.Timestamp}");
    }

    public AccountHistory Transform(IEvent<LimitIncreaseGranted> input)
    {
        var(_, amount) = input.Data;

        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"Limit increase has been granted at: {input.Timestamp} with increase of {amount}");
    }

    public AccountHistory Transform(IEvent<LimitIncreaseRejected> input)
    {
        return new AccountHistory(
            CombGuidIdGeneration.NewGuid(),
            input.Id,
            $"Limit increase has been rejected at: {input.Timestamp}");
    }
}
