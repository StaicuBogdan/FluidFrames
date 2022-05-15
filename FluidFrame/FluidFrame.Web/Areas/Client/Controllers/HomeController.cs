using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FluidFrame.Web.Controllers
{
    [Area("Client")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Frame> frames = _unitOfWork.Frame.GetAll(includeProperties: "FrameType,Category");
            return View(frames);
        }

        public IActionResult Details(int frameId)
        {
            ShoppingCart cart = new()
            {
                Frame = _unitOfWork.Frame.GetFirstOrDefault(x => x.Id == frameId, includeProperties: "FrameType,Category"),
                FrameId = frameId,
                Count = 1
            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            // extragem id-ul user-ului din Claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;

            ShoppingCart cartDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.FrameId == cart.FrameId);

            if (cartDb == null)
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartDb, cart.Count);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}