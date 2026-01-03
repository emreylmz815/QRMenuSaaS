// ============================================
// QRMenuSaaS.Services/CategoryService.cs
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

namespace QRMenuSaaS.Services
{
	public class CategoryService
	{
		private readonly CategoryRepository _categoryRepository;

		public CategoryService(DapperContext context)
		{
			_categoryRepository = new CategoryRepository(context);
		}

		public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(int tenantId)
		{
			var categories = await _categoryRepository.GetAllAsync(tenantId);

			var categoryDTOs = new List<CategoryDTO>();
			foreach (var category in categories)
			{
				int productCount = await _categoryRepository.GetProductCountAsync(category.Id, tenantId);

				categoryDTOs.Add(new CategoryDTO
				{
					Id = category.Id,
					Name = category.Name,
					Description = category.Description,
					SortOrder = category.SortOrder,
					IsActive = category.IsActive,
					ProductCount = productCount
				});
			}

			return categoryDTOs.OrderBy(c => c.SortOrder).ThenBy(c => c.Name);
		}

		public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(int tenantId)
		{
			return await _categoryRepository.GetActiveCategoriesAsync(tenantId);
		}

		public async Task<Category> GetByIdAsync(int id, int tenantId)
		{
			return await _categoryRepository.GetByIdAsync(id, tenantId);
		}

		public async Task<(bool Success, string Message, int CategoryId)> CreateCategoryAsync(CategoryDTO dto, int tenantId)
		{
			try
			{
				// Slug oluştur
				string slug = SlugHelper.GenerateSlug(dto.Name);

				// Slug benzersizliğini kontrol et
				var existingCategory = await _categoryRepository.GetBySlugAsync(slug, tenantId);
				if (existingCategory != null)
				{
					slug = SlugHelper.EnsureUnique(slug, s =>
						_categoryRepository.GetBySlugAsync(s, tenantId).Result != null);
				}

				var category = new Category
				{
					TenantId = tenantId,
					Name = dto.Name,
					Slug = slug,
					Description = dto.Description,
					SortOrder = dto.SortOrder,
					IsActive = dto.IsActive
				};

				int categoryId = await _categoryRepository.InsertAsync(category);

				return (true, "Kategori başarıyla oluşturuldu", categoryId);
			}
			catch (Exception ex)
			{
				return (false, $"Kategori oluşturulurken hata: {ex.Message}", 0);
			}
		}

		public async Task<(bool Success, string Message)> UpdateCategoryAsync(CategoryDTO dto, int tenantId)
		{
			try
			{
				var category = await _categoryRepository.GetByIdAsync(dto.Id, tenantId);
				if (category == null)
				{
					return (false, "Kategori bulunamadı");
				}

				// Eğer isim değiştiyse slug'ı güncelle
				if (category.Name != dto.Name)
				{
					string newSlug = SlugHelper.GenerateSlug(dto.Name);
					var existingCategory = await _categoryRepository.GetBySlugAsync(newSlug, tenantId);

					if (existingCategory != null && existingCategory.Id != dto.Id)
					{
						newSlug = SlugHelper.EnsureUnique(newSlug, s =>
						{
							var cat = _categoryRepository.GetBySlugAsync(s, tenantId).Result;
							return cat != null && cat.Id != dto.Id;
						});
					}

					category.Slug = newSlug;
				}

				category.Name = dto.Name;
				category.Description = dto.Description;
				category.SortOrder = dto.SortOrder;
				category.IsActive = dto.IsActive;

				bool updated = await _categoryRepository.UpdateAsync(category);

				return updated
					? (true, "Kategori başarıyla güncellendi")
					: (false, "Kategori güncellenemedi");
			}
			catch (Exception ex)
			{
				return (false, $"Kategori güncellenirken hata: {ex.Message}");
			}
		}

		public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id, int tenantId)
		{
			try
			{
				// Kategoride ürün var mı kontrol et
				int productCount = await _categoryRepository.GetProductCountAsync(id, tenantId);
				if (productCount > 0)
				{
					return (false, "Bu kategoride ürün bulunmaktadır. Önce ürünleri silin veya başka kategoriye taşıyın.");
				}

				bool deleted = await _categoryRepository.SoftDeleteAsync(id, tenantId);

				return deleted
					? (true, "Kategori başarıyla silindi")
					: (false, "Kategori silinemedi");
			}
			catch (Exception ex)
			{
				return (false, $"Kategori silinirken hata: {ex.Message}");
			}
		}
	}
}
