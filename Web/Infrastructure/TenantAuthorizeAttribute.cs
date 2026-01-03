// ============================================
// QRMenuSaaS.Web/Infrastructure/TenantAuthorizeAttribute.cs
// ============================================
using QRMenuSaaS.Common.Constants;
using QRMenuSaaS.Core.Enums;
using System;
using System.Web;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Infrastructure
{
	/// <summary>
	/// Tenant scope'lu yetkilendirme
	/// Her istekte kullanıcının tenant'ına ait olduğunu kontrol eder
	/// </summary>
	public class TenantAuthorizeAttribute : AuthorizeAttribute
	{
		private readonly string[] _allowedRoles;

		public TenantAuthorizeAttribute(params string[] allowedRoles)
		{
			_allowedRoles = allowedRoles ?? new[] { UserRole.TenantAdmin, UserRole.TenantEditor };
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.Request.IsAuthenticated)
				return false;

			var cookie = httpContext.Request.Cookies[AppConstants.TenantCookieName];
			if (cookie == null)
				return false;

			try
			{
				// Cookie'den kullanıcı bilgilerini çöz
				var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCookieData>(cookie.Value);

				if (userData == null || !userData.TenantId.HasValue)
					return false;

				// Role kontrolü
				bool roleAuthorized = false;
				foreach (var role in _allowedRoles)
				{
					if (userData.Role == role)
					{
						roleAuthorized = true;
						break;
					}
				}

				if (!roleAuthorized)
					return false;

				// Tenant kontrolü - request'teki tenant ile kullanıcının tenant'ı eşleşmeli
				var tenantIdFromContext = httpContext.Items["TenantId"] as int?;
				if (tenantIdFromContext.HasValue && tenantIdFromContext.Value != userData.TenantId.Value)
					return false;

				return true;
			}
			catch
			{
				return false;
			}
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectResult("/panel/login");
		}
	}

	/// <summary>
	/// Super admin yetkilendirme
	/// </summary>
	public class SuperAdminAuthorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.Request.IsAuthenticated)
				return false;

			var cookie = httpContext.Request.Cookies[AppConstants.SuperAdminCookieName];
			if (cookie == null)
				return false;

			try
			{
				var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCookieData>(cookie.Value);
				return userData != null && userData.Role == UserRole.SuperAdmin;
			}
			catch
			{
				return false;
			}
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectResult("/admin/login");
		}
	}

	/// <summary>
	/// Cookie'de saklanan kullanıcı verisi
	/// </summary>
	public class UserCookieData
	{
		public int UserId { get; set; }
		public int? TenantId { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public string Role { get; set; }
	}
}