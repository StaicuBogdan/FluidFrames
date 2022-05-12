using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models.ViewModels;
using FluidFrame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FluidFrame.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = StaticDetails.Role_Admin)]
    public class FrameController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FrameController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        // View-ul este tightly bounded la viewModel.
        public IActionResult Upsert(int? id)
        {
            FrameViewModel frameVM = new()
            {
                Frame = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                FrameTypeList = _unitOfWork.FrameType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                // create frame if Id is null or zero
                return View(frameVM);
            }
            else
            {
                // update product otherwise
                frameVM.Frame = _unitOfWork.Frame.GetFirstOrDefault(i => i.Id == id);
                return View(frameVM);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(FrameViewModel obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    var uploads = Path.Combine(wwwRootPath, @"images\frames");

                    // remove old image if it exists
                    if (obj.Frame.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Frame.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //copy new image to the computed path
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    // save the image path in the Product model
                    obj.Frame.ImageUrl = @"\images\frames\" + fileName + extension;
                }

                if (obj.Frame.Id == 0)
                {
                    _unitOfWork.Frame.Add(obj.Frame);
                }
                else
                {
                    _unitOfWork.Frame.Update(obj.Frame);
                }

                _unitOfWork.Save();
                TempData["success"] = "Frame updated successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        #region API Calls for the front end
        [HttpGet]
        public IActionResult GetAll()
        {
            var framesList = _unitOfWork.Frame.GetAll(includeProperties: "Category,FrameType");
            var json = Json(new { data = framesList });
            return json;
        }

        //POST
        // [HttpPost,ActionName("Delete")] -- asta daca voiam sa las si in View asp-action="Delete", ar fi stiut sa intre aici pentru ca e cu POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Frame.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // remove image from wwwRoot
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Frame.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
