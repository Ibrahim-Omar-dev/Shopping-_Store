using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataModel.Models;
using DataModel.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Utility;

namespace BullkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IEmailSender EmailSender { get; }

        public CartController(IUnitOfWork unitOfWork,IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            EmailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<ImageProduct> imageProducts = _unitOfWork.Image.GetAll();
            var shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new OrderHeader()
            };
            foreach(var cart in  shoppingCartVM.ShoppingCartList)
            {
                cart.Price=GetPrice(cart);
                cart.Product.ImageProduct = imageProducts.Where(i => i.ProductId == cart.ProductId).ToList();
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        public IActionResult plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(i=>i.Id== cartId,tracked:true);

            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cart);

            }
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

     
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(i => i.Id == cartId);
            if (cart == null)
            {
                return NotFound();
            }
            _unitOfWork.ShoppingCart.Remove(cart);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
       .GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
            _unitOfWork.Complete();
            TempData["Success"] = "Deleted Successfuly";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return Unauthorized();

            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = userIdClaim.Value;

            var ShoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new OrderHeader()
            };

            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            if (applicationUser == null)
                return NotFound();

            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUser;

            ShoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = applicationUser.City;
            ShoppingCartVM.OrderHeader.State = applicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.Name = applicationUser.Name;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [Display(Name = "Summary")]
        public IActionResult SummaryOrder()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ShoppingCartList=_unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == UserId,
                includeProperties: "Product"
            );

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = UserId;

           ApplicationUser applicationUser= _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //this order for Regular User
                ShoppingCartVM.OrderHeader.PaymentStatus=SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //this order for Company User
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

             _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Complete();
            foreach (var chart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = chart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Count = chart.Count,
                    Price = chart.Price
                };
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Complete();

            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer account and we need to capture payment
                //stripe logic
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentStatus(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Complete();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

                return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        { 
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if(orderHeader.PaymentStatus !=SD.PaymentStatusDelayedPayment)
            {
                SessionService service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentStatus(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Complete();
                }
            }
            EmailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Web",
                "<p>New Order Created </p>");
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Complete();

            return View(id);
        }
        private double GetPrice(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count < 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count < 100)
                {
                    return shoppingCart.Product.Price50;
                }
                return shoppingCart.Product.Price;
            }
        }
    }
}
