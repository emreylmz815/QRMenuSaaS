// ============================================
// QRMenuSaaS.Core/Entities/Theme.cs
// ============================================
using QRMenuSaaS.Core.Entities;

namespace QRMenuSaaS.Core.Entities
{
	public class Theme : BaseEntity
	{
		public int TenantId { get; set; }
		public string PrimaryColor { get; set; }
		public string SecondaryColor { get; set; }
		public string FontFamily { get; set; }
		public string LogoUrl { get; set; }
		public string CoverImageUrl { get; set; }
		public string CustomCss { get; set; }
	}
}