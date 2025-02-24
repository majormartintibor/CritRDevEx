using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.CreateAccount;
using JasperFx.Core;
using Account = CritRDevEx.API.LoanAccount.LoanAccount;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Fixtures;

public class GivenAccountWithLastLimitEvaluationDateOlderThanThirtyDaysExist(AppFixture fixture) 
    : IntegrationContext(fixture), IAsyncLifetime
{
    public Guid AccountId { get; private set; } = CombGuidIdGeneration.NewGuid();

    public override async Task InitializeAsync()
    {
        using var session = Store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(CombGuidIdGeneration.NewGuid(), -30000, DateTimeProvider.UtcNow.AddDays(-31));            

            _ = session.Events.StartStream<Account>(AccountId, loanAccountCreated);
            await session.SaveChangesAsync();
        }       
    }
}
