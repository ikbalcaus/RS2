using eBooks.Interfaces;
using eBooks.Models.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using eBooks.Models.Exceptions;

namespace eBooks.API
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected IUsersService _usersService;

        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUsersService usersService)
            : base(options, logger, encoder, clock)
        {
            _usersService = usersService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization header");
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(':');
                if (credentials.Length != 2)
                    return AuthenticateResult.Fail("Invalid Authorization header format");
                var req = new LoginReq
                {
                    Email = credentials[0],
                    Password = credentials[1]
                };
                var user = await _usersService.Login(req);
                if (user == null)
                    return AuthenticateResult.Fail("Authentication failed");
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (ExceptionBadRequest ex)
            {
                var errorMessage = string.Join("; ", ex.Errors.SelectMany(x => x.Value));
                return AuthenticateResult.Fail(errorMessage);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Authentication error");
                return AuthenticateResult.Fail("Authentication error");
            }
        }
    }
}
