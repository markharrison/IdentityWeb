using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace IdentityWeb.Pages;

public class AppConfigInfoModel : PageModel
{
    private readonly ILogger<AppConfigInfoModel> _logger;
    private readonly IConfiguration _config;
    private readonly AppConfig _appconfig;
    public string vHTML = "";

    public AppConfigInfoModel(ILogger<AppConfigInfoModel> logger, IConfiguration config, AppConfig appconfig)
    {
        _logger = logger;
        _config = config;
        _appconfig = appconfig;
    }

    public void OnGet()
    {
        string EchoData(string key, string value)
        {
            return key + ": <span style='color: blue'>" + value + "</span><br/>";
        }
        
        vHTML = "";
        vHTML += EchoData("OS Description", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        vHTML += EchoData("Framework Description", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        vHTML += EchoData("ASPNETCORE_ENVIRONMENT", (_config.GetValue<string>("ASPNETCORE_ENVIRONMENT")) ?? "");
        vHTML += EchoData("ENVIRONMENT", (_config.GetValue<string>("ENVIRONMENT")) ?? "");
        vHTML += EchoData("BuildIdentifier", (_config.GetValue<string>("BuildIdentifier")) ?? "");

        if (_appconfig.AdminPW == HttpContext.Request.Query["pw"].ToString())
        {
            vHTML += EchoData("APIURLRed", _appconfig.APIURLRed);
            vHTML += EchoData("ScopesRedRead", _appconfig.ScopesRedRead);
            vHTML += EchoData("ScopesRedReadWrite", _appconfig.ScopesRedReadWrite);
            vHTML += EchoData("APIURLYellow", _appconfig.APIURLYellow);
            vHTML += EchoData("ScopesYellowRead", _appconfig.ScopesYellowRead);
            vHTML += EchoData("ScopesYellowReadWrite", _appconfig.ScopesYellowReadWrite);
            vHTML += EchoData("APIURLBlack", _appconfig.APIURLBlack);
            vHTML += EchoData("ScopesBlackRead", _appconfig.ScopesBlackRead);
            vHTML += EchoData("ScopesBlackReadWrite", _appconfig.ScopesBlackReadWrite);
        }
    }


}
