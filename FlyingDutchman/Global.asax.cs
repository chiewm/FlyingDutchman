using FlyingDutchman.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace FlyingDutchman
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private MyShopDbContext db = new MyShopDbContext();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
          
    }


    }
}
