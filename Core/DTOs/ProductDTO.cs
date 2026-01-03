// ============================================
// QRMenuSaaS.Core/DTOs/ProductDTO.cs
// ============================================
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace QRMenuSaaS.Core.DTOs
{
	public class ProductDTO
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Kategori seçiniz")]
		public int CategoryId { get; set; }

		[Required(ErrorMessage = "Ürün adı gereklidir")]
		[MaxLength(300)]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required(ErrorMessage = "Fiyat giriniz")]
		[Range(0, 999999)]
		public decimal Price { get; set; }

		public string ImageUrl { get; set; }
		public HttpPostedFileBase ImageFile { get; set; }
		public int SortOrder { get; set; }
		public bool IsActive { get; set; }

		public string CategoryName { get; set; }
	}
}
