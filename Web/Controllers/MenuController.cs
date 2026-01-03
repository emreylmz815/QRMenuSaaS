// ============================================
// Controllers/MenuController.cs (PUBLIC)
// ============================================
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Controllers
{
	/// <summary>
	/// Public menü - subdomain.domain.com
	/// </summary>
	public class MenuController : Controller
	{
		private readonly CategoryRepository _categoryRepository;
		private readonly ProductRepository _productRepository;
		private readonly ThemeRepository _themeRepository;

		public MenuController()
		{
			var context = new DapperContext();
			_categoryRepository = new CategoryRepository(context);
			_productRepository = new ProductRepository(context);
			_themeRepository = new ThemeRepository(context);
		}

		// GET: / (ana menü)
		public ActionResult Index()
		{
			var tenant = HttpContext.Items["Tenant"] as Tenant;

			if (tenant == null)
			{
				return View("TenantNotFound");
			}

			// Tema ayarlarını getir
			var theme = _themeRepository.GetByTenantIdAsync(tenant.Id).Result;
			ViewBag.Theme = theme;

			// Kategorileri ve ürünleri getir
			var categories = _categoryRepository.GetActiveCategoriesAsync(tenant.Id).Result;
			var products = _productRepository.GetActiveProductsAsync(tenant.Id).Result;

			ViewBag.Tenant = tenant;
			ViewBag.Categories = categories;
			ViewBag.Products = products.GroupBy(p => p.CategoryId);

			return View();
		}

		// GET: /category/{slug}
		public ActionResult Category(string slug)
		{
			var tenant = HttpContext.Items["Tenant"] as Tenant;

			if (tenant == null)
			{
				return HttpNotFound();
			}

			var category = _categoryRepository.GetBySlugAsync(slug, tenant.Id).Result;

			if (category == null)
			{
				return HttpNotFound();
			}

			var products = _productRepository.GetByCategoryAsync(category.Id, tenant.Id).Result;
			var theme = _themeRepository.GetByTenantIdAsync(tenant.Id).Result;

			ViewBag.Tenant = tenant;
			ViewBag.Category = category;
			ViewBag.Theme = theme;
			ViewBag.Products = products;

			return View();
		}
	}
}