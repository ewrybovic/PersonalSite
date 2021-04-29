using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Models;

using static BCrypt.Net.BCrypt;

namespace PersonalSite.Pages.UserAction
{
    public class LogInModel : PageModel
    {
        [BindProperty]
        public User User { get; set; }

        private readonly PersonalSite.Data.PersonalSiteContext _context;

        public LogInModel(PersonalSite.Data.PersonalSiteContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _context.User.Where(i => i.Username == Request.Form["username"].ToString()).ToArrayAsync();

            // TODO add username invalid fail
            if (result.Length == 0)
                return Page();

            User = result[0];
            if (Verify(Request.Form["password"].ToString(), User.Password))
            {
                HttpContext.Session.SetString("valid", "true");
                HttpContext.Session.SetString("user", User.Username);
                return RedirectToPage("../Index");
            }
            else
            {
                // TODO add password inccorect fail
                return Page();
            } 
        }

        private bool ComparePasswords(string dbPassword, string clientPassword)
        {
            return dbPassword.Equals(HashPassword(clientPassword));
        }
    }
}