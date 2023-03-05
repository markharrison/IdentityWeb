using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityWeb.Pages
{
    public class AuthFailModel : PageModel
    {
        public string vHTML = "";

        private readonly ILogger<AuthFailModel> _logger;

        public AuthFailModel(ILogger<AuthFailModel> logger)
        {
            _logger = logger;
        }


        public void OnGet()
        {
            vHTML = "";

            vHTML += Request.Query["msg"].ToString() + "<hr/>";

            // vHTML += "<a href='https://login.microsoftonline.com/25c5d63e-9b08-4844-a7c2-2467d637847b/oauth2/v2.0/logout'>Signout</a>";
            vHTML += "<a href='MicrosoftIdentity/Account/SignOut'>Signout</a>";

        }
    }
}
