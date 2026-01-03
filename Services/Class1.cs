//// ============================================
//// QRMenuSaaS.Services/AuthService.cs
//// ============================================
//using QRMenuSaaS.Common.Helpers;
//using QRMenuSaaS.Core.DTOs;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Data;
//using QRMenuSaaS.Data.Repositories;
//using System;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Services
//{
//	public class AuthService
//	{
//		private readonly UserRepository _userRepository;

//		public AuthService(DapperContext context)
//		{
//			_userRepository = new UserRepository(context);
//		}

//		/// <summary>
//		/// Kullanıcı girişi yapar
//		/// </summary>
//		public async Task<LoginResultDTO> LoginAsync(string email, string password, int? tenantId = null)
//		{
//			try
//			{
//				var user = await _userRepository.GetByEmailAsync(email, tenantId);

//				if (user == null)
//				{
//					return new LoginResultDTO
//					{
//						Success = false,
//						Message = "E-posta veya şifre hatalı"
//					};
//				}

//				if (!user.IsActive)
//				{
//					return new LoginResultDTO
//					{
//						Success = false,
//						Message = "Hesabınız aktif değil"
//					};
//				}

//				if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
//				{
//					return new LoginResultDTO
//					{
//						Success = false,
//						Message = "E-posta veya şifre hatalı"
//					};
//				}

//				// Login zamanını güncelle
//				await _userRepository.UpdateLastLoginAsync(user.Id);

//				return new LoginResultDTO
//				{
//					Success = true,
//					Message = "Giriş başarılı",
//					User = user
//				};
//			}
//			catch (Exception ex)
//			{
//				return new LoginResultDTO
//				{
//					Success = false,
//					Message = $"Giriş sırasında hata oluştu: {ex.Message}"
//				};
//			}
//		}

//		/// <summary>
//		/// Yeni kullanıcı oluşturur
//		/// </summary>
//		public async Task<(bool Success, string Message, int UserId)> CreateUserAsync(
//			string email,
//			string password,
//			string firstName,
//			string lastName,
//			string role,
//			int? tenantId = null)
//		{
//			try
//			{
//				// Email kontrolü
//				var existingUser = await _userRepository.GetByEmailAsync(email, tenantId);
//				if (existingUser != null)
//				{
//					return (false, "Bu e-posta adresi zaten kullanılıyor", 0);
//				}

//				var user = new User
//				{
//					TenantId = tenantId,
//					Email = email,
//					PasswordHash = PasswordHelper.HashPassword(password),
//					FirstName = firstName,
//					LastName = lastName,
//					Role = role,
//					IsActive = true
//				};

//				int userId = await _userRepository.InsertAsync(user);

//				return (true, "Kullanıcı başarıyla oluşturuldu", userId);
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Kullanıcı oluşturulurken hata: {ex.Message}", 0);
//			}
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Services/CategoryService.cs
//// ============================================
//using QRMenuSaaS.Common.Helpers;
//using QRMenuSaaS.Core.DTOs;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Data;
//using QRMenuSaaS.Data.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Services
//{
//	public class CategoryService
//	{
//		private readonly CategoryRepository _categoryRepository;

//		public CategoryService(DapperContext context)
//		{
//			_categoryRepository = new CategoryRepository(context);
//		}

//		public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(int tenantId)
//		{
//			var categories = await _categoryRepository.GetAllAsync(tenantId);

//			var categoryDTOs = new List<CategoryDTO>();
//			foreach (var category in categories)
//			{
//				int productCount = await _categoryRepository.GetProductCountAsync(category.Id, tenantId);

//				categoryDTOs.Add(new CategoryDTO
//				{
//					Id = category.Id,
//					Name = category.Name,
//					Description = category.Description,
//					SortOrder = category.SortOrder,
//					IsActive = category.IsActive,
//					ProductCount = productCount
//				});
//			}

//			return categoryDTOs.OrderBy(c => c.SortOrder).ThenBy(c => c.Name);
//		}

//		public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(int tenantId)
//		{
//			return await _categoryRepository.GetActiveCategoriesAsync(tenantId);
//		}

