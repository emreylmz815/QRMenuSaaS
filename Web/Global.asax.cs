using System.Web;
using System.Web.Mvc;
using System.Web.Optimization; // 1. Bu namespace'i eklediðinizden emin olun
using System.Web.Routing;
using Web;

namespace QRMenuSaaS.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			// 2. EKSÝK OLAN KRÝTÝK SATIR BURASI:
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// Hangfire baþlatma (opsiyonel)
			// HangfireConfig.Configure();
		}

		protected void Application_BeginRequest()
		{
			// Mevcut tenant kodlarýnýz...
			var context = new QRMenuSaaS.Data.DapperContext();
			var tenantResolver = new Infrastructure.TenantResolver(context);
			var tenant = tenantResolver.ResolveTenant();

			if (tenant != null)
			{
				HttpContext.Current.Items["Tenant"] = tenant;
				HttpContext.Current.Items["TenantId"] = tenant.Id;
			}
		}
	}
}