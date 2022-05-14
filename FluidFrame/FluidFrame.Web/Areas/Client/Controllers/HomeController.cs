using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Frame = _unitOfWork.Frame.GetFirstOrDefault(x => x.Id == id, includeProperties: "FrameType,Category"),
                Count = 1
            };
            return View(cart);
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