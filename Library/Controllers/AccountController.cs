using Library.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Library.Controllers
{
    public class AccountController : Controller
    {
        AccountContext db = new AccountContext();
        public ActionResult StartPage()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Books", new { controller = "Home" });
        }

        [ValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult Login()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                user = db.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "This user isn't already");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    return RedirectToAction("Books", new { controller = "Home" });
                }
            }
            return View(model);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult Register()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    ModelState.AddModelError("", "This user is already");
                    return View(model);
                }

                db.Users.Add(new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    roleId = 1
                });
                db.SaveChanges();
                user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    return RedirectToAction("Books", new { controller = "Home" });
                }
            }
            ModelState.AddModelError("", "Form isn'n valid!");
            return View(model);
        }
    }
}