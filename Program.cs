using IdentityWeb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.Json;
using System.Web;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
      .AddMicrosoftIdentityWebApp(options =>
      {
          builder.Configuration.Bind("AzureAD", options);
          options.ResponseType = OpenIdConnectResponseType.Code;
          options.Events ??= new OpenIdConnectEvents();
          options.Events.OnTokenValidated += OnTokenValidatedFunc;
          options.Events.OnAuthenticationFailed += OnAuthenticationFailedFunc;
          options.Events.OnAccessDenied += OnAccessDeniedFunc;
          options.Events.OnRemoteFailure += OnRemoteFailureFunc;
          options.Events.OnUserInformationReceived += OnUserInformationReceivedFunc;
          options.Events.OnRedirectToIdentityProvider += OnRedirectToIdentityProviderFunc;
          options.Events.OnTicketReceived += OnTicketReceivedFunc;
          options.Events.OnAuthorizationCodeReceived += OnAuthorizationCodeReceivedFunc;
          options.SaveTokens = true;
          options.UsePkce = true;
          options.GetClaimsFromUserInfoEndpoint = true;
          options.TokenValidationParameters = new TokenValidationParameters
          {
              RoleClaimType = "role",
              NameClaimType = "name"
          };
      }
    )
     .EnableTokenAcquisitionToCallDownstreamApi(builder.Configuration.GetValue<string>("ScopesGraph")?.Split(' '))
     .AddInMemoryTokenCaches();

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
    options =>
        {
            options.Events = new RejectSessionCookieWhenAccountNotInCacheEvents();
        });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddSingleton(new AppConfig(builder.Configuration));

builder.Services.AddRazorPages(options =>
{
            options.Conventions.AllowAnonymousToPage("/");
            options.Conventions.AllowAnonymousToPage("/Index");
            options.Conventions.AllowAnonymousToPage("/AuthFail");
            options.Conventions.AllowAnonymousToPage("/Error");
            options.Conventions.AllowAnonymousToPage("/Claims");
            options.Conventions.AllowAnonymousToPage("/Config");
            options.Conventions.AllowAnonymousToPage("/AppConfigInfo");
            options.Conventions.AllowAnonymousToPage("/About");

            options.Conventions.AuthorizePage("/Private");
            options.Conventions.AuthorizePage("/Red");
            options.Conventions.AuthorizePage("/Black");
            options.Conventions.AuthorizePage("/Yellow");
            options.Conventions.AuthorizePage("/Role");
            options.Conventions.AuthorizePage("/User");
        })
    .AddMicrosoftIdentityUI();

var app = builder.Build();

app.Logger.LogInformation("Started");

app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
        });

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.Run();

//--------------------------------------------------//

void AddClaim(ClaimsIdentity identity, string name, string value)
{
    var existingClaim = identity.FindFirst(name);
    if (existingClaim != null)
        identity.RemoveClaim(existingClaim);

    identity.AddClaim(new Claim(name, value));
};


async Task OnUserInformationReceivedFunc(UserInformationReceivedContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnUserInformationReceivedFunc");

        try
        {
            ClaimsIdentity? identity = ctx.Principal?.Identities.First();

            if (identity is not null)
            {
                JsonDocument tkn = ctx.User;

                var array = tkn.RootElement.EnumerateObject();
                foreach (var item in array)
                {
                    AddClaim(identity, item.Name.ToString(), item.Value.ToString());
                }
            }

        }
        catch (Exception e)
        {
            ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogError("Exception - OnUserInformationReceivedFunc - " + e.Message);
        }

        //var dbContext = ctx.HttpContext.RequestServices.GetService<YourDbContext>();  
    }

    await Task.CompletedTask.ConfigureAwait(false);
}

async Task OnTokenValidatedFunc(TokenValidatedContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnTokenValidatedFunc");

        //var actoken = ctx.ProtocolMessage.AccessToken;
        //var idtoken = ctx.SecurityToken as JwtSecurityToken;

        try
        {
            JsonDocument? data = JsonSerializer.Deserialize<JsonDocument>(ctx.ProtocolMessage.State, new JsonSerializerOptions { IncludeFields = true });
            string returnuri = data?.RootElement.GetProperty("returnuri").ToString() ?? "/";

            ClaimsIdentity? identity = ctx.Principal?.Identities.First();
            if (identity is not null)
            {
                AddClaim(identity, "returnuri", returnuri);
                AddClaim(identity, "customx", "customvalx");
            }

            //ClaimsPrincipal? principal = ctx.Principal;
            //if (principal is not null)
            //{
            //    principal.AddIdentity(new ClaimsIdentity(
            //        new List<Claim>
            //        {
            //            new Claim(ClaimTypes.Role, "customrole"),
            //            new Claim("customz", "customvalz")
            //        },
            //        "customauthtype", "customnametype", "customroletype"));
            //}

        }
        catch (Exception e)
        {
            ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogError("Exception - OnTokenValidatedFunc - " + e.Message);
        }
    }

    await Task.CompletedTask.ConfigureAwait(false);
}

