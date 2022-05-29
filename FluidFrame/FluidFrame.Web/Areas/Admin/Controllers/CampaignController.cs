using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using FluidFrame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluidFrame.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CampaignController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampaignController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Campaign> campaigns;

            if (User.IsInRole(StaticDetails.Role_Admin))
            {
                campaigns = _unitOfWork.Campaign.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                campaigns = _unitOfWork.Campaign.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "inprocess":
                    campaigns = campaigns.Where(u => u.CampaignStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    campaigns = campaigns.Where(u => u.CampaignStatus == StaticDetails.StatusCompleted);
                    break;
                case "approved":
                    campaigns = campaigns.Where(u => u.CampaignStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = campaigns });
        }
        #endregion
    }
}
