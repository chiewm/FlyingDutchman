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

        public int CalRate(int today, int yesterday)
        {
            return (today - yesterday) * 100 / yesterday;
        }

        public int OperationQry(IQueryable<Models.User>QryString,string parm)
        {
            var qry = from d in QryString
                      where d.Operation == parm
                      select d;
            return  qry.Count();
        }

        #region 查询热度数据函数
        public Tuple<string, List<ColumnSeriesData>> GetData(List<ColumnSeriesData> HotSaleData, int n)
        {
            string NonName = HotSaleData[n].Name;
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

        #endregion

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


        public ActionResult Index()
        {

            #region   总览
            DateTime yesterday = DateTime.Now.AddDays(-1);
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
                ViewBag.DAURate = CalRate(TodayDAU,YesterdayDAU);
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
                s = "{\"name\"" +":"+ "\""+item.name + "\"" + ","+ "\"value\"" +":"+ item.value+"},"+s;
            }
            
            ViewBag.MapData = "[" +s+"]";


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
            List<ChartPoint> femaleValues = new List<ChartPoint>()
            {
                new ChartPoint(169.5, 67.3), new ChartPoint(160.0, 75.5), new ChartPoint(172.7, 68.2), new ChartPoint(162.6, 61.4), new ChartPoint(157.5, 76.8),
                new ChartPoint(176.5, 71.8), new ChartPoint(164.4, 55.5), new ChartPoint(160.7, 48.6), new ChartPoint(174.0, 66.4), new ChartPoint(163.8, 67.3)
            };


            List<ChartPoint> maleValues = new List<ChartPoint>()
            {
                
            
                new ChartPoint(174.0, 80.0), new ChartPoint(176.5, 82.3), new ChartPoint(180.3, 73.6), new ChartPoint(167.6, 74.1), new ChartPoint(188.0, 85.9),
                new ChartPoint(180.3, 73.2), new ChartPoint(167.6, 76.3), new ChartPoint(183.0, 65.9), new ChartPoint(183.0, 90.9), new ChartPoint(179.1, 89.1),
                new ChartPoint(170.2, 62.3), new ChartPoint(177.8, 82.7), new ChartPoint(179.1, 79.1), new ChartPoint(190.5, 98.2), new ChartPoint(177.8, 84.1),
                new ChartPoint(180.3, 83.2), new ChartPoint(180.3, 83.2)
            };

            List<ScatterSeriesData> femaleData = new List<ScatterSeriesData>();
            List<ScatterSeriesData> maleData = new List<ScatterSeriesData>();

            femaleValues.ForEach(p => femaleData.Add(new ScatterSeriesData { X = p.X, Y = p.Y }));
            maleValues.ForEach(p => maleData.Add(new ScatterSeriesData { X = p.X, Y = p.Y }));

            ViewData["femaleData"] = femaleData;
            ViewData["maleData"] = maleData;

            return View();
        }


        #endregion


    }
}