using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace IdentityWeb.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "YellowScopes")]
    public class YellowModel : RoleModel
    {
        private readonly ILogger<PageModel>? _logger;
        private readonly ITokenAcquisition? _tokenAcquisition;
        private readonly AppConfig? _appconfig;

        public YellowModel(ILogger<PageModel> logger, ITokenAcquisition tokenAcquisition, AppConfig appconfig)
                : base(logger, tokenAcquisition, appconfig)
        {
            base.vColor = "Yellow";
        }

        public void OnGet()
        {
            base.OnGetX();
        }

    }
}
