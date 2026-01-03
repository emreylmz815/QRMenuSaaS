// ============================================
// QRMenuSaaS.Services/ThemeService.cs
// ============================================
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace QRMenuSaaS.Services
{
	public class ThemeService
	{
		private readonly ThemeRepository _themeRepository;

		public ThemeService(DapperContext context)
		{
			_themeRepository = new ThemeRepository(context);
		}

		public async Task<Theme> GetThemeAsync(int tenantId)
		{
			return await _themeRepository.GetByTenantIdAsync(tenantId);
		}

		public async Task<(bool Success, string Message)> SaveThemeAsync(ThemeDTO dto, int tenantId)
		{
			try
			{
				var theme = new Theme
				{
					TenantId = tenantId,
					PrimaryColor = dto.PrimaryColor,
					SecondaryColor = dto.SecondaryColor,
					FontFamily = dto.FontFamily,
					LogoUrl = dto.LogoUrl,
					CoverImageUrl = dto.CoverImageUrl
				};

				await _themeRepository.UpsertAsync(theme);

				return (true, "Tema ayarları başarıyla kaydedildi");
			}
			catch (Exception ex)
			{
				return (false, $"Tema kaydedilirken hata: {ex.Message}");
			}
		}
	}
}