using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using FluidFrame.Models.ViewModels;
using FluidFrame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace FluidFrame.Web.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
        public int OrderTotal { get; set; }

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // first get the identity of the user, vezi cine e
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                CartItemsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Frame"),
                Campaign = new()
            };
            foreach (var cart in ShoppingCartViewModel.CartItemsList)
            {
                cart.Price = GetPriceBasedOnBookedPeriod(cart.Count, cart.Frame.Price, cart.Frame.Price7Days);
                ShoppingCartViewModel.Campaign.CampaignTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult Summary()
        {
            // pentru metoda de Summary, facem cam la fel, cu acelasi model ca la Index 
            // mai intai iau user-ul care este logat
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // creez viewModelul cu lista de panouri publicitare rezervate si o campanie publicitara noua
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                CartItemsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Frame"),
                Campaign = new()
            };

            // adaug user-ul care face campania in viewModel
            ShoppingCartViewModel.Campaign.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

            // adaug celelalte proprietati de la user si le pun si la campanie
            ShoppingCartViewModel.Campaign.Name = ShoppingCartViewModel.Campaign.ApplicationUser.Name;
            ShoppingCartViewModel.Campaign.Phone = ShoppingCartViewModel.Campaign.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.Campaign.Street = ShoppingCartViewModel.Campaign.ApplicationUser.Street;
            ShoppingCartViewModel.Campaign.City = ShoppingCartViewModel.Campaign.ApplicationUser.City;
            ShoppingCartViewModel.Campaign.PostalCode = ShoppingCartViewModel.Campaign.ApplicationUser.PostalCode;
            ShoppingCartViewModel.Campaign.StartDate = DateTime.Now;

            var campaignDurationInDays = 0;
            foreach (var cart in ShoppingCartViewModel.CartItemsList)
            {
                if (cart.Count > campaignDurationInDays)
                    campaignDurationInDays = cart.Count;
            }

            ShoppingCartViewModel.Campaign.EndDate = DateTime.Now.AddDays(campaignDurationInDays);

            foreach (var cart in ShoppingCartViewModel.CartItemsList)
            {
                cart.Price = GetPriceBasedOnBookedPeriod(cart.Count, cart.Frame.Price, cart.Frame.Price7Days);
                ShoppingCartViewModel.Campaign.CampaignTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartViewModel);
        }

        //Daca nu o denumesc Summary, tre sa ii pun ActionName deasupra
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST(ShoppingCartViewModel cartViewModel)
        {
            // iau user-ul logat
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // iau toate panourile publicitare "adaugate in cos"
            cartViewModel.CartItemsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Frame");

            cartViewModel.Campaign.PaymentStatus = StaticDetails.PaymentStatusPending;
            cartViewModel.Campaign.CampaignStatus = StaticDetails.StatusPending;
            cartViewModel.Campaign.StartDate = DateTime.Now;
            
            // todo: calculate last booked day from all the frames.
            cartViewModel.Campaign.EndDate = DateTime.Now.AddDays(21);
            cartViewModel.Campaign.ApplicationUserId = claim.Value;

            foreach (var cart in cartViewModel.CartItemsList)
            {
                cart.Price = GetPriceBasedOnBookedPeriod(cart.Count, cart.Frame.Price, cart.Frame.Price7Days);
                cartViewModel.Campaign.CampaignTotal += (cart.Price * cart.Count);
            }

            _unitOfWork.Campaign.Add(cartViewModel.Campaign);
            _unitOfWork.Save();

            foreach (var cart in cartViewModel.CartItemsList)
            {
                CampaignItem campaignItem = new()
                {
                    FrameId = cart.FrameId,
                    CampaignId = cartViewModel.Campaign.Id,
                    Price = cart.Price,
                    DaysBooked = cart.Count
                };
                _unitOfWork.CampaignItem.Add(campaignItem);
                _unitOfWork.Save();
            }

            #region Stripe Settings
            var domain = "https://localhost:7061/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"client/shoppingcart/ConfirmCampaign?id={cartViewModel.Campaign.Id}",
                CancelUrl = domain + "client/shoppingcart/index",
            };

            foreach(var cartItem in cartViewModel.CartItemsList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cartItem.Price * 100), //20.00 -> 2000 (cents)
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Frame.ModelName,
                        },
                    },
                    Quantity = cartItem.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.Campaign.UpdateStripePaymentId(cartViewModel.Campaign.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            #endregion
        }

        public IActionResult ConfirmCampaign(int id)
        {
            Campaign campaign = _unitOfWork.Campaign.GetFirstOrDefault(u => u.Id == id);
            
            var service = new SessionService();
            Session session = service.Get(campaign.SessionId);
            // verific statusul din stripe
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.Campaign.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == campaign.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart.Count > 1)
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnBookedPeriod(double quantity, double price, double priceWeek)
        {
            if (quantity <= 6)
            {
                return price;
            }

            return priceWeek/7;

        }
    }
}
