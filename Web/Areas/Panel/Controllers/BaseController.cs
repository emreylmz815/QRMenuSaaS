// ============================================
// Areas/Panel/Controllers/BaseController.cs (UPDATED)
// ============================================
using Newtonsoft.Json;
using QRMenuSaaS.Common.Constants;
using QRMenuSaaS.Web.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Panel.Controllers
{
	/// <summary>
	/// Tüm panel controllerları için base controller
	/// ViewBag'e otomatik olarak kullanıcı bilgisi ekler
	/// </summary>
	public abstract class BaseController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			// ViewBag'e kullanıcı bilgisini ekle
			ViewBag.CurrentUser = GetCurrentUser();
		}

		protected int GetCurrentTenantId()
		{
			// Önce HttpContext.Items'dan (TenantResolver tarafından eklenir)
			var tenantId = HttpContext.Items["TenantId"] as int?;
			if (tenantId.HasValue)
			{
				return tenantId.Value;
			}

			// Cookie'den de alabilir
			var cookie = Request.Cookies[AppConstants.TenantCookieName];
			if (cookie != null)
			{
				var userData = JsonConvert.DeserializeObject<UserCookieData>(cookie.Value);
				return userData?.TenantId ?? 0;
			}

			return 0;
		}

		protected UserCookieData GetCurrentUser()
		{
			var cookie = Request.Cookies[AppConstants.TenantCookieName];
			if (cookie != null)
			{
				return JsonConvert.DeserializeObject<UserCookieData>(cookie.Value);
			}
			return null;
		}

		protected int GetCurrentUserId()
		{
			var user = GetCurrentUser();
			return user?.UserId ?? 0;
		}

		/// <summary>
		/// Success mesajı göstermek için helper
		/// </summary>
		protected void SetSuccessMessage(string message)
		{
			TempData["Success"] = message;
		}

		/// <summary>
		/// Error mesajı göstermek için helper
		/// </summary>
		protected void SetErrorMessage(string message)
		{
			TempData["Error"] = message;
		}
	}
}