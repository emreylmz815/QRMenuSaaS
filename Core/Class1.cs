//// ============================================
//// QRMenuSaaS.Core/Entities/BaseEntity.cs
//// ============================================
//using QRMenuSaaS.Core.Entities;
//using System;
//using System.ComponentModel.DataAnnotations;

//namespace QRMenuSaaS.Core.Entities
//{
//	public abstract class BaseEntity
//	{
//		public int Id { get; set; }
//		public DateTime CreatedAt { get; set; }
//		public DateTime UpdatedAt { get; set; }
//		public bool IsDeleted { get; set; }
//		public DateTime? DeletedAt { get; set; }
//	}

//	public abstract class TenantEntity : BaseEntity
//	{
//		public int TenantId { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/Tenant.cs
//// ============================================
//using System;

//namespace QRMenuSaaS.Core.Entities
//{
//	public class Tenant : BaseEntity
//	{
//		public string Name { get; set; }
//		public string Slug { get; set; }
//		public string Email { get; set; }
//		public string Phone { get; set; }
//		public string Status { get; set; } // active, suspended, cancelled
//		public string Settings { get; set; } // JSONB as string
//	}

//	public class TenantDomain
//	{
//		public int Id { get; set; }
//		public int TenantId { get; set; }
//		public string Subdomain { get; set; }
//		public bool IsPrimary { get; set; }
//		public DateTime CreatedAt { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/User.cs
//// ============================================
//using System;

//namespace QRMenuSaaS.Core.Entities
//{
//	public class User : BaseEntity
//	{
//		public int? TenantId { get; set; }
//		public string Email { get; set; }
//		public string PasswordHash { get; set; }
//		public string FirstName { get; set; }
//		public string LastName { get; set; }
//		public string Role { get; set; } // superadmin, tenant_admin, tenant_editor
//		public bool IsActive { get; set; }
//		public DateTime? LastLoginAt { get; set; }

//		public string FullName => $"{FirstName} {LastName}";
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/Category.cs
//// ============================================
//namespace QRMenuSaaS.Core.Entities
//{
//	public class Category : TenantEntity
//	{
//		public string Name { get; set; }
//		public string Slug { get; set; }
//		public string Description { get; set; }
//		public int SortOrder { get; set; }
//		public bool IsActive { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/Product.cs
//// ============================================
//namespace QRMenuSaaS.Core.Entities
//{
//	public class Product : TenantEntity
//	{
//		public int CategoryId { get; set; }
//		public string Name { get; set; }
//		public string Slug { get; set; }
//		public string Description { get; set; }
//		public decimal Price { get; set; }
//		public string Currency { get; set; }
//		public string ImageUrl { get; set; }
//		public int SortOrder { get; set; }
//		public bool IsActive { get; set; }

//		// Navigation (populated separately)
//		public string CategoryName { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/Plan.cs
//// ============================================
//namespace QRMenuSaaS.Core.Entities
//{
//	public class Plan : BaseEntity
//	{
//		public string Name { get; set; }
//		public string Slug { get; set; }
//		public string Description { get; set; }
//		public decimal Price { get; set; }
//		public string Currency { get; set; }
//		public string BillingPeriod { get; set; } // monthly, yearly
//		public int MaxProducts { get; set; }
//		public int MaxCategories { get; set; }
//		public int MaxImagesPerProduct { get; set; }
//		public string Features { get; set; } // JSONB
//		public bool IsActive { get; set; }
//		public int SortOrder { get; set; }
//	}

//	public class Subscription : BaseEntity
//	{
//		public int TenantId { get; set; }
//		public int PlanId { get; set; }
//		public string Status { get; set; } // active, expired, cancelled, suspended
//		public DateTime StartDate { get; set; }
//		public DateTime EndDate { get; set; }
//		public bool AutoRenew { get; set; }
//		public DateTime? CancelledAt { get; set; }

//		// Navigation
//		public string PlanName { get; set; }
//	}

//	public class Payment : BaseEntity
//	{
//		public int TenantId { get; set; }
//		public int? SubscriptionId { get; set; }
//		public string Provider { get; set; }
//		public decimal Amount { get; set; }
//		public string Currency { get; set; }
//		public string Status { get; set; } // pending, completed, failed, refunded
//		public string ReferenceId { get; set; }
//		public string ProviderResponse { get; set; }
//		public DateTime? PaidAt { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/Theme.cs
//// ============================================
//namespace QRMenuSaaS.Core.Entities
//{
//	public class Theme : BaseEntity
//	{
//		public int TenantId { get; set; }
//		public string PrimaryColor { get; set; }
//		public string SecondaryColor { get; set; }
//		public string FontFamily { get; set; }
//		public string LogoUrl { get; set; }
//		public string CoverImageUrl { get; set; }
//		public string CustomCss { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Entities/AuditLog.cs
//// ============================================
//using System;

