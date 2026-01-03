// ============================================
// QRMenuSaaS.Web/App_Start/RouteConfig.cs
// ============================================
using System.Web.Mvc;
using System.Web.Routing;

namespace QRMenuSaaS.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Public menü routes (tenant subdomain'lerde çalışır)
			routes.MapRoute(
				name: "PublicMenu",
				url: "",
				defaults: new { controller = "Menu", action = "Index" },
				namespaces: new[] { "QRMenuSaaS.Web.Controllers" }
			);

			routes.MapRoute(
				name: "PublicMenuCategory",
				url: "category/{slug}",
				defaults: new { controller = "Menu", action = "Category" },
				namespaces: new[] { "QRMenuSaaS.Web.Controllers" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "QRMenuSaaS.Web.Controllers" }
			);
		}
	}
}