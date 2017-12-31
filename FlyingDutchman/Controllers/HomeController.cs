using System.Linq;
using System.Net;
using System.Web.Mvc;
using FlyingDutchman.Models;
using System.IO;
using System.Text;

namespace FlyingDutchman.Controllers
{
    public class HomeController : Controller
    {
        private MyShopDbContext db = new MyShopDbContext();

        // GET: Home
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

  

        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ip,Operation,Good,Sex,Address,CreatedOn")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Ip = GetRemoteIP();
                //user.Sex = true;
                user.Address = GetAddress(user.Ip);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }


        public string GetRemoteIP()
        {
            string SourceIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"]; /* 存在 X-Forwarded-For */
            if (string.IsNullOrEmpty(SourceIP))
            {
                SourceIP = Request.ServerVariables["REMOTE_ADDR"];
            }

            return SourceIP;
        }

        public string GetAddress(string ip)
        {
            string url = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?ip=" + ip;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);    //创建一个请求示例
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //获取响应，即发送请求
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
            string[] html = streamReader.ReadToEnd().Split();
            return html[4];

        }

    }
}
