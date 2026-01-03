// ============================================
// QRMenuSaaS.Web/Infrastructure/GlobalExceptionFilter.cs
// ============================================
using System;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Infrastructure
{
	/// <summary>
	/// Global exception handling
	/// </summary>
	public class GlobalExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext filterContext)
		{
			if (filterContext.ExceptionHandled)
				return;

			// Log exception (örnek - gerçek uygulamada log framework kullanın)
			var exception = filterContext.Exception;
			System.Diagnostics.Debug.WriteLine($"Global Exception: {exception.Message}");
			System.Diagnostics.Debug.WriteLine(exception.StackTrace);

			// Kullanıcıya hata sayfası göster
			filterContext.Result = new ViewResult
			{
				ViewName = "~/Views/Shared/Error.cshtml",
				ViewData = new ViewDataDictionary<string>(exception.Message)
			};

			filterContext.ExceptionHandled = true;
		}
	}
}
