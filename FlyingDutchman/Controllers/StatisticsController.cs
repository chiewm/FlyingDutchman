using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlyingDutchman.Models;

namespace FlyingDutchman.Controllers
{
    public class StatisticsController : Controller
    {
        private MyShopDbContext db = new MyShopDbContext();

        // GET: Statistics
        public ActionResult Index()
        {
            return View(db.Statistics.ToList());
        }

        // GET: Statistics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Statistic statistic = db.Statistics.Find(id);
            if (statistic == null)
            {
                return HttpNotFound();
            }
            return View(statistic);
        }

        // GET: Statistics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Statistics/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]

        public ActionResult Create([Bind(Include = "Id,Date,DAU,BuyNum,AddNum,CollectNum,TotalNum,TotalGood")] Statistic statistic)
        {
            if (ModelState.IsValid)
            {
                db.Statistics.Add(statistic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statistic);
        }



        // POST: Statistics/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
      
    }
}
