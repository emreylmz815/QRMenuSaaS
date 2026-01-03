// ============================================
// Areas/Panel/Controllers/AccountController.cs
// ============================================
using Newtonsoft.Json;
using QRMenuSaaS.Common.Constants;
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Enums;
using QRMenuSaaS.Data;
using QRMenuSaaS.Services;
using QRMenuSaaS.Web.Infrastructure;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QRMenuSaaS.Web.Areas.Panel.Controllers
{
	public class AccountController : Controller
	{
		private readonly AuthService _authService;

		public AccountController()
		{
			_authService = new AuthService(new DapperContext());
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Login()
		{
			var cookie = Request.Cookies[AppConstants.TenantCookieName];
			if (Request.IsAuthenticated && cookie != null)
			{
				return RedirectToAction("Index", "Dashboard");
			}

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginDTO model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var tenantId = HttpContext.Items["TenantId"] as int?;
			if (!tenantId.HasValue)
			{
				ModelState.AddModelError("", "Bu alan adı için geçerli bir işletme bulunamadı.");
				return View(model);
			}

			var result = _authService.LoginAsync(model.Email, model.Password, tenantId).Result;

			if (!result.Success || (result.User.Role != UserRole.TenantAdmin && result.User.Role != UserRole.TenantEditor))
			{
				ModelState.AddModelError("", "Geçersiz giriş bilgileri");
				return View(model);
			}

			var userData = new UserCookieData
			{
				UserId = result.User.Id,
				TenantId = tenantId,
				Email = result.User.Email,
				FullName = result.User.FullName,
				Role = result.User.Role
			};

			var cookie = new HttpCookie(AppConstants.TenantCookieName)
			{
				Value = JsonConvert.SerializeObject(userData),
				Expires = model.RememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(8),
				HttpOnly = true,
				Secure = Request.IsSecureConnection
			};

			Response.Cookies.Add(cookie);
			FormsAuthentication.SetAuthCookie(result.User.Email, model.RememberMe);

			return RedirectToAction("Index", "Dashboard");
		}

		[HttpGet]
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			var cookie = new HttpCookie(AppConstants.TenantCookieName)
			{
				Expires = DateTime.Now.AddDays(-1)
			};
			Response.Cookies.Add(cookie);
			return RedirectToAction("Login");
		}
	}
}
