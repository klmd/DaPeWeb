using DaPe.DataAccess.Repository;
using DaPe.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaPeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KindOfProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public KindOfProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<KindOfProduct> objTypeOfProductList = _unitOfWork.TypeOfProduct.GetAll().ToList();
            return View(objTypeOfProductList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(KindOfProduct obj)
        {
            if (obj.TypeOfProduct != null && obj.TypeOfProduct.ToLower() == obj.DisplayTypeOfProductNr.ToString().ToLower())
            {
                ModelState.AddModelError("name", "Číslo typu měřáku nemůže být stejné jak jméno typu měřáku");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.TypeOfProduct.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Typ měřáku byl úspěšně vytvořen";
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
            KindOfProduct typeOfProductFromDb = _unitOfWork.TypeOfProduct.Get(c => c.Id == id);
            // níže stejné vyjádření téhož
            //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            //Category categoryFromDb = _db.Categories.Find(id);
            if (typeOfProductFromDb == null)
            {
                return NotFound();
            }
            return View(typeOfProductFromDb);
        }
        [HttpPost]
        public IActionResult Edit(KindOfProduct obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.TypeOfProduct.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Typ měřáku byl úspěšně editován";
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
            KindOfProduct typeOfProductFromDb = _unitOfWork.TypeOfProduct.Get(c => c.Id == id);
            // níže stejné vyjádření téhož
            //Category categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            //Category categoryFromDb = _db.Categories.Find(id);
            if (typeOfProductFromDb == null)
            {
                return NotFound();
            }
            return View(typeOfProductFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            KindOfProduct? obj = _unitOfWork.TypeOfProduct.Get(obj => obj.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.TypeOfProduct.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Typ měřáku byl úspěšně smazán";
            return RedirectToAction("Index");
        }
    }
}