//		public async Task<Category> GetByIdAsync(int id, int tenantId)
//		{
//			return await _categoryRepository.GetByIdAsync(id, tenantId);
//		}

//		public async Task<(bool Success, string Message, int CategoryId)> CreateCategoryAsync(CategoryDTO dto, int tenantId)
//		{
//			try
//			{
//				// Slug oluştur
//				string slug = SlugHelper.GenerateSlug(dto.Name);

//				// Slug benzersizliğini kontrol et
//				var existingCategory = await _categoryRepository.GetBySlugAsync(slug, tenantId);
//				if (existingCategory != null)
//				{
//					slug = SlugHelper.EnsureUnique(slug, s =>
//						_categoryRepository.GetBySlugAsync(s, tenantId).Result != null);
//				}

//				var category = new Category
//				{
//					TenantId = tenantId,
//					Name = dto.Name,
//					Slug = slug,
//					Description = dto.Description,
//					SortOrder = dto.SortOrder,
//					IsActive = dto.IsActive
//				};

//				int categoryId = await _categoryRepository.InsertAsync(category);

//				return (true, "Kategori başarıyla oluşturuldu", categoryId);
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Kategori oluşturulurken hata: {ex.Message}", 0);
//			}
//		}

//		public async Task<(bool Success, string Message)> UpdateCategoryAsync(CategoryDTO dto, int tenantId)
//		{
//			try
//			{
//				var category = await _categoryRepository.GetByIdAsync(dto.Id, tenantId);
//				if (category == null)
//				{
//					return (false, "Kategori bulunamadı");
//				}

//				// Eğer isim değiştiyse slug'ı güncelle
//				if (category.Name != dto.Name)
//				{
//					string newSlug = SlugHelper.GenerateSlug(dto.Name);
//					var existingCategory = await _categoryRepository.GetBySlugAsync(newSlug, tenantId);

//					if (existingCategory != null && existingCategory.Id != dto.Id)
//					{
//						newSlug = SlugHelper.EnsureUnique(newSlug, s =>
//						{
//							var cat = _categoryRepository.GetBySlugAsync(s, tenantId).Result;
//							return cat != null && cat.Id != dto.Id;
//						});
//					}

//					category.Slug = newSlug;
//				}

//				category.Name = dto.Name;
//				category.Description = dto.Description;
//				category.SortOrder = dto.SortOrder;
//				category.IsActive = dto.IsActive;

//				bool updated = await _categoryRepository.UpdateAsync(category);

//				return updated
//					? (true, "Kategori başarıyla güncellendi")
//					: (false, "Kategori güncellenemedi");
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Kategori güncellenirken hata: {ex.Message}");
//			}
//		}

//		public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id, int tenantId)
//		{
//			try
//			{
//				// Kategoride ürün var mı kontrol et
//				int productCount = await _categoryRepository.GetProductCountAsync(id, tenantId);
//				if (productCount > 0)
//				{
//					return (false, "Bu kategoride ürün bulunmaktadır. Önce ürünleri silin veya başka kategoriye taşıyın.");
//				}

//				bool deleted = await _categoryRepository.SoftDeleteAsync(id, tenantId);

//				return deleted
//					? (true, "Kategori başarıyla silindi")
//					: (false, "Kategori silinemedi");
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Kategori silinirken hata: {ex.Message}");
//			}
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Services/ProductService.cs
//// ============================================
//using QRMenuSaaS.Common.Helpers;
//using QRMenuSaaS.Core.DTOs;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Data;
//using QRMenuSaaS.Data.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace QRMenuSaaS.Services
//{
//	public class ProductService
//	{
//		private readonly ProductRepository _productRepository;
//		private readonly string _uploadsFolder;

//		public ProductService(DapperContext context, string uploadsFolder)
//		{
//			_productRepository = new ProductRepository(context);
//			_uploadsFolder = uploadsFolder;
//		}

//		public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int tenantId)
//		{
//			var products = await _productRepository.GetAllAsync(tenantId);

