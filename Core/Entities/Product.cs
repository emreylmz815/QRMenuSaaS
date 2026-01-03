// ============================================
// QRMenuSaaS.Core/Entities/Product.cs
// ============================================
using QRMenuSaaS.Core.Entities;

namespace QRMenuSaaS.Core.Entities
{
	public class Product : TenantEntity
	{
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string Currency { get; set; }
		public string ImageUrl { get; set; }
		public int SortOrder { get; set; }
		public bool IsActive { get; set; }

		// Navigation (populated separately)
		public string CategoryName { get; set; }
	}
}
