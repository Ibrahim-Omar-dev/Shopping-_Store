using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataModel.Models;
using DataModel.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utility;

namespace BullkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IWebHostEnvironment webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
    {
        this.unitOfWork = unitOfWork;
        this.webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        var product = unitOfWork.Product.GetAll(includeProperties:"Category");
        return View(product);
    }
    [HttpGet]
    public IActionResult UpSert(int? id)//UpdateInsert
    {
        #region Pass data using ViewBag and ViewData
        //IEnumerable<SelectListItem> categories = unitOfWork.Category.GetAll()
        //    .Select(x => new SelectListItem
        //    {
        //        Text = x.Name,
        //        Value = x.Id.ToString()
        //    });
        //ViewBag.CategoryList = categories;
        //ViewData["CategoryList"] = categories;
        #endregion
        ProductVM productVM = new()
        {
            CategoryList = unitOfWork.Category.GetAll()
    .Select(u => new SelectListItem
    {
        Text = u.Name,
        Value = u.Id.ToString()
    }),
            Product = new Product()
        };
        if (id == 0 || id==null)
        {
            return View(productVM);
        }
        else
        {
            productVM.Product = unitOfWork.Product.Get(i=>i.Id==id,includeProperties: "ImageProduct");
            if(productVM.Product==null)
            {
                return NotFound();
            }
            return View(productVM);
        }
    }
    //[HttpPost]
    //public IActionResult UpSert(ProductVM productVM, IFormFile file)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        string wwwRootPath=webHostEnvironment.WebRootPath;
    //        if (file != null)
    //        {
    //            string fileName = Guid.NewGuid().ToString();
    //            string pathProduct = Path.Combine(wwwRootPath, @"Images\Product");
    //            if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
    //            {
    //                var oldPath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl).TrimStart('\\');
    //                if (System.IO.File.Exists(oldPath))
    //                    System.IO.File.Delete(oldPath);
    //            }
    //            using(var fileStream=new FileStream(Path.Combine(pathProduct,fileName),FileMode.Create))
    //            {
    //                file.CopyTo(fileStream);
    //            }
    //            productVM.Product.ImageUrl = @"\Images\Product\" + fileName; ;

    //        }
    //        if (productVM.Product.Id == 0)
    //        {
    //            unitOfWork.Product.Add(productVM.Product);
    //            unitOfWork.Complete();
    //            TempData["success"] = "Product Create Successful";

    //        }
    //        else
    //        {
    //            unitOfWork.Product.Update(productVM.Product);
    //            unitOfWork.Complete();
    //            TempData["success"] = "Product Update Successful";
    //        }

    //        return RedirectToAction(nameof(Index));
    //    }
    //    productVM.CategoryList = unitOfWork.Category.GetAll()
    //        .Select(u => new SelectListItem
    //        {
    //            Text = u.Name,
    //            Value = u.Id.ToString(),
    //        });
    //    TempData["error"] = "You Mising SomeThing Try Again Please";
    //    return View(productVM);
    //}

    [HttpPost]
    public IActionResult UpSert(ProductVM productVM, List<IFormFile> files)
    {
        if (ModelState.IsValid)
        {
            if (productVM.Product.Id == 0)
            {
                unitOfWork.Product.Add(productVM.Product);
            }
            else
            {
                unitOfWork.Product.Update(productVM.Product);
            }
            unitOfWork.Complete();


            string wwwRootPath = webHostEnvironment.WebRootPath;
            if (files != null)
            {

                foreach (IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images\products\product-" + productVM.Product.Id;
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ImageProduct productImage = new()
                    {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId = productVM.Product.Id,
                    };

                    if (productVM.Product.ImageProduct == null)
                        productVM.Product.ImageProduct = new List<ImageProduct>();

                    productVM.Product.ImageProduct.Add(productImage);
                }

                unitOfWork.Product.Update(productVM.Product);
                unitOfWork.Complete();




            }


            unitOfWork.Complete();
            TempData["success"] = "Successful";
            return RedirectToAction(nameof(Index));      
        }

        productVM.CategoryList = unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

        TempData["error"] = "Please Try Again";
        return View(productVM);
    }

    public IActionResult DeleteImage(int imageId)
    {
        var imageTodeleted = unitOfWork.Image.Get(imageId);
        var productId = imageTodeleted.ProductId;

        if(imageId != null)
        {
            if (!string.IsNullOrEmpty(imageTodeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, imageTodeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            unitOfWork.Image.Remove(imageTodeleted);
            unitOfWork.Complete();
        }
        return RedirectToAction(nameof(UpSert), new { id = productId });
    }


    //[HttpGet]
    //public IActionResult Delete(int id)
    //{
    //    var product = unitOfWork.Product.Get(id);
    //    if (product == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(product);
    //}
    //[HttpPost, ActionName("Delete")]
    //public IActionResult DeleteProduct(Product product)
    //{
    //    if (product == null)
    //    {
    //        return NotFound();
    //    }
    //    if (ModelState.IsValid)
    //    {
    //        unitOfWork.Product.Remove(product);
    //        unitOfWork.Complete();
    //        TempData["success"] = "Product Deleted Successful";
    //        return RedirectToAction("index");
    //    }
    //    TempData["error"] = "Can not Delete Product";
    //    return View(product); // In case of invalid model, show form again with data
    //}

    //API Call
    [HttpGet]
    public IActionResult GetAll()
    {
        var obj = unitOfWork.Product.GetAll(includeProperties:"Category");
        return Json(new { data = obj });
    }
    public IActionResult Delete(int id)
    {
        var product = unitOfWork.Product.Get(id);
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        string productPath = @"images\products\product-" + product.Id;
        string finalPath = Path.Combine(webHostEnvironment.WebRootPath, productPath);

        if(Directory.Exists(productPath))
        {
            string[] files = Directory.GetFiles(finalPath);
            foreach(string file in files)
            {
                System.IO.File.Delete(file);
            }
            Directory.Delete(finalPath, true);
        }


        // var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
        //if(System.IO.File.Exists(oldImagePath))
        //{
        //    System.IO.File.Delete(oldImagePath);
        //}
        unitOfWork.Product.Remove(product);
        unitOfWork.Complete();
        return Json(new { success = true, message = "Delete Successful" });
    }
}
