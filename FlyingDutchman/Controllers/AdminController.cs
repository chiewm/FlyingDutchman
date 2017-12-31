using FlyingDutchman.Models;
using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FlyingDutchman.Controllers
{
    public class AdminController : Controller
    {

        private MyShopDbContext db = new MyShopDbContext();


        public ActionResult Index()
        {

            #region   总览
            DateTime yesterday = DateTime.Now.AddDays(-1).Date;
            DateTime today = DateTime.Now.Date;
   
            var YesterdayQry = from q in db.Users
                               where q.CreatedOn >= yesterday && q.CreatedOn < today
                               select q;

            var TodayQry = from q in db.Users
                           where q.CreatedOn >= today
                           select q;


            /* DAU */
            var YesterdayUserQry = from u in YesterdayQry
                                   select u.Ip;
            int YesterdayDAU = YesterdayUserQry.Distinct().Count();

            var TodayUserQry = from u in TodayQry
                               select u.Ip;
            int TodayDAU = TodayUserQry.Distinct().Count();

            /* 付款人数 */
            int YesterdayBuyNum = OperationQry(YesterdayQry, "buy");
            int TodayBuyNum = OperationQry(TodayQry, "buy");

            /* 加入购物车 */
            int YesterdayAddNum = OperationQry(YesterdayQry, "add");
            int TodayAddNum = OperationQry(TodayQry, "add");

            /* 收藏 */
            int YesterdayCollectNum = OperationQry(YesterdayQry, "collect");
            int TodayCollectNum = OperationQry(TodayQry, "collect");

            /*  搜索 */
            int YesterdaySearchNum = OperationQry(YesterdayQry, "search");
            int TodaySearchNum = OperationQry(TodayQry, "search");

            /*  总数 */
            int YesterdayTotalNum = YesterdayBuyNum + YesterdayAddNum + YesterdayCollectNum + YesterdaySearchNum;
            int TodayTotalNum = TodayBuyNum + TodayAddNum + TodayCollectNum + TodaySearchNum;

            ViewBag.DAU = TodayDAU;
            ViewBag.BuyNum = TodayBuyNum;
            ViewBag.AddNum = TodayAddNum;
            ViewBag.CollectNum = TodayCollectNum;
            ViewBag.SearchNum = TodaySearchNum;
            ViewBag.TotalNum = TodayTotalNum;

            try
            {
                ViewBag.DAURate = CalRate(TodayDAU, YesterdayDAU);
                ViewBag.BuyRate = CalRate(TodayBuyNum, YesterdayBuyNum);
                ViewBag.AddRate = CalRate(TodayAddNum, YesterdayAddNum);
                ViewBag.CollectRate = CalRate(TodayCollectNum, YesterdayCollectNum);
                ViewBag.SearchRate = CalRate(TodaySearchNum, YesterdaySearchNum);
                ViewBag.TotalRate = CalRate(TodayTotalNum, YesterdayTotalNum);
            }
            catch (Exception)
            {
                ViewBag.DAURate = 100;
                ViewBag.BuyRate = 100;
                ViewBag.AddRate = 100;
                ViewBag.CollectRate = 100;
                ViewBag.SearchRate = 100;
                ViewBag.TotalRate = 100;
            }
            


            #endregion

            #region  各时间段
            List<double> BuyTime = new List<double> { };
            List<double> AddTime = new List<double> { };
            List<double> CollectTime = new List<double> { };
            List<double> SearchTime = new List<double> { };

            for (int i = 0; i < 12; i++)
            {
                BuyTime.Add(OperationQry2(TodayQry, "buy", i));
                AddTime.Add(OperationQry2(TodayQry, "add", i));
                CollectTime.Add(OperationQry2(TodayQry, "collect", i));
                SearchTime.Add(OperationQry2(TodayQry, "search", i));
            }

            List<SplineSeriesData> BuyTimeData = new List<SplineSeriesData>();
            List<SplineSeriesData> AddTimeData = new List<SplineSeriesData>();
            List<SplineSeriesData> CollectTimeData = new List<SplineSeriesData>();
            List<SplineSeriesData> SearchTimeData = new List<SplineSeriesData>();

            BuyTime.ForEach(p => BuyTimeData.Add(new SplineSeriesData { Y = p }));
            AddTime.ForEach(p => AddTimeData.Add(new SplineSeriesData { Y = p }));
            CollectTime.ForEach(p => CollectTimeData.Add(new SplineSeriesData { Y = p }));
            SearchTime.ForEach(p => SearchTimeData.Add(new SplineSeriesData { Y = p }));

            ViewData["BuyTimeData"] = BuyTimeData;
            ViewData["AddTimeData"] = AddTimeData;
            ViewData["CollectTimeData"] = CollectTimeData;
            ViewData["SearchTimeData"] = SearchTimeData;


            #endregion

            #region 用户分布
            var AreaQry = from a in TodayQry
                          group a by a.Address into g
                          orderby g.Count() descending
                          select new
                          {
                              name = g.Key,
                              value = g.Count()
                          };

            string s = "";
            foreach (var item in AreaQry)
            {
                s = "{\"name\"" + ":" + "\"" + item.name + "\"" + "," + "\"value\"" + ":" + item.value + "}," + s;
            }

            ViewBag.MapData = "[" + s + "]";


            #endregion

            #region    热销产品

            var HotSaleQry = from h in TodayQry
                             group h by h.Good into g
                             orderby g.Count() descending
                             select new
                             {
                                 g.Key,
                                 Num = g.Count()
                             };

            List<ColumnSeriesData> HotSaleData = new List<ColumnSeriesData>();
            foreach (var item in HotSaleQry.Take(5))
            {
                HotSaleData.Add(new ColumnSeriesData { Name = item.Key, Y = item.Num, Drilldown = item.Key });
            }

            ViewData["HotSale"] = HotSaleData;

            //  第一
            var Data1 = GetData(HotSaleData, 0);
            ViewBag.No1Name = Data1.Item1;
            ViewData["No1Data"] = Data1.Item2;

            // 第二
            var Data2 = GetData(HotSaleData, 1);
            ViewBag.No2Name = Data2.Item1;
            ViewData["No2Data"] = Data2.Item2;

            // 第三
            var Data3 = GetData(HotSaleData, 2);
            ViewBag.No3Name = Data3.Item1;
            ViewData["No3Data"] = Data3.Item2;

            // 第四
            var Data4 = GetData(HotSaleData, 3);
            ViewBag.No4Name = Data4.Item1;
            ViewData["No4Data"] = Data4.Item2;

            // 第五
            var Data5 = GetData(HotSaleData, 4);
            ViewBag.No5Name = Data5.Item1;
            ViewData["No5Data"] = Data5.Item2;

            #endregion

            #region   行为分析


            List<string> Goods = new List<string> { "女鞋", "香水", "口红", "女帽", "牙刷", "毛巾", "皮带", "男鞋", "烟斗", "男帽", "剃须刀" };
            List<double?> MaleValues = new List<double?> { };
            List<double?> FamaleValues = new List<double?> { };

            foreach (var good in Goods)
            {
                MaleValues.Add(WhoBuyQry(TodayQry, good, true));
                FamaleValues.Add(WhoBuyQry(TodayQry, good, false));
            }

            List<AreasplineSeriesData> MaleData = new List<AreasplineSeriesData>();
            List<AreasplineSeriesData> FamaleData = new List<AreasplineSeriesData>();

            MaleValues.ForEach(p => MaleData.Add(new AreasplineSeriesData { Y = p }));
            FamaleValues.ForEach(p => FamaleData.Add(new AreasplineSeriesData { Y = p }));

            ViewData["MaleData"] = MaleData;
            ViewData["FamaleData"] = FamaleData;
            #endregion

            return View();
        }


        public int CalRate(int today, int yesterday)
        {
            return (today - yesterday) * 100 / yesterday;
        }

        public int OperationQry(IQueryable<Models.User> QryString, string parm)
        {
            var qry = from d in QryString
                      where d.Operation == parm
                      select d;
            return qry.Count();
        }

        public int WhoBuyQry(IQueryable<Models.User> QryString, string parm, bool sex)
        {
            var qry = from d in QryString
                      where d.Good == parm && d.Sex == sex
                      select d;
            return qry.Count();
        }

        public int OperationQry2(IQueryable<Models.User> QryString, string parm, int n)
        {
            DateTime last = DateTime.Now.Date.AddHours(2 * n);
            DateTime lastAdd2Hour = DateTime.Now.Date.AddHours(2 * (n + 1));
            var qry = from d in QryString
                      where d.Operation == parm && d.CreatedOn >= last && d.CreatedOn < lastAdd2Hour
                      select d;
            return qry.Count();
        }

        public Tuple<string, List<ColumnSeriesData>> GetData(List<ColumnSeriesData> HotSaleData, int n)
        {
            string NonName = "";
            try
            {
                NonName = HotSaleData[n].Name;
            }
            catch (Exception)
            {
                NonName = "NULL";
            }
            var NonQry = from no in db.Users
                         where no.Good == NonName
                         group no by no.Operation into g
                         orderby g.Count() descending
                         select new
                         {
                             g.Key,
                             Num = g.Count()
                         };

            List<ColumnSeriesData> NonData = new List<ColumnSeriesData>();
            foreach (var item in NonQry)
            {
                NonData.Add(new ColumnSeriesData { Name = item.Key, Y = item.Num });
            }
            return Tuple.Create(NonName, NonData);
        }

        private class ChartPoint
        {
            public ChartPoint(double? x, double? y)
            {
                X = x;
                Y = y;
            }
            public double? X { get; set; }
            public double? Y { get; set; }
        }

    }



}