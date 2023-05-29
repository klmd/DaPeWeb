using DaPe.DataAccess.Repository;
using DaPe.Models;
using DaPe.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DaPeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                ModelState.AddModelError("name", "Číslo productu nemůže být stejné jak jméno produktu");
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
                    if (productVm.Product.Id == null)
                    {
                        _unitOfWork.Product.Add(productVm.Product);
                    }
                    else
                    {
                        _unitOfWork.Product.Update(productVm.Product);
                    }
                }
                _unitOfWork.Save();
                TempData["success"] = "Produkt byl úspěšně vytvořen";
                return RedirectToAction("Index");
            }
            else
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
        //edit is not needed is part of upsert
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product productFromDb = _unitOfWork.Product.Get(c => c.Id == id);
        //    // níže stejné vyjádření téhož
        //    //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
        //    //Category categoryFromDb = _db.Categories.Find(id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Produkt byl úspěšně editován";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        // nahrazeno akcí v api calls

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product productFromDb = _unitOfWork.Product.Get(c => c.Id == id);
        //    // níže stejné vyjádření téhož
        //    //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
        //    //Category categoryFromDb = _db.Categories.Find(id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(obj => obj.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Produkt byl úspěšně smazán";
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            objProductList = _unitOfWork.Product.GetAll(includeProperties: "KindOfProduct").ToList();
            return Json(new {data = objProductList});
        }

        
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id==id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Chyba při mazání" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
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
