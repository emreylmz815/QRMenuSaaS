// ============================================
// QRMenuSaaS.Services/AuthService.cs
// ============================================
using QRMenuSaaS.Common.Helpers;
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace QRMenuSaaS.Services
{
	public class AuthService
	{
		private readonly UserRepository _userRepository;

		public AuthService(DapperContext context)
		{
			_userRepository = new UserRepository(context);
		}

		/// <summary>
		/// Kullanıcı girişi yapar
		/// </summary>
		public async Task<LoginResultDTO> LoginAsync(string email, string password, int? tenantId = null)
		{
			try
			{
				var user = await _userRepository.GetByEmailAsync(email, tenantId);

				if (user == null)
				{
					return new LoginResultDTO
					{
						Success = false,
						Message = "E-posta veya şifre hatalı"
					};
				}

				//if (!user.IsActive)
				//{
				//	return new LoginResultDTO
				//	{
				//		Success = false,
				//		Message = "Hesabınız aktif değil"
				//	};
				//}

				//if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
				//{
				//	return new LoginResultDTO
				//	{
				//		Success = false,
				//		Message = "E-posta veya şifre hatalı"
				//	};
				//}

				// Login zamanını güncelle
				await _userRepository.UpdateLastLoginAsync(user.Id);

				return new LoginResultDTO
				{
					Success = true,
					Message = "Giriş başarılı",
					User = user
				};
			}
			catch (Exception ex)
			{
				return new LoginResultDTO
				{
					Success = false,
					Message = $"Giriş sırasında hata oluştu: {ex.Message}"
				};
			}
		}

		/// <summary>
		/// Yeni kullanıcı oluşturur
		/// </summary>
		public async Task<(bool Success, string Message, int UserId)> CreateUserAsync(
			string email,
			string password,
			string firstName,
			string lastName,
			string role,
			int? tenantId = null)
		{
			try
			{
				// Email kontrolü
				var existingUser = await _userRepository.GetByEmailAsync(email, tenantId);
				if (existingUser != null)
				{
					return (false, "Bu e-posta adresi zaten kullanılıyor", 0);
				}

				var user = new User
				{
					TenantId = tenantId,
					Email = email,
					PasswordHash = PasswordHelper.HashPassword(password),
					FirstName = firstName,
					LastName = lastName,
					Role = role,
					IsActive = true
				};

				int userId = await _userRepository.InsertAsync(user);

				return (true, "Kullanıcı başarıyla oluşturuldu", userId);
			}
			catch (Exception ex)
			{
				return (false, $"Kullanıcı oluşturulurken hata: {ex.Message}", 0);
			}
		}
	}
}