using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityWeb.Pages;

[Authorize]
[AuthorizeForScopes(Scopes = new[] { "User.Read" })]
public class UserModel : PageModel
{
    private readonly ILogger<UserModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _config;
    private static readonly HttpClient _httpClient = new HttpClient();

    [BindProperty]
    public string? vHTML { get; set; }

    [BindProperty]
    public string? vEndPoint { get; set; }

    public UserModel(ILogger<UserModel> logger, ITokenAcquisition tokenAcquisition, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _tokenAcquisition = tokenAcquisition;
        vEndPoint = "https://graph.microsoft.com/beta/me";
        vHTML = "";
    }

    public void OnGet()
    {

        vHTML = "";

        if (!(User.Identity is not null && User.Identity.IsAuthenticated))
        {
            Response.Redirect("/MicrosoftIdentity/Account/AccessDenied");
            return;
        }

    }

    public async Task<IActionResult> OnPost()
    {
        JsonDocument userdata;
        string graphToken = "";

        string WriteKeyPair(string key, string value)
        {
            return key + ": <span style='color: green'>" + value + "</span><br/>";
        }

        string WriteProperty(string key)
        {
            return WriteKeyPair(key, userdata?.RootElement.GetProperty(key).ToString() ?? "null");
        }

        vHTML = "";
        try
        {
            graphToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.Read" });

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", graphToken);

            HttpResponseMessage datarsp = await _httpClient.GetAsync(vEndPoint);
            if (datarsp.IsSuccessStatusCode)
            {
                userdata = JsonDocument.Parse(await datarsp.Content.ReadAsStreamAsync());

                vHTML += WriteProperty("displayName");
                vHTML += WriteProperty("mail");
                vHTML += WriteProperty("userPrincipalName");
                vHTML += WriteProperty("givenName");
                vHTML += WriteProperty("surname");
                vHTML += WriteProperty("jobTitle");
                vHTML += WriteProperty("businessPhones");
                vHTML += WriteProperty("mobilePhone");
                vHTML += WriteProperty("officeLocation");
                vHTML += WriteProperty("preferredLanguage");
                vHTML += WriteProperty("id");
                vHTML += "<br />";
            }
            else
            {
                vHTML = $"<span style='color: red;'>Http failure - User OnPost - Error: {{Convert.ToInt32(datarsp.StatusCode)}} - {datarsp.ReasonPhrase ?? ""}</span><br />";
            }
        }
        catch (Exception ex)
        {
            var errmsg = "Exception - User OnPost - " + ex.Message;
            _logger.LogError(errmsg);
            vHTML = "<span style='color: red;'>" + errmsg + "</span><br>";
        }

        if (graphToken.StartsWith("ey"))
        {
            string jwtlink = $"<a href='https://jwt.ms/#access_token={graphToken}' class='inspectjwt' target='_blank'>Inspect JWT</a>";
            vHTML += $"Access Token: &#123;&nbsp;{jwtlink}&nbsp;&#125;<br />";
        };

        return Page();

    }

}

