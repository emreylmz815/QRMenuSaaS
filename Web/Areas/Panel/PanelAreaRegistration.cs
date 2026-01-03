// ============================================
// QRMenuSaaS.Web/Areas/Panel/PanelAreaRegistration.cs
// ============================================
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Panel
{
	public class PanelAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Panel";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Panel_default",
				"panel/{controller}/{action}/{id}",
				new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "QRMenuSaaS.Web.Areas.Panel.Controllers" }
			);
		}
	}
}