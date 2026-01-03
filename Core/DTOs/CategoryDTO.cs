// ============================================
// QRMenuSaaS.Core/DTOs/CategoryDTO.cs
// ============================================
using System.ComponentModel.DataAnnotations;

namespace QRMenuSaaS.Core.DTOs
{
	public class CategoryDTO
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Kategori adı gereklidir")]
		[MaxLength(200)]
		public string Name { get; set; }

		public string Description { get; set; }
		public int SortOrder { get; set; }
		public bool IsActive { get; set; }
		public int ProductCount { get; set; }
	}
}