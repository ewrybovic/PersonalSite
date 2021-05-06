using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Models;

namespace PersonalSite.Pages
{
    public class ArduinoCommunicatorModel : PageModel
    {
        private readonly PersonalSite.Data.PersonalSiteContext _context;

        [BindProperty]
        public User User { get; set; }

        public string Username { get; private set; } = "No User";
        public bool IsAuthorized { get; private set; } = false;

        public ArduinoCommunicatorModel(PersonalSite.Data.PersonalSiteContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("user");
            if (username == null)
            {
                IsAuthorized = false;
                return Page();
            }

            User = await _context.User.FirstOrDefaultAsync(m => m.Username == username);
            if (User == null)
            {
                return Page();
            }
            else
            {
                if (User.IsAuthorized)
                {
                    IsAuthorized = true;
                    Username = username;
                }
                else
                    Username = "Not Authorized";
                
                return Page();
            }
        }
    }
}