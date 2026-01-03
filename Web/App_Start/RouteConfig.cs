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

			// 1. Ana Sayfa Rotası (localhost:44391 direkt buraya düşer)
			routes.MapRoute(
				name: "PublicHome",
				url: "",
				defaults: new { controller = "Home", action = "Index" },
				namespaces: new[] { "QRMenuSaaS.Web.Controllers" }
			);

			// 2. Kategori Rotası
			routes.MapRoute(
				name: "PublicMenuCategory",
				url: "category/{slug}",
				defaults: new { controller = "Menu", action = "Category" },
				namespaces: new[] { "QRMenuSaaS.Web.Controllers" }
			);

			// 3. Genel Varsayılan Rota (controller/action/id)
			routes.MapRoute(
					name: "Default",
					url: "{controller}/{action}/{id}",
					defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
					// ÖNEMLİ: Sadece ana klasördeki controller'lara bakmasını söyleyin
					namespaces: new[] { "QRMenuSaaS.Web.Controllers" }
				);
		}
	}
}