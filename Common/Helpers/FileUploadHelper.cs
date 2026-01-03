using System;
using System.IO;
using System.Linq;

namespace QRMenuSaaS.Common.Helpers
{
	public class FileUploadResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public string FilePath { get; set; }
		public string FileName { get; set; }
	}

	public static class FileUploadHelper
	{
		private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
		private static readonly long MaxFileSize = 5 * 1024 * 1024; // 5MB

		/// <summary>
		/// Görsel dosyasını upload eder (web bağımlılığı yoktur)
		/// </summary>
		public static FileUploadResult UploadImage(
			Stream fileStream,
			string originalFileName,
			string contentType,
			long contentLength,
			string uploadsRootFolder,
			int? tenantId = null)
		{
			if (fileStream == null || contentLength <= 0)
			{
				return new FileUploadResult { Success = false, Message = "Dosya seçilmedi" };
			}

			if (contentLength > MaxFileSize)
			{
				return new FileUploadResult
				{
					Success = false,
					Message = $"Dosya boyutu en fazla {MaxFileSize / 1024 / 1024}MB olabilir"
				};
			}

			string extension = Path.GetExtension(originalFileName)?.ToLowerInvariant() ?? "";
			if (!AllowedImageExtensions.Contains(extension))
			{
				return new FileUploadResult
				{
					Success = false,
					Message = "Sadece görsel dosyaları yüklenebilir (jpg, png, gif, webp)"
				};
			}

			if (string.IsNullOrWhiteSpace(contentType) || !contentType.StartsWith("image/"))
			{
				return new FileUploadResult { Success = false, Message = "Geçersiz dosya türü" };
			}

			try
			{
				// Tenant klasörü
				string tenantFolder = tenantId.HasValue ? tenantId.Value.ToString() : "default";
				string fullPath = Path.Combine(uploadsRootFolder, "Uploads", tenantFolder);

				if (!Directory.Exists(fullPath))
					Directory.CreateDirectory(fullPath);

				// Benzersiz dosya adı
				string fileName = $"{Guid.NewGuid():N}{extension}";
				string physicalPath = Path.Combine(fullPath, fileName);

				// Kaydet
				using (var fs = File.Create(physicalPath))
				{
					fileStream.CopyTo(fs);
				}

				// Web path döndür (uygulama içinde kullanılacak)
				string webPath = $"/Uploads/{tenantFolder}/{fileName}";

				return new FileUploadResult
				{
					Success = true,
					FilePath = webPath,
					FileName = fileName,
					Message = "Dosya başarıyla yüklendi"
				};
			}
			catch (Exception ex)
			{
				return new FileUploadResult { Success = false, Message = $"Dosya yüklenirken hata oluştu: {ex.Message}" };
			}
		}

		public static bool DeleteFile(string webPath, string webRootFolder)
		{
			try
			{
				if (string.IsNullOrEmpty(webPath))
					return true;

				string trimmed = webPath.TrimStart('~').TrimStart('/');
				string physicalPath = Path.Combine(webRootFolder, trimmed.Replace("/", "\\"));

				if (File.Exists(physicalPath))
				{
					File.Delete(physicalPath);
					return true;
				}

				return false;
			}
			catch
			{
				return false;
			}
		}
	}
}
