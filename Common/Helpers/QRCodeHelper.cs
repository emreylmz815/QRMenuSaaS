// ============================================
// QRMenuSaaS.Common/Helpers/QRCodeHelper.cs
// ============================================
using iTextSharp.text.pdf.qrcode;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCode = QRCoder.QRCode;
using System.Configuration;

namespace QRMenuSaaS.Common.Helpers
{
	/// <summary>
	/// QRCoder kütüphanesi ile QR kod üretimi
	/// NuGet: Install-Package QRCoder -Version 1.4.3
	/// </summary>
	public static class QRCodeHelper
	{
		/// <summary>
		/// URL için QR kod üretir ve base64 string döner
		/// </summary>
		public static string GenerateQRCodeBase64(string url, int pixelsPerModule = 10)
		{
			using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
			using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
			using (QRCode qrCode = new QRCode(qrCodeData))
				// Dosyanın en üstüne ekleyin
			using (Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule))
			using (MemoryStream ms = new MemoryStream())
			{
				qrCodeImage.Save(ms, ImageFormat.Png);
				byte[] imageBytes = ms.ToArray();
				return Convert.ToBase64String(imageBytes);
			}
		}

		/// <summary>
		/// QR kodu dosyaya kaydeder
		/// </summary>
		public static void SaveQRCodeToFile(string url, string filePath, int pixelsPerModule = 10)
		{
			using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
			using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
			using (QRCode qrCode = new QRCode(qrCodeData))
			using (Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule))
			{
				qrCodeImage.Save(filePath, ImageFormat.Png);
			}
		}

		/// <summary>
		/// Tenant için menü URL'i üretir
		/// </summary>
		public static string GetMenuUrl(string subdomain, string baseUrl = "")
		{
			if (string.IsNullOrEmpty(baseUrl))
			{
				// Production'da domain.com olacak
				baseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"] ?? "domain.com";
			}

			return $"https://{subdomain}.{baseUrl}";
		}
	}
}