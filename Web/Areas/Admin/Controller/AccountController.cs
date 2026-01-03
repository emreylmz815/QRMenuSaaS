// ============================================
// Areas/Admin/Controllers/AccountController.cs
// ============================================
using Dapper;
using Newtonsoft.Json; 
using QRMenuSaaS.Common.Constants;
using QRMenuSaaS.Common.Helpers;
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using QRMenuSaaS.Services;
using QRMenuSaaS.Web.Infrastructure;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
			if (Request.IsAuthenticated && cookie != null)
			{
				return RedirectToAction("Index", "Dashboard");
			}
			return View();
		}

		// Geçici olarak AccountController içine yapıştır
		public async Task<ActionResult> FixPassword()
		{
			// admin@qrmenu.com adresli kullanıcının şifresini 123 olarak günceller
			string newHash = QRMenuSaaS.Common.Helpers.PasswordHelper.HashPassword("123");

			using (var db = new QRMenuSaaS.Data.DapperContext().Connection)
			{
				await db.ExecuteAsync("UPDATE users SET password_hash = @hash WHERE email = @email",
					new { hash = newHash, email = "test@test.com" });
			}

			return Content("Şifre başarıyla 123 olarak güncellendi ve hash'lendi!");
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginDTO model) // async Task eklendi
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				// .Result yerine await kullanarak kilitlenmeyi önlüyoruz
				var result = await _authService.LoginAsync(model.Email, model.Password, null);

				if (!result.Success || result.User.Role != "superadmin")
				{
					ModelState.AddModelError("", result.Message ?? "Geçersiz giriş bilgileri");
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
				FormsAuthentication.SetAuthCookie(result.User.Email, model.RememberMe);

				return RedirectToAction("Index", "Dashboard");
			}
			catch (Exception ex)
			{
				// Beklenmedik hataları yakalayıp kullanıcıya mesaj gösteriyoruz
				ModelState.AddModelError("", "Giriş işlemi sırasında bir teknik hata oluştu: " + ex.Message);
				return View(model);
			}
		}
		[HttpGet]
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			var cookie = new HttpCookie(AppConstants.SuperAdminCookieName)
			{
				Expires = DateTime.Now.AddDays(-1)
			};
			Response.Cookies.Add(cookie);
			return RedirectToAction("Login");
		}
	}
}