//			return products.Select(p => new ProductDTO
//			{
//				Id = p.Id,
//				CategoryId = p.CategoryId,
//				Name = p.Name,
//				Description = p.Description,
//				Price = p.Price,
//				ImageUrl = p.ImageUrl,
//				SortOrder = p.SortOrder,
//				IsActive = p.IsActive,
//				CategoryName = p.CategoryName
//			});
//		}

//		public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, int tenantId)
//		{
//			return await _productRepository.GetByCategoryAsync(categoryId, tenantId);
//		}

//		public async Task<Product> GetByIdAsync(int id, int tenantId)
//		{
//			return await _productRepository.GetByIdAsync(id, tenantId);
//		}

//		public async Task<(bool Success, string Message, int ProductId)> CreateProductAsync(ProductDTO dto, int tenantId)
//		{
//			try
//			{
//				// Görsel yükleme
//				string imageUrl = null;
//				if (dto.ImageFile != null && dto.ImageFile.ContentLength > 0)
//				{
//					var uploadResult = FileUploadHelper.UploadImage(dto.ImageFile, _uploadsFolder, tenantId);
//					if (!uploadResult.Success)
//					{
//						return (false, uploadResult.Message, 0);
//					}
//					imageUrl = uploadResult.FilePath;
//				}

//				// Slug oluştur
//				string slug = SlugHelper.GenerateSlug(dto.Name);
//				var existingProduct = await _productRepository.GetBySlugAsync(slug, tenantId);
//				if (existingProduct != null)
//				{
//					slug = SlugHelper.EnsureUnique(slug, s =>
//						_productRepository.GetBySlugAsync(s, tenantId).Result != null);
//				}

//				var product = new Product
//				{
//					TenantId = tenantId,
//					CategoryId = dto.CategoryId,
//					Name = dto.Name,
//					Slug = slug,
//					Description = dto.Description,
//					Price = dto.Price,
//					Currency = "TRY",
//					ImageUrl = imageUrl,
//					SortOrder = dto.SortOrder,
//					IsActive = dto.IsActive
//				};

//				int productId = await _productRepository.InsertAsync(product);

//				return (true, "Ürün başarıyla oluşturuldu", productId);
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Ürün oluşturulurken hata: {ex.Message}", 0);
//			}
//		}

//		public async Task<(bool Success, string Message)> UpdateProductAsync(ProductDTO dto, int tenantId)
//		{
//			try
//			{
//				var product = await _productRepository.GetByIdAsync(dto.Id, tenantId);
//				if (product == null)
//				{
//					return (false, "Ürün bulunamadı");
//				}

//				// Yeni görsel yükleme
//				if (dto.ImageFile != null && dto.ImageFile.ContentLength > 0)
//				{
//					// Eski görseli sil
//					if (!string.IsNullOrEmpty(product.ImageUrl))
//					{
//						FileUploadHelper.DeleteFile(product.ImageUrl, _uploadsFolder);
//					}

//					var uploadResult = FileUploadHelper.UploadImage(dto.ImageFile, _uploadsFolder, tenantId);
//					if (!uploadResult.Success)
//					{
//						return (false, uploadResult.Message);
//					}
//					product.ImageUrl = uploadResult.FilePath;
//				}

//				// İsim değiştiyse slug'ı güncelle
//				if (product.Name != dto.Name)
//				{
//					string newSlug = SlugHelper.GenerateSlug(dto.Name);
//					var existingProduct = await _productRepository.GetBySlugAsync(newSlug, tenantId);

//					if (existingProduct != null && existingProduct.Id != dto.Id)
//					{
//						newSlug = SlugHelper.EnsureUnique(newSlug, s =>
//						{
//							var prod = _productRepository.GetBySlugAsync(s, tenantId).Result;
//							return prod != null && prod.Id != dto.Id;
//						});
//					}

//					product.Slug = newSlug;
//				}

//				product.CategoryId = dto.CategoryId;
//				product.Name = dto.Name;
//				product.Description = dto.Description;
//				product.Price = dto.Price;
//				product.SortOrder = dto.SortOrder;
//				product.IsActive = dto.IsActive;

//				bool updated = await _productRepository.UpdateAsync(product);

