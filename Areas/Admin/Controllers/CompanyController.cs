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
    //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        public IActionResult Upsert(int? id) //Update + Insert = UpSert
        {
            
            if (id == null || id == 0)
            {
                //create view
                return View(new Company());
            }
            else
            {
                //update view
                Company CompanyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(CompanyObj);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id != 0) //toto zkontrolovat proto to asi neupdejtuje musí být 0 ne null
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Produkt byl úspěšně vytvořen/aktualizován";
                return RedirectToAction("Index");
            }
            else //tady je asi někde chyba při updatu
            {
                return View(CompanyObj);
            }
        }
        
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data = objCompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var cToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (cToBeDeleted == null)
            {
                return Json(new { success = false, message = "Chyba při mazání" });
            }
            _unitOfWork.Company.Remove(cToBeDeleted);
            _unitOfWork.Save();

            return Json(new { succes = true, message = "Produkt vymazán" });
        }

        #endregion
    }
}