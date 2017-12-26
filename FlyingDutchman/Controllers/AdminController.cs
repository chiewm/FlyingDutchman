using FlyingDutchman.Models;
using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlyingDutchman.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        //public ActionResult Index()
        //{
        //    return View();
        //}
        private MyShopDbContext db = new MyShopDbContext();

        public ActionResult Index()
        {

            #region   总览
            /* 每日活跃用户数量 */
            var DAUQry = from i in db.Statistics
                         orderby i.Id descending
                         select i;

            var DAUList = DAUQry.Take(2).ToList();
            
            ViewBag.DAU = DAUList[0].DAU;
            ViewBag.DAURate = (DAUList[0].DAU - DAUList[1].DAU) * 100 / DAUList[1].DAU;

            //ViewBag.DAU = 0;
            //ViewBag.DAURate = 0;

            #endregion


            #region    热销产品

            var HotSaleQry = from p in db.Users
                             group p by p.Good into g
                             orderby g.Count() descending
                             select new
                             {
                                 g.Key,
                                 Num = g.Count()
                             };

            List<ColumnSeriesData> populationData = new List<ColumnSeriesData>();
            foreach (var item in HotSaleQry.Take(10))
            {
                populationData.Add(new ColumnSeriesData { Name = item.Key, Y = item.Num });

            }
            ViewData["HotSale"] = populationData;

            #endregion


            #region   用户分布
            #endregion


            return View();
        }


    }
}