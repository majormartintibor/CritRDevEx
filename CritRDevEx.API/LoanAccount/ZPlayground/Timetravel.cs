using CritRDevEx.API.LoanAccount.Details;
using FluentValidation;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.ZPlayground;

/*
  
Just trying out random things in this file to learn more about Marten features.
  
*/

public static class Endpoint
{
    //if I want to change to Post
    public sealed record RequesTimeTravelt(Guid LoanAccountId, int Version)
    {
        public sealed class RequesTimeTravelValidator : AbstractValidator<RequesTimeTravelt>
        {
            public RequesTimeTravelValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
                RuleFor(x => x.Version).GreaterThan(0);                
            }
        }
    }

    public const string TimetravelEndpoint = "/api/loanAccount/teststuff/";

    [Tags(Tag.LoanAccount)]
    [WolverineGet(TimetravelEndpoint + "{loanAccountId:guid}/{version:int}")]
    public static async Task<IResult> TimeTravelAccountDetail(
        [FromRoute] Guid loanAccountId,
        [FromRoute] int version,        
        //[FromBody] RequesTimeTravelt request,
        IDocumentSession query)
    {
        //Time Traveling works even if the LoanAccountDetailProjection is registered as Inline
        //LoanAccountDetail as an aggegator still works on the LiveStreamAggregation<LoanAccount> 

        //Can just grab the LoanAccount stream as is
        var version1 =
            await query.Events
                .AggregateStreamAsync<LoanAccountDetail>(loanAccountId, version: version)
                    ?? new LoanAccountDetail();

        //Or do any LINQ for fancier stuff
        var version2 = await query.Events
            .QueryAllRawEvents()
            .Where(e => e.StreamId == loanAccountId && e.Version <= version)
            //add more LINQ stuff here if you need
            .AggregateToAsync<LoanAccountDetail>() 
                ?? new LoanAccountDetail();       

        

        LoanAccountDetail[] result = [version2, version1];

        return Results.Ok(result);
    }
}

