//// ============================================
//// QRMenuSaaS.Common/Helpers/SlugHelper.cs
//// ============================================
//using System;
//using System.Globalization;
//using System.Security.Cryptography;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace QRMenuSaaS.Common.Helpers
//{
//	/// <summary>
//	/// Türkçe karakter normalize ederek URL-friendly slug üretir
//	/// </summary>
//	public static class SlugHelper
//	{
//		public static string GenerateSlug(string text)
//		{
//			if (string.IsNullOrWhiteSpace(text))
//				return string.Empty;

//			// Türkçe karakterleri normalize et
//			text = NormalizeTurkishCharacters(text);

//			// Küçük harfe çevir
//			text = text.ToLowerInvariant();

//			// Özel karakterleri kaldır, sadece harf, rakam ve tire bırak
//			text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

//			// Birden fazla boşluğu tek boşluğa çevir
//			text = Regex.Replace(text, @"\s+", " ").Trim();

//			// Boşlukları tire ile değiştir
//			text = Regex.Replace(text, @"\s", "-");

//			// Birden fazla tireyi tek tire yap
//			text = Regex.Replace(text, @"-+", "-");

//			// Baş ve son tireleri kaldır
//			text = text.Trim('-');

//			return text;
//		}

//		private static string NormalizeTurkishCharacters(string text)
//		{
//			var normalized = new StringBuilder();

//			foreach (char c in text)
//			{
//				switch (c)
//				{
//					case 'ş': normalized.Append('s'); break;
//					case 'Ş': normalized.Append('S'); break;
//					case 'ı': normalized.Append('i'); break;
//					case 'İ': normalized.Append('I'); break;
//					case 'ğ': normalized.Append('g'); break;
//					case 'Ğ': normalized.Append('G'); break;
//					case 'ü': normalized.Append('u'); break;
//					case 'Ü': normalized.Append('U'); break;
//					case 'ö': normalized.Append('o'); break;
//					case 'Ö': normalized.Append('O'); break;
//					case 'ç': normalized.Append('c'); break;
//					case 'Ç': normalized.Append('C'); break;
//					default: normalized.Append(c); break;
//				}
//			}

//			return normalized.ToString();
//		}

//		/// <summary>
//		/// Eğer slug zaten varsa sonuna sayı ekler (urun-adi-2)
//		/// </summary>
//		public static string EnsureUnique(string baseSlug, Func<string, bool> slugExists)
//		{
//			string slug = baseSlug;
//			int counter = 2;

//			while (slugExists(slug))
//			{
//				slug = $"{baseSlug}-{counter}";
//				counter++;
//			}

//			return slug;
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Common/Helpers/PasswordHelper.cs
//// ============================================
//using System;
//using System.Security.Cryptography;
//using System.Text;

//namespace QRMenuSaaS.Common.Helpers
//{
//	/// <summary>
//	/// PBKDF2 kullanarak güvenli password hashing
//	/// </summary>
//	public static class PasswordHelper
//	{
//		private const int SaltSize = 16; // 128 bit
//		private const int KeySize = 32; // 256 bit
//		private const int Iterations = 10000;
//		private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

//		/// <summary>
//		/// Şifre hash'ler ve salt ile birlikte döner
//		/// Format: {iterations}.{salt}.{hash}
//		/// </summary>
//		public static string HashPassword(string password)
//		{
//			if (string.IsNullOrEmpty(password))
//				throw new ArgumentNullException(nameof(password));

//			using (var rng = new RNGCryptoServiceProvider())
//			{
//				byte[] salt = new byte[SaltSize];
//				rng.GetBytes(salt);

//				byte[] hash = GenerateHash(password, salt, Iterations, KeySize);

//				return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
//			}
//		}

//		/// <summary>
//		/// Girilen şifreyi hash ile karşılaştırır
//		/// </summary>
//		public static bool VerifyPassword(string password, string hashedPassword)
//		{
//			if (string.IsNullOrEmpty(password))
//				throw new ArgumentNullException(nameof(password));

//			if (string.IsNullOrEmpty(hashedPassword))
//				throw new ArgumentNullException(nameof(hashedPassword));

//			try
//			{
//				string[] parts = hashedPassword.Split('.');
//				if (parts.Length != 3)
//					return false;

//				int iterations = int.Parse(parts[0]);
//				byte[] salt = Convert.FromBase64String(parts[1]);
//				byte[] hash = Convert.FromBase64String(parts[2]);

