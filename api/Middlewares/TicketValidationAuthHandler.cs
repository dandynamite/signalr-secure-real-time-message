using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace SignalRTest.Middlewares;

public class MyCustomAuthSchemeOptions : AuthenticationSchemeOptions
{
    public static string Schema = "TicketValidation";
}

public class TicketValidationAuthHandler : AuthenticationHandler<MyCustomAuthSchemeOptions>
{
    public TicketValidationAuthHandler(
        IOptionsMonitor<MyCustomAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = string.Empty;

        if (Request.Headers.TryGetValue(HeaderNames.Authorization, out var reqToken))
        {
            token = reqToken.ToString().Split(" ").Last();
        }
        else
        {
            token = Request.Query["access_token"];
        }

        if (string.IsNullOrEmpty(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        // FIX ME: Validating the actual TOKEN
        if (!"just-for-example-unique-id-as-a-token".Equals(token))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Token"));

        var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, token),
                    new Claim(ClaimTypes.Name, token) };

        // generate claimsIdentity on the name of the class
        var claimsIdentity = new ClaimsIdentity(claims, nameof(TicketValidationAuthHandler));

        // generate AuthenticationTicket from the Identity
        // and current authentication scheme
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

        // pass on the ticket to the middleware
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
