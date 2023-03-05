using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Security.Claims;

namespace IdentityWeb.Pages
{
    public class RoleModel : PageModel
    {
        private readonly ILogger<PageModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly AppConfig _appconfig;
        private static readonly HttpClient _httpClient = new();

        [BindProperty]
        public string vHTML { get; set; }

        [BindProperty]
        public string vEndPoint { get; set; }

        [BindProperty]
        public string vColor { get; set; }

        [BindProperty]
        public string vMode { get; set; }

        [BindProperty]
        public string vScope { get; set; }

        [BindProperty]
        public string vCircle { get; set; }

        public RoleModel(ILogger<PageModel> logger, ITokenAcquisition tokenAcquisition, AppConfig appconfig)
        {
            _appconfig = appconfig;
            _tokenAcquisition = tokenAcquisition;
            _logger = logger;

            vHTML = "";
            vEndPoint = "";
            vColor = "";
            vMode = "";
            vScope = "";
            vCircle = "";
        }

        private string Writekeypair(string key, string value)
        {
            return key + ": <span style='color: green'>" + value + "</span><br/>";
        }
        private bool AuthZ(string role)
        {
            if (!(User.Identity is not null && User.Identity.IsAuthenticated)) { return false; }

            foreach (ClaimsIdentity ident in User.Identities)
            {
                foreach (Claim claim in ident.Claims)
                {
                    if ((claim.Type == ClaimConstants.Roles || claim.Type == ClaimConstants.Role)
                                    && string.Equals(claim.Value, role, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        private string[] GetScopes()
        {
            string[] scopesarray = vScope.Split(" ");
            return scopesarray;
        }

        private void InitCircle()
        {
            vCircle += $"<a href='{vColor}' data-toggle='tooltip' title='{vColor}'  >"
                    + $"<svg xmlns=\"http://www.w3.org/2000/svg\" width='100' height='100'><circle cx='50' cy='50' r='50' fill='{vColor}' /></svg>"
                    + "</a>";
        }

        private void InitEndpoint()
        {
            vEndPoint = vColor.ToLower() switch
            {
                "red" => Request.Cookies["APIURLRed"]?.ToString() ?? _appconfig.APIURLRed,
                "yellow" => Request.Cookies["APIURLYellow"]?.ToString() ?? _appconfig.APIURLYellow,
                "black" => Request.Cookies["APIURLBlack"]?.ToString() ?? _appconfig.APIURLBlack,
                _ => "Error",
            };
        }

        private void InitScopes()
        {
            vScope = vMode.ToLower() switch
            {
                "red.read" => _appconfig.ScopesRedRead,
                "red.readwrite" => _appconfig.ScopesRedReadWrite,
                "yellow.read" => _appconfig.ScopesYellowRead,
                "yellow.readwrite" => _appconfig.ScopesYellowReadWrite,
                "black.read" => _appconfig.ScopesBlackRead,
                "black.readwrite" => _appconfig.ScopesBlackReadWrite,
                _ => "error",
            };

        }

        public void OnGetX()
        {
            if (!AuthZ(vColor))
            {
                Response.Redirect("/MicrosoftIdentity/Account/AccessDenied");
                return;
            }

            InitCircle();
            InitEndpoint();
            InitScopes();

        }

        public async Task<IActionResult> OnPostX()
        {
            if (!AuthZ(vColor))
            {
                Response.Redirect("/MicrosoftIdentity/Account/AccessDenied");
                return Page();
            }

            InitCircle();
            InitEndpoint();
            InitScopes();

            string accessToken = "";
            try
            {
                vHTML = "<hr />Response: <br /><br /> ";

                accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(GetScopes());

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage? datarsp = null;
                if (vMode.EndsWith(".ReadWrite"))
                {
                    var myContent = "{\"key\": \"value\" }";
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    datarsp = await _httpClient.PostAsync(vEndPoint, byteContent);
                    ;
                }
                else
                {
                    datarsp = await _httpClient.GetAsync(vEndPoint);
                }

                if (datarsp.IsSuccessStatusCode)
                {
                    var userdata = await datarsp.Content.ReadAsStringAsync();
                    vHTML += Writekeypair("userdata", userdata) + "<br />";
                }
                else
                {
                    vHTML = $"<span style='color: red;'>Http failure - {vColor} OnPost - Error: {Convert.ToInt32(datarsp.StatusCode)} - {datarsp.ReasonPhrase ?? ""}</span><br /><br />";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("MsalUiRequiredException")) { throw; }

                var errmsg = "Exception - {vColor} OnPost - " + ex.Message;
                vHTML = "<span style='color: red;'>" + errmsg + "</span><br /><br />";
            }

            if (accessToken.StartsWith("ey"))
            {
                string jwtlink = $"<a href='https://jwt.ms/#access_token={accessToken}' class='inspectjwt' target='_blank'>Inspect JWT</a>";
                vHTML += $"Access Token: &#123;&nbsp;{jwtlink}&nbsp;&#125;<br />";
            };

            return Page();

        }
    }
}
