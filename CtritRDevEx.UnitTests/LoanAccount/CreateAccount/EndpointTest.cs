using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.Write.CreateAccount.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.CreateAccount;

public class EndpointTest
{
    [Fact]
    public void CreateAnAccountSucceeds()
    {
        CreateLoanAccountCommand command = new(default);

        var (result, _) = CreateNewAccount(command);

        var okResult = Assert.IsType<Ok<Guid>>(result);
        Assert.IsType<Guid>(okResult.Value);
    }   
    
    [Fact]
    public void CreateAnAccountStartsStream()
    {
        CreateLoanAccountCommand command = new(default);

        var (_, stream) = CreateNewAccount(command);

        Assert.IsType<LoanAccountCreated>(stream.Events.Single());
    }
}
