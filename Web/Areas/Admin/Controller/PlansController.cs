// ============================================
// Areas/Admin/Controllers/PlansController.cs
// ============================================
using Dapper;
using QRMenuSaaS.Data;
using QRMenuSaaS.Web.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Admin.Controllers
{
	[SuperAdminAuthorize]
	public class PlansController : Controller
	{
		private readonly DapperContext _context;

		public PlansController()
		{
			_context = new DapperContext();
		}

		public ActionResult Index()
		{
			var plans = _context.Connection.Query<dynamic>(@"
                SELECT *
                FROM plans
                ORDER BY sort_order ASC, created_at DESC
            ").ToList();

			return View(plans);
		}

		public ActionResult Details(int id)
		{
			var plan = _context.Connection.QueryFirstOrDefault<dynamic>(@"
                SELECT *
                FROM plans
                WHERE id = @Id
            ", new { Id = id });

			if (plan == null)
			{
				return HttpNotFound();
			}

			ViewBag.Plan = plan;
			return View();
		}
	}
}
