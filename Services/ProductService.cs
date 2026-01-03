// ============================================
// QRMenuSaaS.Services/ProductService.cs
// ============================================
using QRMenuSaaS.Common.Helpers;
using QRMenuSaaS.Core.DTOs;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QRMenuSaaS.Services
{
	public class ProductService
	{
		private readonly ProductRepository _productRepository;
		private readonly string _uploadsFolder;

		public ProductService(DapperContext context, string uploadsFolder)
		{
			_productRepository = new ProductRepository(context);
			_uploadsFolder = uploadsFolder;
		}

		public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int tenantId)
		{
			var products = await _productRepository.GetAllAsync(tenantId);

			return products.Select(p => new ProductDTO
			{
				Id = p.Id,
				CategoryId = p.CategoryId,
				Name = p.Name,
				Description = p.Description,
				Price = p.Price,
				ImageUrl = p.ImageUrl,
				SortOrder = p.SortOrder,
				IsActive = p.IsActive,
				CategoryName = p.CategoryName
			});
		}

		public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, int tenantId)
		{
			return await _productRepository.GetByCategoryAsync(categoryId, tenantId);
		}

		public async Task<Product> GetByIdAsync(int id, int tenantId)
		{
			return await _productRepository.GetByIdAsync(id, tenantId);
		}

		//public async Task<(bool Success, string Message, int ProductId)> CreateProductAsync(ProductDTO dto, int tenantId)
		//{
		//	try
		//	{
		//		// Görsel yükleme
		//		string imageUrl = null;
		//		if (dto.ImageFile != null && dto.ImageFile.ContentLength > 0)
		//		{
		//			var uploadResult = FileUploadHelper.UploadImage(dto.ImageFile, _uploadsFolder, tenantId);
		//			if (!uploadResult.Success)
		//			{
		//				return (false, uploadResult.Message, 0);
		//			}
		//			imageUrl = uploadResult.FilePath;
		//		}

		//		// Slug oluştur
		//		string slug = SlugHelper.GenerateSlug(dto.Name);
		//		var existingProduct = await _productRepository.GetBySlugAsync(slug, tenantId);
		//		if (existingProduct != null)
		//		{
		//			slug = SlugHelper.EnsureUnique(slug, s =>
		//				_productRepository.GetBySlugAsync(s, tenantId).Result != null);
		//		}

		//		var product = new Product
		//		{
		//			TenantId = tenantId,
		//			CategoryId = dto.CategoryId,
		//			Name = dto.Name,
		//			Slug = slug,
		//			Description = dto.Description,
		//			Price = dto.Price,
		//			Currency = "TRY",
		//			ImageUrl = imageUrl,
		//			SortOrder = dto.SortOrder,
		//			IsActive = dto.IsActive
		//		};

		//		int productId = await _productRepository.InsertAsync(product);

		//		return (true, "Ürün başarıyla oluşturuldu", productId);
		//	}
		//	catch (Exception ex)
		//	{
		//		return (false, $"Ürün oluşturulurken hata: {ex.Message}", 0);
		//	}
		//}

		//public async Task<(bool Success, string Message)> UpdateProductAsync(ProductDTO dto, int tenantId)
		//{
		//	try
		//	{
		//		var product = await _productRepository.GetByIdAsync(dto.Id, tenantId);
		//		if (product == null)
		//		{
		//			return (false, "Ürün bulunamadı");
		//		}

		//		// Yeni görsel yükleme
		//		if (dto.ImageFile != null && dto.ImageFile.ContentLength > 0)
		//		{
		//			// Eski görseli sil
		//			if (!string.IsNullOrEmpty(product.ImageUrl))
		//			{
		//				FileUploadHelper.DeleteFile(product.ImageUrl, _uploadsFolder);
		//			}

		//			var uploadResult = FileUploadHelper.UploadImage(dto.ImageFile, _uploadsFolder, tenantId);
		//			if (!uploadResult.Success)
		//			{
		//				return (false, uploadResult.Message);
		//			}
		//			product.ImageUrl = uploadResult.FilePath;
		//		}

		//		// İsim değiştiyse slug'ı güncelle
		//		if (product.Name != dto.Name)
		//		{
		//			string newSlug = SlugHelper.GenerateSlug(dto.Name);
		//			var existingProduct = await _productRepository.GetBySlugAsync(newSlug, tenantId);

		//			if (existingProduct != null && existingProduct.Id != dto.Id)
		//			{
		//				newSlug = SlugHelper.EnsureUnique(newSlug, s =>
		//				{
		//					var prod = _productRepository.GetBySlugAsync(s, tenantId).Result;
		//					return prod != null && prod.Id != dto.Id;
		//				});
		//			}

		//			product.Slug = newSlug;
		//		}

		//		product.CategoryId = dto.CategoryId;
		//		product.Name = dto.Name;
		//		product.Description = dto.Description;
		//		product.Price = dto.Price;
		//		product.SortOrder = dto.SortOrder;
		//		product.IsActive = dto.IsActive;

		//		bool updated = await _productRepository.UpdateAsync(product);

		//		return updated
		//			? (true, "Ürün başarıyla güncellendi")
		//			: (false, "Ürün güncellenemedi");
		//	}
		//	catch (Exception ex)
		//	{
		//		return (false, $"Ürün güncellenirken hata: {ex.Message}");
		//	}
		//}

		public async Task<(bool Success, string Message)> DeleteProductAsync(int id, int tenantId)
		{
			try
			{
				var product = await _productRepository.GetByIdAsync(id, tenantId);
				if (product == null)
				{
					return (false, "Ürün bulunamadı");
				}

				// Görseli sil
				if (!string.IsNullOrEmpty(product.ImageUrl))
				{
					FileUploadHelper.DeleteFile(product.ImageUrl, _uploadsFolder);
				}

				bool deleted = await _productRepository.SoftDeleteAsync(id, tenantId);

				return deleted
					? (true, "Ürün başarıyla silindi")
					: (false, "Ürün silinemedi");
			}
			catch (Exception ex)
			{
				return (false, $"Ürün silinirken hata: {ex.Message}");
			}
		}
	}
}