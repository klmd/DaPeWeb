using DaPe.DataAccess.Repository;
using DaPe.Models;
using DaPe.Models.ViewModels;
using DaPe.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace DaPeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            objProductList = _unitOfWork.Product.GetAll(includeProperties: "KindOfProduct").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id) //Update + Insert = UpSert
        {
            ProductVM productVm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                KindOfProductList = _unitOfWork.TypeOfProduct.GetAll().Select(u => new SelectListItem
                {
                    Text = u.TypeOfProduct,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create view
                return View(productVm);
            }
            else
            {
                //update view
                productVm.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVm);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVm, IFormFile? file)
        {
            if (productVm.Product.NameOfProduct != null && productVm.Product.NameOfProduct.ToLower() == productVm.Product.DisplayProductNr.ToString().ToLower())
            {
                ModelState.AddModelError("name", "Číslo produktu nemůže být stejné jak jméno produktu");
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVm.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVm.Product.Id != 0) //toto zkontrolovat proto to asi neupdejtuje musí být 0 ne null
                {
                    _unitOfWork.Product.Update(productVm.Product);
                }
                else
                {
                    _unitOfWork.Product.Add(productVm.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Produkt byl úspěšně vytvořen/aktualizován";
                return RedirectToAction("Index");
            }
            else //tady je asi někde chyba při updatu
            {
                productVm.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                productVm.KindOfProductList = _unitOfWork.TypeOfProduct.GetAll().Select(u => new SelectListItem
                {
                    Text = u.TypeOfProduct,
                    Value = u.Id.ToString()
                });
                TempData["success"] = "Produkt byl úspěšně aktualizován";
                return View(productVm);
            }
        }
        
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            objProductList = _unitOfWork.Product.GetAll(includeProperties: "KindOfProduct").ToList();
            return Json(new {data = objProductList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Chyba při mazání" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\')); //tady mi to hází nějakou vyjímku když product neobsahuje obrázek => ošetřit případ není - li obrázek 
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { succes = true, message = "Produkt vymazán" });
        }

        #endregion
    }
}
