// ============================================
// Areas/Admin/Controllers/AccountController.cs
// ============================================
using Newtonsoft.Json; 
using QRMenuSaaS.Web.Infrastructure;
using QRMenuSaaS.Common.Helpers;
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System;
using System.Web;
using System.Web.Mvc;
using QRMenuSaaS.Services;
using QRMenuSaaS.Common.Constants;

namespace QRMenuSaaS.Web.Areas.Admin.Controllers
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
			var cookie = Request.Cookies[AppConstants.SuperAdminCookieName];
			if (cookie != null)
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

			// Super admin girişi (tenantId = null)
			var result = _authService.LoginAsync(model.Email, model.Password, null).Result;

			if (!result.Success || result.User.Role != "superadmin")
			{
				ModelState.AddModelError("", "Geçersiz giriş bilgileri");
				return View(model);
			}

			var userData = new UserCookieData
			{
				UserId = result.User.Id,
				TenantId = null,
				Email = result.User.Email,
				FullName = result.User.FullName,
				Role = result.User.Role
			};

			var cookie = new HttpCookie(AppConstants.SuperAdminCookieName)
			{
				Value = JsonConvert.SerializeObject(userData),
				Expires = model.RememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(8),
				HttpOnly = true,
				Secure = Request.IsSecureConnection
			};

			Response.Cookies.Add(cookie);
			return RedirectToAction("Index", "Dashboard");
		}

		[HttpGet]
		public ActionResult Logout()
		{
			var cookie = new HttpCookie(AppConstants.SuperAdminCookieName)
			{
				Expires = DateTime.Now.AddDays(-1)
			};
			Response.Cookies.Add(cookie);
			return RedirectToAction("Login");
		}
	}
}