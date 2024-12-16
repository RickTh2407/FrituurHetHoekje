using FrituurHetHoekje.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FrituurHetHoekje.Controllers
{
    public class BaseController : Controller
    {
        protected readonly FrituurDB _context;

        public BaseController(FrituurDB context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = _context.Accounts.FirstOrDefault(u => u.Email == userEmail);
                ViewData["CurrentUser"] = currentUser;
            }
            base.OnActionExecuting(context);
        }
    }
}