//				byte[] testHash = GenerateHash(password, salt, iterations, hash.Length);

//				return SlowEquals(hash, testHash);
//			}
//			catch
//			{
//				return false;
//			}
//		}

//		private static byte[] GenerateHash(string password, byte[] salt, int iterations, int length)
//		{
//			using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, Algorithm))
//			{
//				return pbkdf2.GetBytes(length);
//			}
//		}

//		/// <summary>
//		/// Timing attack'a karşı güvenli karşılaştırma
//		/// </summary>
//		private static bool SlowEquals(byte[] a, byte[] b)
//		{
//			uint diff = (uint)a.Length ^ (uint)b.Length;
//			for (int i = 0; i < a.Length && i < b.Length; i++)
//			{
//				diff |= (uint)(a[i] ^ b[i]);
//			}
//			return diff == 0;
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Common/Helpers/QRCodeHelper.cs
//// ============================================
//using QRCoder;
//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;

//namespace QRMenuSaaS.Common.Helpers
//{
//	/// <summary>
//	/// QRCoder kütüphanesi ile QR kod üretimi
//	/// NuGet: Install-Package QRCoder -Version 1.4.3
//	/// </summary>
//	public static class QRCodeHelper
//	{
//		/// <summary>
//		/// URL için QR kod üretir ve base64 string döner
//		/// </summary>
//		public static string GenerateQRCodeBase64(string url, int pixelsPerModule = 10)
//		{
//			using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
//			using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
//			using (QRCode qrCode = new QRCode(qrCodeData))
//			using (Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule))
//			using (MemoryStream ms = new MemoryStream())
//			{
//				qrCodeImage.Save(ms, ImageFormat.Png);
//				byte[] imageBytes = ms.ToArray();
//				return Convert.ToBase64String(imageBytes);
//			}
//		}

//		/// <summary>
//		/// QR kodu dosyaya kaydeder
//		/// </summary>
//		public static void SaveQRCodeToFile(string url, string filePath, int pixelsPerModule = 10)
//		{
//			using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
//			using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
//			using (QRCode qrCode = new QRCode(qrCodeData))
//			using (Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule))
//			{
//				qrCodeImage.Save(filePath, ImageFormat.Png);
//			}
//		}

//		/// <summary>
//		/// Tenant için menü URL'i üretir
//		/// </summary>
//		public static string GetMenuUrl(string subdomain, string baseUrl = "")
//		{
//			if (string.IsNullOrEmpty(baseUrl))
//			{
//				// Production'da domain.com olacak
//				baseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"] ?? "domain.com";
//			}

//			return $"https://{subdomain}.{baseUrl}";
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Common/Helpers/FileUploadHelper.cs
//// ============================================
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;

//namespace QRMenuSaaS.Common.Helpers
//{
//	public class FileUploadResult
//	{
//		public bool Success { get; set; }
//		public string Message { get; set; }
//		public string FilePath { get; set; }
//		public string FileName { get; set; }
//	}

//	public static class FileUploadHelper
//	{
//		private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
//		private static readonly long MaxFileSize = 5 * 1024 * 1024; // 5MB

//		/// <summary>
//		/// Görsel dosyasını upload eder ve güvenlik kontrollerini yapar
//		/// </summary>
//		public static FileUploadResult UploadImage(HttpPostedFileBase file, string uploadFolder, int? tenantId = null)
//		{
//			if (file == null || file.ContentLength == 0)
//			{
//				return new FileUploadResult
//				{
//					Success = false,
//					Message = "Dosya seçilmedi"
//				};
//			}

//			// Dosya boyutu kontrolü
//			if (file.ContentLength > MaxFileSize)
//			{
//				return new FileUploadResult
//				{
//					Success = false,
//					Message = $"Dosya boyutu en fazla {MaxFileSize / 1024 / 1024}MB olabilir"
//				};
//			}

//			// Uzantı kontrolü
//			string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
//			if (!AllowedImageExtensions.Contains(extension))
//			{
//				return new FileUploadResult
//				{
//					Success = false,
//					Message = "Sadece görsel dosyaları yüklenebilir (jpg, png, gif, webp)"
//				};
//			}

//			// MIME type kontrolü (güvenlik)
//			if (!file.ContentType.StartsWith("image/"))
//			{
//				return new FileUploadResult
//				{
//					Success = false,
//					Message = "Geçersiz dosya türü"
//				};
//			}

