// ============================================
// QRMenuSaaS.Core/DTOs/ThemeDTO.cs
// ============================================
using System.ComponentModel.DataAnnotations;

namespace QRMenuSaaS.Core.DTOs
{
	public class ThemeDTO
	{
		[Required]
		[RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Geçerli renk kodu giriniz (#RRGGBB)")]
		public string PrimaryColor { get; set; }

		[Required]
		[RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Geçerli renk kodu giriniz (#RRGGBB)")]
		public string SecondaryColor { get; set; }

		[Required]
		public string FontFamily { get; set; }

		public string LogoUrl { get; set; }
		public string CoverImageUrl { get; set; }
	}
}
