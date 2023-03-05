using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace IdentityWeb.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "ScopesBlackReadWrite")]
    public class BlackRWModel : RoleModel
    {
        private readonly ILogger<PageModel>? _logger;
        private readonly ITokenAcquisition? _tokenAcquisition;
        private readonly AppConfig? _appconfig;

        public BlackRWModel(ILogger<PageModel> logger, ITokenAcquisition tokenAcquisition, AppConfig appconfig)
                : base (logger, tokenAcquisition, appconfig)
        {            
            base.vColor = "Black";
            base.vMode = "Black.ReadWrite";
        }
   
        public void OnGet()
        {
            Response.Redirect("/" + base.vColor);
        }

        public async Task<IActionResult> OnPost(string submit)
        {
            return await base.OnPostX();
        }

    }
}
