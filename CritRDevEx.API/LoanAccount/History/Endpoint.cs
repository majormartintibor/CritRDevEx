using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.History;

public static class Endpoint
{
    public const string GetAccountHistoryEnpoint = "/api/loanAccount/history/";

    [Tags(Tag.LoanAccount)]
    [WolverineGet(GetAccountHistoryEnpoint + "{loanAccountId:guid}")]
    public static async Task<IReadOnlyList<LoanAccountHistory>> GetInspection(
        [FromRoute] Guid loanAccountId, 
        IQuerySession querySession, 
        CancellationToken ct)
    {
        var all = await querySession.Query<LoanAccountHistory>().ToListAsync(ct);

        var list = all.Where(h => h.LoanAccountId == loanAccountId).ToList();

        return list;
    }
}