//				return updated
//					? (true, "Ürün başarıyla güncellendi")
//					: (false, "Ürün güncellenemedi");
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Ürün güncellenirken hata: {ex.Message}");
//			}
//		}

//		public async Task<(bool Success, string Message)> DeleteProductAsync(int id, int tenantId)
//		{
//			try
//			{
//				var product = await _productRepository.GetByIdAsync(id, tenantId);
//				if (product == null)
//				{
//					return (false, "Ürün bulunamadı");
//				}

//				// Görseli sil
//				if (!string.IsNullOrEmpty(product.ImageUrl))
//				{
//					FileUploadHelper.DeleteFile(product.ImageUrl, _uploadsFolder);
//				}

//				bool deleted = await _productRepository.SoftDeleteAsync(id, tenantId);

//				return deleted
//					? (true, "Ürün başarıyla silindi")
//					: (false, "Ürün silinemedi");
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Ürün silinirken hata: {ex.Message}");
//			}
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Services/SubscriptionService.cs
//// ============================================
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Data;
//using QRMenuSaaS.Data.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Services
//{
//	public class SubscriptionService
//	{
//		private readonly SubscriptionRepository _subscriptionRepository;

//		public SubscriptionService(DapperContext context)
//		{
//			_subscriptionRepository = new SubscriptionRepository(context);
//		}

//		/// <summary>
//		/// Tenant'ın aktif aboneliğini getirir
//		/// </summary>
//		public async Task<Subscription> GetActiveSubscriptionAsync(int tenantId)
//		{
//			return await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId);
//		}

//		/// <summary>
//		/// Tenant'ın abonelik durumunu kontrol eder
//		/// </summary>
//		public async Task<(bool IsActive, string Message, int DaysRemaining)> CheckSubscriptionStatusAsync(int tenantId)
//		{
//			var subscription = await GetActiveSubscriptionAsync(tenantId);

//			if (subscription == null)
//			{
//				return (false, "Aktif abonelik bulunamadı", 0);
//			}

//			var daysRemaining = (subscription.EndDate - DateTime.UtcNow).Days;

//			if (daysRemaining < 0)
//			{
//				return (false, "Aboneliğinizin süresi dolmuş", 0);
//			}

//			if (daysRemaining <= 7)
//			{
//				return (true, $"Aboneliğiniz {daysRemaining} gün içinde sona erecek", daysRemaining);
//			}

//			return (true, "Abonelik aktif", daysRemaining);
//		}

//		/// <summary>
//		/// Süresi dolmak üzere olan abonelikleri getirir (background job için)
//		/// </summary>
//		public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysThreshold = 7)
//		{
//			return await _subscriptionRepository.GetExpiringSubscriptionsAsync(daysThreshold);
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Services/ThemeService.cs
//// ============================================
//using QRMenuSaaS.Core.DTOs;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Data;
//using QRMenuSaaS.Data.Repositories;
//using System;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Services
//{
//	public class ThemeService
//	{
//		private readonly ThemeRepository _themeRepository;

//		public ThemeService(DapperContext context)
//		{
//			_themeRepository = new ThemeRepository(context);
//		}

//		public async Task<Theme> GetThemeAsync(int tenantId)
//		{
//			return await _themeRepository.GetByTenantIdAsync(tenantId);
//		}

//		public async Task<(bool Success, string Message)> SaveThemeAsync(ThemeDTO dto, int tenantId)
//		{
//			try
//			{
//				var theme = new Theme
//				{
//					TenantId = tenantId,
//					PrimaryColor = dto.PrimaryColor,
//					SecondaryColor = dto.SecondaryColor,
//					FontFamily = dto.FontFamily,
//					LogoUrl = dto.LogoUrl,
//					CoverImageUrl = dto.CoverImageUrl
//				};

//				await _themeRepository.UpsertAsync(theme);

//				return (true, "Tema ayarları başarıyla kaydedildi");
//			}
//			catch (Exception ex)
//			{
//				return (false, $"Tema kaydedilirken hata: {ex.Message}");
//			}
//		}
//	}
//}