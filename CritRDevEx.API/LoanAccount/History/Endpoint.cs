using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.History;

public static class Endpoint
{
    public const string GetAccountHistoryEndpoint = "/api/loanAccount/history/";

    [Tags(Tag.LoanAccount)]
    [WolverineGet(GetAccountHistoryEndpoint + "{loanAccountId:guid}")]
    public static Task<IReadOnlyList<LoanAccountHistory>> GetInspection(
        [FromRoute] Guid loanAccountId, 
        IQuerySession querySession, 
        CancellationToken ct)
            => querySession.Query<LoanAccountHistory>().Where(a => a.LoanAccountId == loanAccountId).ToListAsync(ct);
}