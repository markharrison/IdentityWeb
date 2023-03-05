using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace IdentityWeb.Pages;

public class ConfigModel : PageModel
{
    private readonly ILogger<AppConfigInfoModel> _logger;
    private readonly IConfiguration _config;
    private readonly AppConfig _appconfig;
    public string vHTML = "";

    [BindProperty]
    public string? RedAPIURL { get; set; }
    [BindProperty]
    public string? RedScopes { get; set; }
    [BindProperty]
    public string? BlackAPIURL { get; set; }
    [BindProperty]
    public string? BlackScopes { get; set; }
    [BindProperty]
    public string? YellowAPIURL { get; set; }
    [BindProperty]
    public string? YellowScopes { get; set; }
    [BindProperty]
    public string? ErrorMessage { get; set; }

    public ConfigModel(ILogger<AppConfigInfoModel> logger, IConfiguration config, AppConfig appconfig)
    {
        _logger = logger;
        _config = config;
        _appconfig = appconfig;
    }

    public void OnGet()
    {
        var vCookie = "";

        vCookie = Request.Cookies["APIURLRed"];
        RedAPIURL = (vCookie != null) ? vCookie.ToString() : "https://markidentityapi.azurewebsites.net/red";

        vCookie = Request.Cookies["APIURLBlack"];
        BlackAPIURL = (vCookie != null) ? vCookie.ToString() : "https://markidentityapi.azurewebsites.net/black";

        vCookie = Request.Cookies["APIURLYellow"];
        YellowAPIURL = (vCookie != null) ? vCookie.ToString() : "https://markidentityapi.azurewebsites.net/yellow";

    }

    public IActionResult OnPost()
    {
        if (RedAPIURL == null || RedAPIURL.Trim() == "" || (!(RedAPIURL.ToLower().StartsWith("http://") || RedAPIURL.ToLower().StartsWith("https://"))))
        {
            ErrorMessage = "Invalid RedAPIURL";
            return Page();
        }

        if (YellowAPIURL == null || YellowAPIURL.Trim() == "" || (!(YellowAPIURL.ToLower().StartsWith("http://") || YellowAPIURL.ToLower().StartsWith("https://"))))
        {
            ErrorMessage = "Invalid YellowAPIURL";
            return Page();
        }

        if (BlackAPIURL == null || BlackAPIURL.Trim() == "" || (!(BlackAPIURL.ToLower().StartsWith("http://") || BlackAPIURL.ToLower().StartsWith("https://"))))
        {
            ErrorMessage = "Invalid BlackAPIURL";
            return Page();
        }

        Response.Cookies.Append("APIURLRed", RedAPIURL.Trim(),
                new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTime.Now.AddMonths(12)
                }
            );
        Response.Cookies.Append("APIURLYellow", YellowAPIURL.Trim(),
        new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTime.Now.AddMonths(12)
                }
            );
        Response.Cookies.Append("APIURLBlack", BlackAPIURL.Trim(),
        new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTime.Now.AddMonths(12)
                }
            );

        return RedirectToPage("/Index");

    }

}