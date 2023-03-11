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

        string EchoDataBull(string key, string value)
        {
            return EchoData("&nbsp;&bull;&nbsp;" + key, value);
        }

        vHTML = "";
        vHTML += EchoData("OS Description", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        vHTML += EchoData("Framework Description", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        vHTML += EchoData("ASPNETCORE_ENVIRONMENT", (_config.GetValue<string>("ASPNETCORE_ENVIRONMENT")) ?? "");
        vHTML += EchoData("ENVIRONMENT", _config.GetValue<string>("ENVIRONMENT") ?? "");
        vHTML += EchoData("BuildIdentifier", _config.GetValue<string>("BuildIdentifier") ?? "");
        vHTML += EchoData("ASPNETCORE_FORWARDEDHEADERS_ENABLED", _config.GetValue<string>("ASPNETCORE_FORWARDEDHEADERS_ENABLED") ?? "");

        if (_appconfig.AdminPW == HttpContext.Request.Query["pw"].ToString())
        {
            vHTML += EchoData("Instance", _config.GetValue<string>("BuildIdentifier") ?? "");
            vHTML += EchoData("Domain", _config.GetValue<string>("Domain") ?? "");
            vHTML += EchoData("TenantId", _config.GetValue<string>("TenantId") ?? "");
            vHTML += EchoData("ClientId", _config.GetValue<string>("ClientId") ?? "");
            vHTML += EchoData("ClientSecret", _config.GetValue<string>("ClientSecret") ?? "");
            vHTML += EchoData("CallbackPath", _config.GetValue<string>("CallbackPath") ?? "");

            vHTML += EchoData("APIURLRed", _appconfig.APIURLRed);
            vHTML += EchoData("ScopesRedRead", _appconfig.ScopesRedRead);
            vHTML += EchoData("ScopesRedReadWrite", _appconfig.ScopesRedReadWrite);
            vHTML += EchoData("APIURLYellow", _appconfig.APIURLYellow);
            vHTML += EchoData("ScopesYellowRead", _appconfig.ScopesYellowRead);
            vHTML += EchoData("ScopesYellowReadWrite", _appconfig.ScopesYellowReadWrite);
            vHTML += EchoData("APIURLBlack", _appconfig.APIURLBlack);
            vHTML += EchoData("ScopesBlackRead", _appconfig.ScopesBlackRead);
            vHTML += EchoData("ScopesBlackReadWrite", _appconfig.ScopesBlackReadWrite);

            vHTML += "RequestInfo: <br/>";
            vHTML += EchoDataBull("host", HttpContext.Request.Host.ToString());
            vHTML += EchoDataBull("ishttps", HttpContext.Request.IsHttps.ToString());
            vHTML += EchoDataBull("method", HttpContext.Request.Method.ToString());
            vHTML += EchoDataBull("path", HttpContext.Request.Path.ToString());
            vHTML += EchoDataBull("pathbase", HttpContext.Request.PathBase.ToString());
            vHTML += EchoDataBull("pathbase", HttpContext.Request.Protocol.ToString());
            vHTML += EchoDataBull("pathbase", HttpContext.Request.QueryString.ToString());
            vHTML += EchoDataBull("scheme", HttpContext.Request.Scheme.ToString());

            vHTML += "Headers: <br/>";
            foreach (var key in HttpContext.Request.Headers.Keys)
            {
                vHTML += EchoDataBull(key, $"{HttpContext.Request.Headers[key]}");
            }

            vHTML += "Connection:<br/>";
            vHTML += EchoDataBull("localipaddress", HttpContext.Connection.LocalIpAddress?.ToString() ?? "null");
            vHTML += EchoDataBull("localport", HttpContext.Connection.LocalPort.ToString());
            vHTML += EchoDataBull("remoteipaddress", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "null");
            vHTML += EchoDataBull("remoteport", HttpContext.Connection.RemotePort.ToString());


        }

    }

}
