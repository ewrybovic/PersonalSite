using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalSite.Models;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

using static BCrypt.Net.BCrypt;

namespace PersonalSite.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly PersonalSite.Data.PersonalSiteContext _context;

        public SignUpModel(PersonalSite.Data.PersonalSiteContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Make sure passwords are the same
            if (!Request.Form["password"].ToString().Equals(Request.Form["rePassword"].ToString()))
            {
                // TODO make an error for if passwords do not match
                Console.WriteLine("Passwords were not similar");
                return Page();
            }

            User.Username = Request.Form["username"].ToString();
            
            User.Password = HashPassword(Request.Form["password"].ToString());
            User.CreationDate = DateTime.Now;
            User.IsAuthorized = false;
            _context.User.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
