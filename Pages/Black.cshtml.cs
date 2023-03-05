using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace IdentityWeb.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "BlackScopes")]
    public class BlackModel : RoleModel
    {
        private readonly ILogger<PageModel>? _logger;
        private readonly ITokenAcquisition? _tokenAcquisition;
        private readonly AppConfig? _appconfig;

        public BlackModel(ILogger<PageModel> logger, ITokenAcquisition tokenAcquisition, AppConfig appconfig)
                : base(logger, tokenAcquisition, appconfig)
        {
            base.vColor = "Black";
        }

        public void OnGet()
        {
            base.OnGetX();
        }

    }
}
