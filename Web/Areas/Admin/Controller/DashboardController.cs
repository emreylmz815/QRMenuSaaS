// ============================================
// Areas/Admin/Controllers/DashboardController.cs
// ============================================
using QRMenuSaaS.Data;
using QRMenuSaaS.Web.Infrastructure;
using System.Web.Mvc;
using Dapper;

namespace QRMenuSaaS.Web.Areas.Admin.Controllers
{
	[SuperAdminAuthorize]
	public class DashboardController : Controller
	{
		private readonly DapperContext _context;

		public DashboardController()
		{
			_context = new DapperContext();
		}

		public ActionResult Index()
		{
			// İstatistikler
			var stats = _context.Connection.QueryFirst<dynamic>(@"
                SELECT 
                    (SELECT COUNT(*) FROM tenants WHERE is_deleted = FALSE) as tenant_count,
                    (SELECT COUNT(*) FROM tenants WHERE status = 'active' AND is_deleted = FALSE) as active_tenant_count,
                    (SELECT COUNT(*) FROM users WHERE is_deleted = FALSE) as user_count,
                    (SELECT COUNT(*) FROM subscriptions WHERE status = 'active') as active_subscription_count,
                    (SELECT SUM(amount) FROM payments WHERE status = 'completed' AND created_at >= NOW() - INTERVAL '30 days') as monthly_revenue
            ");

			ViewBag.Stats = stats;
			return View();
		}
	}
}