using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlyingDutchman.Models;

namespace FlyingDutchman.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        private UserDBContext db = new UserDBContext();
        public ActionResult Index()
        {
            return View();
        }



     
        [HttpPost]
  
        public ActionResult Index([Bind(Include = "Id,Ip,Operation,Good,CreatedOn")] User movie)
        {

        
            if (ModelState.IsValid)
            {

                db.Users.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

    }
}