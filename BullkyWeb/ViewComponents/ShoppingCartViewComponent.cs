using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Utility;

namespace BullkyWeb.viewCompontent
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShoppingCart.
                        GetAll(u => u.ApplicationUserId == claims.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);

            }
        }
    }
}
