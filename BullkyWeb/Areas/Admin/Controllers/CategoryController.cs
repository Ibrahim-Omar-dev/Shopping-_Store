using DataModel.Models;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BullkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            // Custom validation
            if (obj.Name?.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Test is not a valid category name");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Category.Add(obj);
                    _unitOfWork.Complete();
                    TempData["success"] = "Category created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception here
                    TempData["error"] = "An error occurred while creating the category";
                    return View(obj);
                }
            }

            TempData["error"] = "Please correct the errors and try again";
            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var category = _unitOfWork.Category.Get(id);
            if (category == null)
            {
                TempData["error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            // Custom validation
            if (obj.Name?.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Test is not a valid category name");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Category.Update(obj);
                    _unitOfWork.Complete();
                    TempData["success"] = "Category updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception here
                    TempData["error"] = "An error occurred while updating the category";
                    return View(obj);
                }
            }

            TempData["error"] = "Please correct the errors and try again";
            return View(obj);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var category = _unitOfWork.Category.Get(id);
            if (category == null)
            {
                TempData["error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _unitOfWork.Category.Get(id);
            if (category == null)
            {
                TempData["error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _unitOfWork.Category.Remove(category);
                _unitOfWork.Complete();
                TempData["success"] = "Category deleted successfully";
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["error"] = "An error occurred while deleting the category";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}