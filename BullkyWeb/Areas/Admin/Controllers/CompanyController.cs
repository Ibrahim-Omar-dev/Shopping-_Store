using DataAccess.Repository.IRepository;
using DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BullkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CompanyController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Company> companies = _unitOfWork.Company.GetAll();
            return View(companies);
        }
        [HttpGet]
        public IActionResult UpSert(int id)
        {
            if (id == 0)
            {
                // Create mode: return a new instance
                var newCompany = new Company();
                return View(newCompany);
            }
            else
            {
                // Edit mode: fetch from DB
                var company = _unitOfWork.Company.Get(c => c.Id == id);
                if (company == null)
                {
                    return NotFound();
                }
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult UpSert(Company company)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please check the details and try again.";
                return View(company);
            }

            // Check if this is an update (existing record) or create (new record)
            if (company.Id == 0)
            {
                // Create new company
                _unitOfWork.Company.Add(company);
                _unitOfWork.Complete();
                TempData["success"] = "Company created successfully.";
            }
            else
            {
                // Update existing company
                _unitOfWork.Company.Update(company);
                _unitOfWork.Complete();
                TempData["success"] = "Company updated successfully.";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Company> companies = _unitOfWork.Company.GetAll();
            return Json(new { data = companies });
        }
    }
}
