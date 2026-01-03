// ============================================
// QRMenuSaaS.Web/Areas/Admin/AdminAreaRegistration.cs
// ============================================
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Admin";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Admin_default",
				"admin/{controller}/{action}/{id}",
				new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "QRMenuSaaS.Web.Areas.Admin.Controllers" }
			);
		}
	}
}
