using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.History;

public static class Endpoint
{
    public const string GetAccountHistoryEndpoint = "/api/loanAccount/history/";

    [Tags(Tag.LoanAccount)]
    [WolverineGet(GetAccountHistoryEndpoint + "{loanAccountId:guid}")]
    public static Task GetInspection(
        [FromRoute] Guid loanAccountId, 
        IQuerySession querySession, 
        HttpContext context)
            => querySession.Json.WriteById<LoanAccountHistory>(loanAccountId, context);
}