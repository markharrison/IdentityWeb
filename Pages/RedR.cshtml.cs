using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace IdentityWeb.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "ScopesRedRead")]
    public class RedRModel : RoleModel
    {
        private readonly ILogger<PageModel>? _logger;
        private readonly ITokenAcquisition? _tokenAcquisition;
        private readonly AppConfig? _appconfig;

        public RedRModel(ILogger<PageModel> logger, ITokenAcquisition tokenAcquisition, AppConfig appconfig)
                : base (logger, tokenAcquisition, appconfig)
        {            
            base.vColor = "Red";
            base.vMode = "Red.Read";
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
