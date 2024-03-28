using AsoftTest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace AsoftTest.Controllers
{
    public class UserController : Controller
    {
        private testEntities db = new testEntities();
        // GET: User
        public ActionResult Index()
        {
            var listUser = new testEntities().Tests.ToList();
            return View(listUser);
        }
      
        public ActionResult ThemUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemUser(Test user)
        {
            if (ModelState.IsValid)
            {
                bool checkID = db.Tests.Any(u => u.UserID == user.UserID);
                if (checkID)
                {
                    ModelState.AddModelError("", "UserID đã tồn tại.");
                    return RedirectToAction("Index");
                }
                db.Tests.Add(user);
                db.SaveChanges(); 
                return RedirectToAction("Index", "User");
            }
            else
            {
                return View(user);
            }

        }
        [HttpGet]
        public ActionResult GetUserByID(string userID)
        {
            var user = db.Tests.FirstOrDefault(u => u.UserID == userID);

            if (user != null)
            {
                // Trả về dữ liệu người dùng dưới dạng JSON
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Nếu không tìm thấy người dùng, trả về null
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ khỏi các cuộc tấn công CSRF
        public ActionResult SuaUser(Test user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                try
                {
                   
                    db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
           
           return View(user);
        }


        public ActionResult XoaUser(string userID)
        {
          
            var user = db.Tests.FirstOrDefault(u => u.UserID == userID);
            if (user == null)
            {
                return RedirectToAction("Index", "User");
            }

            try
            {
                db.Tests.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }


    }
}