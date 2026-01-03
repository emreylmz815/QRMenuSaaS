// ============================================
// QRMenuSaaS.Core/DTOs/LoginDTO.cs
// ============================================
using QRMenuSaaS.Core.Entities;

namespace QRMenuSaaS.Core.DTOs
{
	public class LoginDTO
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}

	public class LoginResultDTO
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public User User { get; set; }
	}
}