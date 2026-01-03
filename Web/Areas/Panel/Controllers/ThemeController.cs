// ============================================
// Areas/Panel/Controllers/ThemeController.cs
// ============================================
using QRMenuSaaS.Common.Helpers;
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Enums;
using QRMenuSaaS.Data;
using QRMenuSaaS.Services;
using QRMenuSaaS.Web.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Panel.Controllers
{
	[TenantAuthorize(UserRole.TenantAdmin)]
	public class ThemeController : BaseController
	{
		private readonly ThemeService _themeService;
		private readonly string _uploadsFolder;

		public ThemeController()
		{
			_themeService = new ThemeService(new DapperContext());
			_uploadsFolder = Server.MapPath("~/Uploads");
		}

		// GET: /panel/theme
		[HttpGet]
		public ActionResult Index()
		{
			var tenantId = GetCurrentTenantId();
			var theme = _themeService.GetThemeAsync(tenantId).Result;

			var dto = new ThemeDTO
			{
				PrimaryColor = theme?.PrimaryColor ?? "#e74c3c",
				SecondaryColor = theme?.SecondaryColor ?? "#34495e",
				FontFamily = theme?.FontFamily ?? "Roboto",
				LogoUrl = theme?.LogoUrl,
				CoverImageUrl = theme?.CoverImageUrl
			};

			return View(dto);
		}

		// POST: /panel/theme
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Index(ThemeDTO model, HttpPostedFileBase logoFile, HttpPostedFileBase coverFile)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return View(model);
		//	}

		//	var tenantId = GetCurrentTenantId();

		//	// Logo upload
		//	if (logoFile != null && logoFile.ContentLength > 0)
		//	{
		//		var uploadResult = FileUploadHelper.UploadImage(logoFile, _uploadsFolder, tenantId);
		//		if (!uploadResult.Success)
		//		{
		//			ModelState.AddModelError("", uploadResult.Message);
		//			return View(model);
		//		}
		//		model.LogoUrl = uploadResult.FilePath;
		//	}

		//	// Cover image upload
		//	if (coverFile != null && coverFile.ContentLength > 0)
		//	{
		//		var uploadResult = FileUploadHelper.UploadImage(coverFile, _uploadsFolder, tenantId);
		//		if (!uploadResult.Success)
		//		{
		//			ModelState.AddModelError("", uploadResult.Message);
		//			return View(model);
		//		}
		//		model.CoverImageUrl = uploadResult.FilePath;
		//	}

		//	// Tema kaydet
		//	var result = _themeService.SaveThemeAsync(model, tenantId).Result;

		//	if (!result.Success)
		//	{
		//		ModelState.AddModelError("", result.Message);
		//		return View(model);
		//	}

		//	SetSuccessMessage(result.Message);
		//	return RedirectToAction("Index");
		//}
	}
}
