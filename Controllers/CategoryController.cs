using DaPe.DataAccess.Data;
using DaPe.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaPeWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        { 
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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
            Category categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
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
            Category categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
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
            Category? obj = _db.Categories.FirstOrDefault(obj => obj.Id == id);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Kategorie byla úspěšně smazána";
            return RedirectToAction("Index");
        }
    }
}
