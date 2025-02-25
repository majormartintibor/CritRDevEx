using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.BlockAccount;
using CritRDevEx.API.LoanAccount.CreateAccount;
using CritRDevEx.API.LoanAccount.Deposit;
using CritRDevEx.API.LoanAccount.LimitIncrease;
using JasperFx.Core;
using Marten;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers;

internal static class Given
{
    public static async Task BlockedAccount(this IDocumentStore store, Guid accountId)
    {
        using var session = store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(CombGuidIdGeneration.NewGuid(), -30000, DateTimeProvider.UtcNow);
            LoanAccountBlocked loanAccountBlocked = new(accountId, DateTimeProvider.UtcNow);

            _ = session.Events.StartStream<CritRDevEx.API.LoanAccount.LoanAccount>(accountId, [loanAccountCreated, loanAccountBlocked]);
            await session.SaveChangesAsync();
        }
    }

    public static async Task AccountWithPendingLimitIncreaseRequest(this IDocumentStore store, Guid accountId)
    {
        using var session = store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(CombGuidIdGeneration.NewGuid(), -30000, DateTimeProvider.UtcNow);
            LimitIncreaseRequested limitIncreaseRequested = new(accountId, DateTimeProvider.UtcNow);

            _ = session.Events.StartStream<CritRDevEx.API.LoanAccount.LoanAccount>(accountId, [loanAccountCreated, limitIncreaseRequested]);
            await session.SaveChangesAsync();
        }
    }

    public static async Task AccountForDebtorExists(this IDocumentStore store, Guid debtorId)
    {
        using var session = store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(debtorId, -30000, DateTimeProvider.UtcNow);

            _ = session.Events.StartStream<CritRDevEx.API.LoanAccount.LoanAccount>(CombGuidIdGeneration.NewGuid(), loanAccountCreated);
            await session.SaveChangesAsync();
        }
    }

    public static async Task AccountWithLastLimitEvaluationDateOlderThanThirtyDaysExist(this IDocumentStore store, Guid accountId)
    {
        using var session = store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(CombGuidIdGeneration.NewGuid(), -30000, DateTimeProvider.UtcNow.AddDays(-31));

            _ = session.Events.StartStream<CritRDevEx.API.LoanAccount.LoanAccount>(accountId, loanAccountCreated);
            await session.SaveChangesAsync();
        }
    }

    public static async Task AccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(this IDocumentStore store, Guid accountId)
    {
        using var session = store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(CombGuidIdGeneration.NewGuid(), -30000, DateTimeProvider.UtcNow);

            _ = session.Events.StartStream<CritRDevEx.API.LoanAccount.LoanAccount>(accountId, loanAccountCreated);
            await session.SaveChangesAsync();
        }
    }

    public static async Task AddDeposit(this IDocumentStore store, Guid accointId, decimal amount)
    {
        using var session = store.LightweightSession();
        {
            MoneyDeposited moneyDeposited = new(accointId, amount, DateTimeProvider.UtcNow);

            _ = session.Events.Append(accointId, moneyDeposited);
            await session.SaveChangesAsync();
        }
    }
}