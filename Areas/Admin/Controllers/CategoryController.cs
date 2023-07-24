using DaPe.DataAccess.Data;
using DaPe.DataAccess.Repository;
using DaPe.Models;
using DaPe.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaPeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name != null && obj.Name.ToLower() == obj.DisplayCategoryNr.ToString().ToLower())
            {
                ModelState.AddModelError("name", "Číslo kategorie nemůže být stejné jak jméno kategorie");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Kategorie byla úspěšně vytvořena";
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
            Category categoryFromDb = _unitOfWork.Category.Get(c => c.Id == id);
            // níže stejné vyjádření téhož
            //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            //Category categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Kategorie byla úspěšně editována";
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
            Category categoryFromDb = _unitOfWork.Category.Get(c => c.Id == id);
            // níže stejné vyjádření téhož
            //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            //Category categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(obj => obj.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Kategorie byla úspěšně smazána";
            return RedirectToAction("Index");
        }
    }
}
