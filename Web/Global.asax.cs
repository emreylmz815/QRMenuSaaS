// ============================================
// QRMenuSaaS.Web/Global.asax.cs
// ============================================
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QRMenuSaaS.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			// Hangfire baþlatma (opsiyonel)
			// HangfireConfig.Configure();
		}

		protected void Application_BeginRequest()
		{
			// Her request'te tenant'ý çözümle ve context'e ekle
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