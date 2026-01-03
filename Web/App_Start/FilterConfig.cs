// ============================================
// QRMenuSaaS.Web/App_Start/FilterConfig.cs
// ============================================
using QRMenuSaaS.Web.Infrastructure;
using System.Web.Mvc;

namespace QRMenuSaaS.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new GlobalExceptionFilter());
		}
	}
}