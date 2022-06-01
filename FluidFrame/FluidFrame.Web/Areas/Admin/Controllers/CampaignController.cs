using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using FluidFrame.Models.ViewModels;
using FluidFrame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace FluidFrame.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CampaignController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public CampaignViewModel CampaignViewModel { get; set; }

        public CampaignController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int campaignId)
        {
            CampaignViewModel = new CampaignViewModel()
            {
                Campaign = _unitOfWork.Campaign.GetFirstOrDefault(u => u.Id == campaignId, includeProperties: "ApplicationUser"),
                CampaignItems = _unitOfWork.CampaignItem.GetAll(u => u.CampaignId == campaignId, includeProperties: "Frame"),
            };
            return View(CampaignViewModel);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCampaign()
        {
            var campaign = _unitOfWork.Campaign.GetFirstOrDefault(u => u.Id == CampaignViewModel.Campaign.Id, tracked: false);
            campaign.Name = CampaignViewModel.Campaign.Name;
            campaign.Phone = CampaignViewModel.Campaign.Phone;
            campaign.Street = CampaignViewModel.Campaign.Street;
            campaign.City = CampaignViewModel.Campaign.City;
            campaign.PostalCode = CampaignViewModel.Campaign.PostalCode;

            _unitOfWork.Campaign.Update(campaign);
            _unitOfWork.Save();
            TempData["success"] = "Campaign updated successfully.";
            return RedirectToAction("Details", "Campaign", new { campaignId = campaign.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _unitOfWork.Campaign.UpdateStatus(CampaignViewModel.Campaign.Id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Campaign updated successfully.";
            return RedirectToAction("Details", "Campaign", new { campaignId = CampaignViewModel.Campaign.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult CancelCampaign()
        {
            var campaign = _unitOfWork.Campaign.GetFirstOrDefault(u => u.Id == CampaignViewModel.Campaign.Id, tracked: false);
            if (campaign.PaymentStatus == StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = campaign.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.Campaign.UpdateStatus(campaign.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.Campaign.UpdateStatus(campaign.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
            };
            _unitOfWork.Save();
            TempData["success"] = "Campaign Cancelled successfully.";
            return RedirectToAction("Details", "Campaign", new { campaignId = CampaignViewModel.Campaign.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult StartCampaign()
        {
            var campaign = _unitOfWork.Campaign.GetFirstOrDefault(u => u.Id == CampaignViewModel.Campaign.Id, tracked: false);
            campaign.CampaignStatus = StaticDetails.StatusActive;
            campaign.StartDate = DateTime.Now;
            //default 21 days campaign
            campaign.EndDate = DateTime.Now.AddDays(21);

            _unitOfWork.Campaign.Update(campaign);
            _unitOfWork.Save();
            TempData["success"] = "Campaign Started successfully.";
            return RedirectToAction("Details", "Campaign", new { campaignId = CampaignViewModel.Campaign.Id });
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
                case "active":
                    campaigns = campaigns.Where(u => u.CampaignStatus == StaticDetails.StatusActive);
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