//namespace QRMenuSaaS.Core.Entities
//{
//	public class AuditLog
//	{
//		public int Id { get; set; }
//		public int? TenantId { get; set; }
//		public int? UserId { get; set; }
//		public string Action { get; set; }
//		public string EntityType { get; set; }
//		public int? EntityId { get; set; }
//		public string OldValues { get; set; }
//		public string NewValues { get; set; }
//		public string IpAddress { get; set; }
//		public string UserAgent { get; set; }
//		public DateTime CreatedAt { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/DTOs/LoginDTO.cs
//// ============================================
//namespace QRMenuSaaS.Core.DTOs
//{
//	public class LoginDTO
//	{
//		public string Email { get; set; }
//		public string Password { get; set; }
//		public bool RememberMe { get; set; }
//	}

//	public class LoginResultDTO
//	{
//		public bool Success { get; set; }
//		public string Message { get; set; }
//		public User User { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/DTOs/CategoryDTO.cs
//// ============================================
//using System.ComponentModel.DataAnnotations;

//namespace QRMenuSaaS.Core.DTOs
//{
//	public class CategoryDTO
//	{
//		public int Id { get; set; }

//		[Required(ErrorMessage = "Kategori adı gereklidir")]
//		[MaxLength(200)]
//		public string Name { get; set; }

//		public string Description { get; set; }
//		public int SortOrder { get; set; }
//		public bool IsActive { get; set; }
//		public int ProductCount { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/DTOs/ProductDTO.cs
//// ============================================
//using System.ComponentModel.DataAnnotations;
//using System.Web;

//namespace QRMenuSaaS.Core.DTOs
//{
//	public class ProductDTO
//	{
//		public int Id { get; set; }

//		[Required(ErrorMessage = "Kategori seçiniz")]
//		public int CategoryId { get; set; }

//		[Required(ErrorMessage = "Ürün adı gereklidir")]
//		[MaxLength(300)]
//		public string Name { get; set; }

//		public string Description { get; set; }

//		[Required(ErrorMessage = "Fiyat giriniz")]
//		[Range(0, 999999)]
//		public decimal Price { get; set; }

//		public string ImageUrl { get; set; }
//		public HttpPostedFileBase ImageFile { get; set; }
//		public int SortOrder { get; set; }
//		public bool IsActive { get; set; }

//		public string CategoryName { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/DTOs/ThemeDTO.cs
//// ============================================
//using System.ComponentModel.DataAnnotations;

//namespace QRMenuSaaS.Core.DTOs
//{
//	public class ThemeDTO
//	{
//		[Required]
//		[RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Geçerli renk kodu giriniz (#RRGGBB)")]
//		public string PrimaryColor { get; set; }

//		[Required]
//		[RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Geçerli renk kodu giriniz (#RRGGBB)")]
//		public string SecondaryColor { get; set; }

//		[Required]
//		public string FontFamily { get; set; }

//		public string LogoUrl { get; set; }
//		public string CoverImageUrl { get; set; }
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Interfaces/IRepository.cs
//// ============================================
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Core.Interfaces
//{
//	public interface IRepository<T> where T : class
//	{
//		Task<T> GetByIdAsync(int id, int? tenantId = null);
//		Task<IEnumerable<T>> GetAllAsync(int? tenantId = null);
//		Task<int> InsertAsync(T entity);
//		Task<bool> UpdateAsync(T entity);
//		Task<bool> DeleteAsync(int id, int? tenantId = null);
//		Task<bool> SoftDeleteAsync(int id, int? tenantId = null);
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Interfaces/ITenantRepository.cs
//// ============================================
//using QRMenuSaaS.Core.Entities;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Core.Interfaces
//{
//	public interface ITenantRepository : IRepository<Tenant>
//	{
//		Task<Tenant> GetBySubdomainAsync(string subdomain);
//		Task<Tenant> GetBySlugAsync(string slug);
//		Task<TenantDomain> GetDomainBySubdomainAsync(string subdomain);
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Interfaces/IUserRepository.cs
//// ============================================
//using QRMenuSaaS.Core.Entities;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Core.Interfaces
//{
//	public interface IUserRepository : IRepository<User>
//	{
//		Task<User> GetByEmailAsync(string email, int? tenantId = null);
//		Task<bool> UpdateLastLoginAsync(int userId);
//	}
//}

//// ============================================
//// QRMenuSaaS.Core/Enums/UserRole.cs
//// ============================================
//namespace QRMenuSaaS.Core.Enums
//{
//	public static class UserRole
//	{
//		public const string SuperAdmin = "superadmin";
//		public const string TenantAdmin = "tenant_admin";
//		public const string TenantEditor = "tenant_editor";
//	}

//	public static class TenantStatus
//	{
//		public const string Active = "active";
//		public const string Suspended = "suspended";
//		public const string Cancelled = "cancelled";
//	}

//	public static class SubscriptionStatus
//	{
//		public const string Active = "active";
//		public const string Expired = "expired";
//		public const string Cancelled = "cancelled";
//		public const string Suspended = "suspended";
//	}
//}