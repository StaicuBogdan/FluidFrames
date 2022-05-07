using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using FluidFrame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluidFrame.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FrameTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FrameTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<FrameType> objFrameTypeList = _unitOfWork.FrameType.GetAll();
            return View(objFrameTypeList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FrameType obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FrameType.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Frame Type created successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var frameTypeFromDb = _unitOfWork.FrameType.GetFirstOrDefault(x => x.Id == id);

            if (frameTypeFromDb == null)
            {
                return NotFound();
            }

            return View(frameTypeFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FrameType obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FrameType.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Frame Type updated successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var frameTypeFromDb = _unitOfWork.FrameType.GetFirstOrDefault(u => u.Id == id);

            if (frameTypeFromDb == null)
            {
                return NotFound();
            }

            return View(frameTypeFromDb);
        }

        //POST
        // [HttpPost,ActionName("Delete")] -- asta daca voiam sa las si in View asp-action="Delete", ar fi stiut sa intre aici pentru ca e cu POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var frameTypeFromDb = _unitOfWork.FrameType.GetFirstOrDefault(u => u.Id == id);

            if (frameTypeFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.FrameType.Remove(frameTypeFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Frame Type deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
