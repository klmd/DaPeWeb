using DaPe.DataAccess.Repository;
using DaPe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DaPeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }
        public IActionResult Create()
        {
        IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        IEnumerable<SelectListItem> KindOfProductList = _unitOfWork.TypeOfProduct.GetAll().Select(u => new SelectListItem
        {
            Text = u.TypeOfProduct,
            Value = u.Id.ToString()
        });
        ViewBag.CategoryList = CategoryList;
        ViewBag.KindOfProductList = KindOfProductList;
        return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (obj.NameOfProduct != null && obj.NameOfProduct.ToLower() == obj.DisplayProductNr.ToString().ToLower())
            {
                ModelState.AddModelError("name", "Číslo productu nemůže být stejné jak jméno produktu");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Produkt byl úspěšně vytvořen";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productFromDb = _unitOfWork.Product.Get(c => c.Id == id);
            // níže stejné vyjádření téhož
            //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            //Category categoryFromDb = _db.Categories.Find(id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Produkt byl úspěšně editován";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productFromDb = _unitOfWork.Product.Get(c => c.Id == id);
            // níže stejné vyjádření téhož
            //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            //Category categoryFromDb = _db.Categories.Find(id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
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
    }
}
