using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityWeb.Pages
{
    public class ClaimsModel : PageModel
    {
        private readonly ILogger<ClaimsModel> _logger;
        public string vHTML = "";

        public ClaimsModel(ILogger<ClaimsModel> logger)
        {
            _logger = logger;
        }

        public async void OnGet()
        {
            string WriteKeyPair(string key, string value)
            {
                return key + ": <span style='color: green'>" + value + "</span><br/>";
            }

            string WriteJWT(string name, string jwt)
            {
                string jwtlink = $"<a href='https://jwt.ms/#access_token={jwt}' class='inspectjwt' target='_blank'>Inspect JWT</a>";
                return $"{name}: &#123;&nbsp;{jwtlink}&nbsp;&#125;<br /><br />";
            }
            
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                vHTML = "";
                foreach (var ident in User.Identities)
                {

                    vHTML += WriteKeyPair("Identity RoleClaimType", ident.RoleClaimType);
                    vHTML += WriteKeyPair("Identity AuthenticationType", ident.AuthenticationType ?? "null");

                    foreach (var claim in ident.Claims)
                    {
                        vHTML += WriteKeyPair(claim.Type, claim.Value);
                    }

                    vHTML += "<br/>";
                }

                string? jwt;
                if ((jwt = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken)) is not null)
                {
                    vHTML += WriteJWT("Id Token", jwt);
                }
                if ((jwt = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)) is not null)
                {
                    vHTML += WriteJWT("Access Token", jwt);
                }
                if ((jwt = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken)) is not null)
                {
                    vHTML += WriteJWT("Refresh Token", jwt);
                }
            }
        }
    }
}
