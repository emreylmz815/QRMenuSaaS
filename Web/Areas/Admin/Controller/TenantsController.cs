// ============================================
// Areas/Admin/Controllers/TenantsController.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using QRMenuSaaS.Web.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Admin.Controllers
{
	[SuperAdminAuthorize]
	public class TenantsController : Controller
	{
		private readonly TenantRepository _tenantRepository;
		private readonly DapperContext _context;

		public TenantsController()
		{
			_context = new DapperContext();
			_tenantRepository = new TenantRepository(_context);
		}

		public ActionResult Index()
		{
			var tenants = _context.Connection.Query<dynamic>(@"
    SELECT t.*, td.subdomain, s.status as subscription_status
    FROM tenants t
    LEFT JOIN tenant_domains td ON t.id = td.tenant_id AND td.is_primary = 1
    LEFT JOIN subscriptions s ON t.id = s.tenant_id AND s.status = 'active'
    WHERE t.is_deleted = 0
    ORDER BY t.created_at DESC
").ToList();

			return View(tenants);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Suspend(int id)
		{
			_context.Connection.Execute(@"
                UPDATE tenants 
                SET status = 'suspended', updated_at = NOW()
                WHERE id = @Id", new { Id = id });

			TempData["Success"] = "Tenant başarıyla askıya alındı";
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Resume(int id)
		{
			_context.Connection.Execute(@"
                UPDATE tenants 
                SET status = 'active', updated_at = NOW()
                WHERE id = @Id", new { Id = id });

			TempData["Success"] = "Tenant başarıyla aktif edildi";
			return RedirectToAction("Index");
		}

		public ActionResult Details(int id)
		{
			var tenant = _context.Connection.QueryFirst<dynamic>(@"
                SELECT 
                    t.*,
                    td.subdomain,
                    s.status as subscription_status,
                    s.start_date,
                    s.end_date,
                    p.name as plan_name
                FROM tenants t
                LEFT JOIN tenant_domains td ON t.id = td.tenant_id AND td.is_primary = TRUE
                LEFT JOIN subscriptions s ON t.id = s.tenant_id AND s.status = 'active'
                LEFT JOIN plans p ON s.plan_id = p.id
                WHERE t.id = @Id", new { Id = id });

			ViewBag.Tenant = tenant;

			// Kullanıcılar
			ViewBag.Users = _context.Connection.Query<User>(@"
                SELECT * FROM users 
                WHERE tenant_id = @TenantId AND is_deleted = FALSE",
				new { TenantId = id });

			// İstatistikler
			ViewBag.ProductCount = _context.Connection.ExecuteScalar<int>(@"
                SELECT COUNT(*) FROM products WHERE tenant_id = @TenantId AND is_deleted = FALSE",
				new { TenantId = id });

			ViewBag.CategoryCount = _context.Connection.ExecuteScalar<int>(@"
                SELECT COUNT(*) FROM categories WHERE tenant_id = @TenantId AND is_deleted = FALSE",
				new { TenantId = id });

			return View();
		}
	}
}