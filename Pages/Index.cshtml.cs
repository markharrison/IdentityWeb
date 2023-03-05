using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;
using System.Security.Claims;

namespace IdentityWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public string vHTML = "";

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        void adduser (){
            vHTML += $"<a href='user' data-toggle='tooltip' title='user'  >";
            vHTML += $"<svg xmlns=\"http://www.w3.org/2000/svg\" width='100' height='100' viewBox='0 0 256 256'>";
            vHTML += "<path id='path1' transform='rotate(0,128,128) translate(3.3280029296875,0) scale(8,8)  ' fill='#888888' " + 
                "d='M8.3560181,18.363037C10.371002,19.880005 12.873993,20.779053 15.584991,20.779053 18.295013,20.779053 20.799011,19.880005 22.813995,18.363037 27.524017,18.955994 31.167999," + 
                "22.973022 31.167999,27.843994L31.167999,32 0,32 0,27.843994C0,22.973022,3.6459961,18.955994,8.3560181,18.363037z M15.584991,0C20.403992,0 24.311005,3.90802 24.311005," + 
                "8.7280273 24.311005,13.548035 20.403992,17.454041 15.584991,17.454041 10.765015,17.454041 6.8580017,13.548035 6.8580017,8.7280273 6.8580017,3.90802 10.765015,0 15.584991,0z' />";
            vHTML += "</svg></a>";
            vHTML += $"<svg xmlns=\"http://www.w3.org/2000/svg\" width='20' height='100'></svg>";
        }
        

        void addcircle(string color) {
            vHTML += $"<a href='{color}' data-toggle='tooltip' title='{color}'  >";
            vHTML += $"<svg xmlns=\"http://www.w3.org/2000/svg\" width='100' height='100'><circle cx='50' cy='50' r='50' fill='{color}' /></svg>";
            vHTML += "</a>";
            vHTML += $"<svg xmlns=\"http://www.w3.org/2000/svg\" width='20' height='100'></svg>";
        }

        vHTML = "Anonymous";

        if (User.Identity is not null && User.Identity.IsAuthenticated)
        {
            vHTML = "Authenticated<br /><br />";

            adduser();

            foreach (ClaimsIdentity ident in User.Identities)
            {
                foreach (Claim claim in ident.Claims)
                {
                    if (claim.Type == "roles" )
                    {
                        addcircle(claim.Value);
                    }
                }
            }

            vHTML += "<hr />";

        }
    }
}