async Task OnRemoteFailureFunc(RemoteFailureContext ctx)
{

    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnRemoteFailureFunc");

        ctx.HandleResponse();

        var msg = (ctx.Failure is not null) ? ctx.Failure.Message : string.Empty;
        ctx.Response.Redirect($"/AuthFail?e=RemoteFailure&msg=" + HttpUtility.UrlEncode(msg));
    }

    await Task.CompletedTask.ConfigureAwait(false);
}

async Task OnAccessDeniedFunc(AccessDeniedContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnAccessDeniedFunc");

        ctx.HandleResponse();
        ctx.Response.Redirect($"/AuthFail?e=AccessDenied=&msg=" + HttpUtility.UrlEncode(ctx.AccessDeniedPath));
    }

    await Task.CompletedTask.ConfigureAwait(false);
}

async Task OnAuthenticationFailedFunc(AuthenticationFailedContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnAuthenticationFailedFunc");

        ctx.HandleResponse();
        ctx.Response.Redirect($"/AuthFail?e=AuthenticationFailed=&msg=" + HttpUtility.UrlEncode(ctx.Exception.Message));
    }

    await Task.CompletedTask.ConfigureAwait(false);
}

async Task OnRedirectToIdentityProviderFunc(RedirectContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnRedirectToIdentityProviderFunc");

        string? returnuri = ctx.HttpContext?.Request.Path.ToString().ToLower();
        if (returnuri is null || returnuri.Contains("/microsoftidentity/account/signin"))
        {
            returnuri = ctx.HttpContext?.Request.Headers.Referer.ToString().ToLower();
            if (returnuri is null || returnuri.Contains("/authfail") || returnuri.Contains("/microsoftidentity/account/signedout"))
            {
                returnuri = "/";
            }
        }

        returnuri += ctx.HttpContext?.Request.QueryString;

        //string? returnuri = ctx.HttpContext?.Request.Headers.Referer.ToString().ToLower();
        //if (returnuri is null || returnuri.Contains("/authfail") || returnuri.Contains("/microsoftidentity/account/signedout"))
        //{
        //    returnuri = "/";
        //}

        ctx.ProtocolMessage.State = JsonSerializer.Serialize(new
        {
            returnuri,
            rand = Guid.NewGuid().ToString()
        });
    }

    await Task.CompletedTask.ConfigureAwait(false);
}


async Task OnTicketReceivedFunc(TicketReceivedContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnTicketReceivedFunc");

        ctx.ReturnUri = ctx.Principal?.FindFirst("returnuri")?.Value.ToString() ?? "/";
    }

    await Task.CompletedTask.ConfigureAwait(false);
}

async Task OnAuthorizationCodeReceivedFunc(AuthorizationCodeReceivedContext ctx)
{
    lock (ctx)
    {
        ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogInformation("OnAuthorizationCodeReceivedFunc");
    }

    await Task.CompletedTask.ConfigureAwait(false);
}



// github.com/AzureAD/microsoft-identity-web/issues/13
class RejectSessionCookieWhenAccountNotInCacheEvents : CookieAuthenticationEvents
{
    public async override Task ValidatePrincipal(CookieValidatePrincipalContext ctx)
    {
        try
        {
            var tokenAcquisition = ctx.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();

            var token = await tokenAcquisition.GetAccessTokenForUserAsync(
                scopes: new[] { "profile" },
                user: ctx.Principal);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
            when (AccountDoesNotExitInTokenCache(ex))
        {
            ctx.RejectPrincipal();
        }
        catch (Exception ex)
        {
            ctx.HttpContext?.RequestServices?.GetService<ILogger<Program>>()?.LogError("Exception - ValidatePrincipal - " + ex.Message);
        }
    }

    private bool AccountDoesNotExitInTokenCache(MicrosoftIdentityWebChallengeUserException ex)
    {
        return ex.InnerException is MsalUiRequiredException &&
              (ex.InnerException as MsalUiRequiredException)?.ErrorCode == MsalError.UserNullError;
    }
}