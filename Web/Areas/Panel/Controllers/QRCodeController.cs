// ============================================
// Areas/Panel/Controllers/QRCodeController.cs
// ============================================
using QRMenuSaaS.Common.Helpers;
using QRMenuSaaS.Core.Enums;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using QRMenuSaaS.Web.Infrastructure;
using System;
using System.Web.Mvc;

namespace QRMenuSaaS.Web.Areas.Panel.Controllers
{
	[TenantAuthorize(UserRole.TenantAdmin, UserRole.TenantEditor)]
	public class QRCodeController : BaseController
	{
		private readonly TenantRepository _tenantRepository;

		public QRCodeController()
		{
			_tenantRepository = new TenantRepository(new DapperContext());
		}

		// GET: /panel/qrcode
		[HttpGet]
		public ActionResult Index()
		{
			var tenantId = GetCurrentTenantId();
			var tenant = _tenantRepository.GetByIdAsync(tenantId).Result;

			if (tenant == null)
			{
				return HttpNotFound();
			}

			// Tenant subdomain'ini al
			var tenantDomain = _tenantRepository.GetDomainBySubdomainAsync(tenant.Slug).Result;

			// Menü URL'i oluştur
			string menuUrl = QRCodeHelper.GetMenuUrl(tenantDomain?.Subdomain ?? tenant.Slug);

			// QR kod üret (base64)
			string qrCodeBase64 = QRCodeHelper.GenerateQRCodeBase64(menuUrl, 10);

			ViewBag.MenuUrl = menuUrl;
			ViewBag.QRCodeBase64 = qrCodeBase64;
			ViewBag.TenantName = tenant.Name;

			return View();
		}

		// GET: /panel/qrcode/download
		[HttpGet]
		public ActionResult Download()
		{
			var tenantId = GetCurrentTenantId();
			var tenant = _tenantRepository.GetByIdAsync(tenantId).Result;

			if (tenant == null)
			{
				return HttpNotFound();
			}

			var tenantDomain = _tenantRepository.GetDomainBySubdomainAsync(tenant.Slug).Result;
			string menuUrl = QRCodeHelper.GetMenuUrl(tenantDomain?.Subdomain ?? tenant.Slug);

			// QR kodu geçici dosyaya kaydet
			string tempPath = Server.MapPath("~/App_Data/Temp");
			if (!System.IO.Directory.Exists(tempPath))
			{
				System.IO.Directory.CreateDirectory(tempPath);
			}

			string fileName = $"QRCode_{tenant.Slug}_{DateTime.Now:yyyyMMddHHmmss}.png";
			string filePath = System.IO.Path.Combine(tempPath, fileName);

			QRCodeHelper.SaveQRCodeToFile(menuUrl, filePath, 10);

			// Dosyayı indir ve sil
			byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
			System.IO.File.Delete(filePath);

			return File(fileBytes, "image/png", fileName);
		}

		// GET: /panel/qrcode/print
		[HttpGet]
		public ActionResult Print()
		{
			var tenantId = GetCurrentTenantId();
			var tenant = _tenantRepository.GetByIdAsync(tenantId).Result;

			if (tenant == null)
			{
				return HttpNotFound();
			}

			var tenantDomain = _tenantRepository.GetDomainBySubdomainAsync(tenant.Slug).Result;
			string menuUrl = QRCodeHelper.GetMenuUrl(tenantDomain?.Subdomain ?? tenant.Slug);
			string qrCodeBase64 = QRCodeHelper.GenerateQRCodeBase64(menuUrl, 15); // Daha büyük

			ViewBag.MenuUrl = menuUrl;
			ViewBag.QRCodeBase64 = qrCodeBase64;
			ViewBag.TenantName = tenant.Name;

			return View();
		}
	}
}