//			try
//			{
//				// Tenant klasörü oluştur
//				string tenantFolder = tenantId.HasValue ? tenantId.Value.ToString() : "default";
//				string fullPath = Path.Combine(uploadFolder, tenantFolder);

//				if (!Directory.Exists(fullPath))
//				{
//					Directory.CreateDirectory(fullPath);
//				}

//				// Benzersiz dosya adı oluştur
//				string fileName = $"{Guid.NewGuid()}{extension}";
//				string filePath = Path.Combine(fullPath, fileName);

//				// Dosyayı kaydet
//				file.SaveAs(filePath);

//				// Web path döndür
//				string webPath = $"/Uploads/{tenantFolder}/{fileName}";

//				return new FileUploadResult
//				{
//					Success = true,
//					FilePath = webPath,
//					FileName = fileName,
//					Message = "Dosya başarıyla yüklendi"
//				};
//			}
//			catch (Exception ex)
//			{
//				return new FileUploadResult
//				{
//					Success = false,
//					Message = $"Dosya yüklenirken hata oluştu: {ex.Message}"
//				};
//			}
//		}

//		/// <summary>
//		/// Dosyayı siler
//		/// </summary>
//		public static bool DeleteFile(string webPath, string uploadsFolder)
//		{
//			try
//			{
//				if (string.IsNullOrEmpty(webPath))
//					return true;

//				// Web path'i fiziksel path'e çevir
//				string fileName = Path.GetFileName(webPath);
//				string directory = Path.GetDirectoryName(webPath).Replace("/", "\\").TrimStart('\\');
//				string fullPath = Path.Combine(uploadsFolder, directory, fileName);

//				if (File.Exists(fullPath))
//				{
//					File.Delete(fullPath);
//					return true;
//				}

//				return false;
//			}
//			catch
//			{
//				return false;
//			}
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Common/Extensions/DateTimeExtensions.cs
//// ============================================
//using System;

//namespace QRMenuSaaS.Common.Extensions
//{
//	public static class DateTimeExtensions
//	{
//		private static readonly TimeZoneInfo TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

//		/// <summary>
//		/// UTC'den Türkiye saatine çevirir
//		/// </summary>
//		public static DateTime ToTurkeyTime(this DateTime utcDateTime)
//		{
//			if (utcDateTime.Kind != DateTimeKind.Utc)
//				utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

//			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TurkeyTimeZone);
//		}

//		/// <summary>
//		/// Türkiye saatinden UTC'ye çevirir
//		/// </summary>
//		public static DateTime ToUtcFromTurkey(this DateTime turkeyDateTime)
//		{
//			return TimeZoneInfo.ConvertTimeToUtc(turkeyDateTime, TurkeyTimeZone);
//		}

//		/// <summary>
//		/// Kullanıcı dostu tarih formatı
//		/// </summary>
//		public static string ToFriendlyDate(this DateTime dateTime)
//		{
//			var localTime = dateTime.ToTurkeyTime();
//			var today = DateTime.UtcNow.ToTurkeyTime().Date;
//			var targetDate = localTime.Date;

//			if (targetDate == today)
//				return "Bugün " + localTime.ToString("HH:mm");

//			if (targetDate == today.AddDays(-1))
//				return "Dün " + localTime.ToString("HH:mm");

//			if (targetDate > today.AddDays(-7))
//				return localTime.ToString("dddd HH:mm");

//			return localTime.ToString("dd.MM.yyyy HH:mm");
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Common/Constants/AppConstants.cs
//// ============================================
//namespace QRMenuSaaS.Common.Constants
//{
//	public static class AppConstants
//	{
//		public const string SuperAdminCookieName = "SuperAdminAuth";
//		public const string TenantCookieName = "TenantAuth";

//		public const string TenantCacheKeyPrefix = "Tenant_";
//		public const int TenantCacheMinutes = 60;

//		public const string DefaultCurrency = "TRY";
//		public const string DefaultFontFamily = "Roboto";

//		public const int DefaultPageSize = 20;
//		public const int MaxPageSize = 100;
//	}

//	public static class CacheKeys
//	{
//		public static string TenantBySubdomain(string subdomain) => $"Tenant_Subdomain_{subdomain}";
//		public static string TenantById(int tenantId) => $"Tenant_Id_{tenantId}";
//		public static string ActiveSubscription(int tenantId) => $"Subscription_Tenant_{tenantId}";
//	}
//}