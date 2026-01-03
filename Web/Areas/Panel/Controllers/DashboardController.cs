// ============================================
// Areas/Panel/Controllers/DashboardController.cs
// ============================================
using QRMenuSaaS.Core.Enums;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using QRMenuSaaS.Web.Infrastructure;
using System;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Panel.Controllers
{
	[TenantAuthorize(UserRole.TenantAdmin, UserRole.TenantEditor)]
	public class DashboardController : BaseController
	{
		private readonly CategoryRepository _categoryRepository;
		private readonly ProductRepository _productRepository;
		private readonly SubscriptionRepository _subscriptionRepository;

		public DashboardController()
		{
			var context = new DapperContext();
			_categoryRepository = new CategoryRepository(context);
			_productRepository = new ProductRepository(context);
			_subscriptionRepository = new SubscriptionRepository(context);
		}

		[HttpGet]
		public ActionResult Index()
		{
			var tenantId = GetCurrentTenantId();
			if (tenantId == 0)
			{
				return RedirectToAction("Login", "Account");
			}

			var categoryCount = _categoryRepository.GetCategoryCountByTenantAsync(tenantId).Result;
			var productCount = _productRepository.GetProductCountByTenantAsync(tenantId).Result;
			var subscription = _subscriptionRepository.GetActiveSubscriptionAsync(tenantId).Result;

			ViewBag.CategoryCount = categoryCount;
			ViewBag.ProductCount = productCount;
			ViewBag.Subscription = subscription;

			if (subscription != null)
			{
				ViewBag.SubscriptionStatus = GetSubscriptionStatusLabel(subscription.Status);
				var daysRemaining = (subscription.EndDate - DateTime.UtcNow).Days;
				ViewBag.DaysRemaining = Math.Max(daysRemaining, 0);
			}

			return View();
		}

		private static string GetSubscriptionStatusLabel(string status)
		{
			switch (status)
			{
				case SubscriptionStatus.Active:
					return "Aktif";
				case SubscriptionStatus.Expired:
					return "Süresi Doldu";
				case SubscriptionStatus.Cancelled:
					return "İptal Edildi";
				case SubscriptionStatus.Suspended:
					return "Askıya Alındı";
				default:
					return status;
			}
		}
	}
}
