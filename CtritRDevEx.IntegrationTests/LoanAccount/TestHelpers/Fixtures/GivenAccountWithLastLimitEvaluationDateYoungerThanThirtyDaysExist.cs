using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.CreateAccount;
using JasperFx.Core;

namespace CtritRDevEx.IntegrationTests.LoanAccount.TestHelpers.Fixtures;

public class GivenAccountWithLastLimitEvaluationDateYoungerThanThirtyDaysExist(AppFixture fixture) 
    : IntegrationContext(fixture), IAsyncLifetime
{
    public Guid AccountId { get; private set; } = CombGuidIdGeneration.NewGuid();

    public override async Task InitializeAsync()
    {
        using var session = Store.LightweightSession();
        {
            LoanAccountCreated loanAccountCreated = new(CombGuidIdGeneration.NewGuid(), -30000, DateTimeProvider.UtcNow);            

            _ = session.Events.StartStream<CritRDevEx.API.LoanAccount.LoanAccount>(AccountId, loanAccountCreated);
            await session.SaveChangesAsync();
        }       
    }
}
