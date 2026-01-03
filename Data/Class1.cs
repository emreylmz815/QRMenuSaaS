//// ============================================
//// QRMenuSaaS.Data/DapperContext.cs
//// ============================================
//using Npgsql;
//using System;
//using System.Configuration;
//using System.Data;

//namespace QRMenuSaaS.Data
//{
//	public class DapperContext : IDisposable
//	{
//		private readonly string _connectionString;
//		private IDbConnection _connection;

//		public DapperContext()
//		{
//			_connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//		}

//		public DapperContext(string connectionString)
//		{
//			_connectionString = connectionString;
//		}

//		public IDbConnection Connection
//		{
//			get
//			{
//				if (_connection == null || _connection.State != ConnectionState.Open)
//				{
//					_connection = new NpgsqlConnection(_connectionString);
//					_connection.Open();
//				}
//				return _connection;
//			}
//		}

//		public void Dispose()
//		{
//			if (_connection != null && _connection.State == ConnectionState.Open)
//			{
//				_connection.Close();
//				_connection.Dispose();
//			}
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/BaseRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Core.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	/// <summary>
//	/// Tenant izolasyonunu zorunlu kılan base repository
//	/// Her sorguda tenant_id kontrolü yapar
//	/// </summary>
//	public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
//	{
//		protected readonly DapperContext _context;
//		protected abstract string TableName { get; }
//		protected abstract bool RequiresTenantId { get; }

//		protected BaseRepository(DapperContext context)
//		{
//			_context = context;
//		}

//		public virtual async Task<T> GetByIdAsync(int id, int? tenantId = null)
//		{
//			ValidateTenantId(tenantId);

//			string sql = $@"
//                SELECT * FROM {TableName} 
//                WHERE id = @Id 
//                  AND is_deleted = FALSE
//                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}";

//			return await _context.Connection.QueryFirstOrDefaultAsync<T>(
//				sql,
//				new { Id = id, TenantId = tenantId });
//		}

//		public virtual async Task<IEnumerable<T>> GetAllAsync(int? tenantId = null)
//		{
//			ValidateTenantId(tenantId);

//			string sql = $@"
//                SELECT * FROM {TableName} 
//                WHERE is_deleted = FALSE
//                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}
//                ORDER BY id DESC";

//			return await _context.Connection.QueryAsync<T>(
//				sql,
//				new { TenantId = tenantId });
//		}

//		public virtual async Task<int> InsertAsync(T entity)
//		{
//			if (entity is TenantEntity tenantEntity)
//			{
//				if (tenantEntity.TenantId == 0)
//					throw new InvalidOperationException("TenantId is required for tenant entities");
//			}

//			entity.CreatedAt = DateTime.UtcNow;
//			entity.UpdatedAt = DateTime.UtcNow;

//			// Dinamik insert sorgusu oluştur
//			var properties = typeof(T).GetProperties()
//				.Where(p => p.Name != "Id" && p.CanWrite)
//				.ToList();

//			var columns = string.Join(", ", properties.Select(p => ToSnakeCase(p.Name)));
//			var values = string.Join(", ", properties.Select(p => "@" + p.Name));

//			string sql = $@"
//                INSERT INTO {TableName} ({columns})
//                VALUES ({values})
//                RETURNING id";

//			return await _context.Connection.QuerySingleAsync<int>(sql, entity);
//		}

//		public virtual async Task<bool> UpdateAsync(T entity)
//		{
//			if (entity is TenantEntity tenantEntity)
//			{
//				if (tenantEntity.TenantId == 0)
//					throw new InvalidOperationException("TenantId is required for tenant entities");
//			}

//			entity.UpdatedAt = DateTime.UtcNow;

//			var properties = typeof(T).GetProperties()
//				.Where(p => p.Name != "Id" && p.Name != "CreatedAt" && p.CanWrite)
//				.ToList();

//			var setClauses = string.Join(", ", properties.Select(p => $"{ToSnakeCase(p.Name)} = @{p.Name}"));

//			string sql = $@"
//                UPDATE {TableName} 
//                SET {setClauses}
//                WHERE id = @Id 
//                  AND is_deleted = FALSE
//                  {(RequiresTenantId && entity is TenantEntity ? "AND tenant_id = @TenantId" : "")}";

//			var affected = await _context.Connection.ExecuteAsync(sql, entity);
//			return affected > 0;
//		}

//		public virtual async Task<bool> DeleteAsync(int id, int? tenantId = null)
//		{
//			ValidateTenantId(tenantId);

//			string sql = $@"
//                DELETE FROM {TableName} 
//                WHERE id = @Id
//                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}";

//			var affected = await _context.Connection.ExecuteAsync(
//				sql,
//				new { Id = id, TenantId = tenantId });

//			return affected > 0;
//		}

//		public virtual async Task<bool> SoftDeleteAsync(int id, int? tenantId = null)
//		{
//			ValidateTenantId(tenantId);

//			string sql = $@"
//                UPDATE {TableName} 
//                SET is_deleted = TRUE, 
//                    deleted_at = @DeletedAt
//                WHERE id = @Id 
//                  AND is_deleted = FALSE
//                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}";

//			var affected = await _context.Connection.ExecuteAsync(
//				sql,
//				new { Id = id, TenantId = tenantId, DeletedAt = DateTime.UtcNow });

//			return affected > 0;
//		}

//		protected void ValidateTenantId(int? tenantId)
//		{
//			if (RequiresTenantId && !tenantId.HasValue)
//			{
//				throw new InvalidOperationException("TenantId is required for this operation");
//			}
//		}

//		protected string ToSnakeCase(string str)
//		{
//			return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
//				.ToLower();
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/TenantRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Core.Interfaces;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
//	{
//		protected override string TableName => "tenants";
//		protected override bool RequiresTenantId => false;

//		public TenantRepository(DapperContext context) : base(context) { }

//		public async Task<Tenant> GetBySubdomainAsync(string subdomain)
//		{
//			string sql = @"
//                SELECT t.* 
//                FROM tenants t
//                INNER JOIN tenant_domains td ON t.id = td.tenant_id
//                WHERE td.subdomain = @Subdomain
//                  AND t.is_deleted = FALSE
//                  AND t.status = 'active'";

//			return await _context.Connection.QueryFirstOrDefaultAsync<Tenant>(
//				sql,
//				new { Subdomain = subdomain });
//		}

//		public async Task<Tenant> GetBySlugAsync(string slug)
//		{
//			string sql = @"
//                SELECT * FROM tenants 
//                WHERE slug = @Slug 
//                  AND is_deleted = FALSE";

//			return await _context.Connection.QueryFirstOrDefaultAsync<Tenant>(
//				sql,
//				new { Slug = slug });
//		}

//		public async Task<TenantDomain> GetDomainBySubdomainAsync(string subdomain)
//		{
//			string sql = @"
//                SELECT * FROM tenant_domains 
//                WHERE subdomain = @Subdomain";

//			return await _context.Connection.QueryFirstOrDefaultAsync<TenantDomain>(
//				sql,
//				new { Subdomain = subdomain });
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/UserRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using QRMenuSaaS.Core.Interfaces;
//using System;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class UserRepository : BaseRepository<User>, IUserRepository
//	{
//		protected override string TableName => "users";
//		protected override bool RequiresTenantId => false; // Users can be superadmin (no tenant)

//		public UserRepository(DapperContext context) : base(context) { }

//		public async Task<User> GetByEmailAsync(string email, int? tenantId = null)
//		{
//			string sql = @"
//                SELECT * FROM users 
//                WHERE email = @Email 
//                  AND is_deleted = FALSE
//                  AND is_active = TRUE";

//			if (tenantId.HasValue)
//			{
//				sql += " AND tenant_id = @TenantId";
//			}
//			else
//			{
//				sql += " AND tenant_id IS NULL"; // SuperAdmin
//			}

//			return await _context.Connection.QueryFirstOrDefaultAsync<User>(
//				sql,
//				new { Email = email, TenantId = tenantId });
//		}

//		public async Task<bool> UpdateLastLoginAsync(int userId)
//		{
//			string sql = @"
//                UPDATE users 
//                SET last_login_at = @LastLoginAt,
//                    updated_at = @UpdatedAt
//                WHERE id = @UserId";

//			var affected = await _context.Connection.ExecuteAsync(
//				sql,
//				new
//				{
//					UserId = userId,
//					LastLoginAt = DateTime.UtcNow,
//					UpdatedAt = DateTime.UtcNow
//				});

//			return affected > 0;
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/CategoryRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class CategoryRepository : BaseRepository<Category>
//	{
//		protected override string TableName => "categories";
//		protected override bool RequiresTenantId => true;

//		public CategoryRepository(DapperContext context) : base(context) { }

//		public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(int tenantId)
//		{
//			string sql = @"
//                SELECT * FROM categories 
//                WHERE tenant_id = @TenantId 
//                  AND is_deleted = FALSE
//                  AND is_active = TRUE
//                ORDER BY sort_order, name";

//			return await _context.Connection.QueryAsync<Category>(
//				sql,
//				new { TenantId = tenantId });
//		}

//		public async Task<Category> GetBySlugAsync(string slug, int tenantId)
//		{
//			string sql = @"
//                SELECT * FROM categories 
//                WHERE tenant_id = @TenantId 
//                  AND slug = @Slug
//                  AND is_deleted = FALSE";

//			return await _context.Connection.QueryFirstOrDefaultAsync<Category>(
//				sql,
//				new { TenantId = tenantId, Slug = slug });
//		}

//		public async Task<int> GetProductCountAsync(int categoryId, int tenantId)
//		{
//			string sql = @"
//                SELECT COUNT(*) FROM products 
//                WHERE category_id = @CategoryId 
//                  AND tenant_id = @TenantId
//                  AND is_deleted = FALSE";

//			return await _context.Connection.ExecuteScalarAsync<int>(
//				sql,
//				new { CategoryId = categoryId, TenantId = tenantId });
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/ProductRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class ProductRepository : BaseRepository<Product>
//	{
//		protected override string TableName => "products";
//		protected override bool RequiresTenantId => true;

//		public ProductRepository(DapperContext context) : base(context) { }

//		public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, int tenantId)
//		{
//			string sql = @"
//                SELECT p.*, c.name as CategoryName
//                FROM products p
//                INNER JOIN categories c ON p.category_id = c.id
//                WHERE p.tenant_id = @TenantId 
//                  AND p.category_id = @CategoryId
//                  AND p.is_deleted = FALSE
//                  AND p.is_active = TRUE
//                ORDER BY p.sort_order, p.name";

//			return await _context.Connection.QueryAsync<Product>(
//				sql,
//				new { TenantId = tenantId, CategoryId = categoryId });
//		}

//		public async Task<IEnumerable<Product>> GetActiveProductsAsync(int tenantId)
//		{
//			string sql = @"
//                SELECT p.*, c.name as CategoryName
//                FROM products p
//                INNER JOIN categories c ON p.category_id = c.id
//                WHERE p.tenant_id = @TenantId 
//                  AND p.is_deleted = FALSE
//                  AND p.is_active = TRUE
//                ORDER BY c.sort_order, p.sort_order, p.name";

//			return await _context.Connection.QueryAsync<Product>(
//				sql,
//				new { TenantId = tenantId });
//		}

//		public async Task<Product> GetBySlugAsync(string slug, int tenantId)
//		{
//			string sql = @"
//                SELECT p.*, c.name as CategoryName
//                FROM products p
//                INNER JOIN categories c ON p.category_id = c.id
//                WHERE p.tenant_id = @TenantId 
//                  AND p.slug = @Slug
//                  AND p.is_deleted = FALSE";

//			return await _context.Connection.QueryFirstOrDefaultAsync<Product>(
//				sql,
//				new { TenantId = tenantId, Slug = slug });
//		}

//		public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int tenantId)
//		{
//			string sql = @"
//                SELECT p.*, c.name as CategoryName
//                FROM products p
//                INNER JOIN categories c ON p.category_id = c.id
//                WHERE p.tenant_id = @TenantId 
//                  AND p.is_deleted = FALSE
//                  AND p.is_active = TRUE
//                  AND (p.name ILIKE @SearchTerm OR p.description ILIKE @SearchTerm)
//                ORDER BY p.name";

//			return await _context.Connection.QueryAsync<Product>(
//				sql,
//				new { TenantId = tenantId, SearchTerm = $"%{searchTerm}%" });
//		}

//		public async Task<int> GetProductCountByTenantAsync(int tenantId)
//		{
//			string sql = @"
//                SELECT COUNT(*) FROM products 
//                WHERE tenant_id = @TenantId 
//                  AND is_deleted = FALSE";

//			return await _context.Connection.ExecuteScalarAsync<int>(
//				sql,
//				new { TenantId = tenantId });
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/SubscriptionRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class SubscriptionRepository : BaseRepository<Subscription>
//	{
//		protected override string TableName => "subscriptions";
//		protected override bool RequiresTenantId => false;

//		public SubscriptionRepository(DapperContext context) : base(context) { }

//		public async Task<Subscription> GetActiveSubscriptionAsync(int tenantId)
//		{
//			string sql = @"
//                SELECT s.*, p.name as PlanName
//                FROM subscriptions s
//                INNER JOIN plans p ON s.plan_id = p.id
//                WHERE s.tenant_id = @TenantId 
//                  AND s.status = 'active'
//                  AND s.end_date > @Now
//                ORDER BY s.created_at DESC
//                LIMIT 1";

//			return await _context.Connection.QueryFirstOrDefaultAsync<Subscription>(
//				sql,
//				new { TenantId = tenantId, Now = DateTime.UtcNow });
//		}

//		public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysThreshold)
//		{
//			string sql = @"
//                SELECT s.*, p.name as PlanName, t.email as TenantEmail
//                FROM subscriptions s
//                INNER JOIN plans p ON s.plan_id = p.id
//                INNER JOIN tenants t ON s.tenant_id = t.id
//                WHERE s.status = 'active'
//                  AND s.end_date BETWEEN @Now AND @Threshold";

//			return await _context.Connection.QueryAsync<Subscription>(
//				sql,
//				new
//				{
//					Now = DateTime.UtcNow,
//					Threshold = DateTime.UtcNow.AddDays(daysThreshold)
//				});
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/ThemeRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class ThemeRepository : BaseRepository<Theme>
//	{
//		protected override string TableName => "themes";
//		protected override bool RequiresTenantId => false;

//		public ThemeRepository(DapperContext context) : base(context) { }

//		public async Task<Theme> GetByTenantIdAsync(int tenantId)
//		{
//			string sql = @"
//                SELECT * FROM themes 
//                WHERE tenant_id = @TenantId";

//			return await _context.Connection.QueryFirstOrDefaultAsync<Theme>(
//				sql,
//				new { TenantId = tenantId });
//		}

//		public async Task<int> UpsertAsync(Theme theme)
//		{
//			var existing = await GetByTenantIdAsync(theme.TenantId);

//			if (existing != null)
//			{
//				theme.Id = existing.Id;
//				await UpdateAsync(theme);
//				return theme.Id;
//			}
//			else
//			{
//				return await InsertAsync(theme);
//			}
//		}
//	}
//}

//// ============================================
//// QRMenuSaaS.Data/Repositories/AuditLogRepository.cs
//// ============================================
//using Dapper;
//using QRMenuSaaS.Core.Entities;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace QRMenuSaaS.Data.Repositories
//{
//	public class AuditLogRepository
//	{
//		private readonly DapperContext _context;

//		public AuditLogRepository(DapperContext context)
//		{
//			_context = context;
//		}

//		public async Task<int> LogAsync(AuditLog log)
//		{
//			log.CreatedAt = DateTime.UtcNow;

//			string sql = @"
//                INSERT INTO audit_logs 
//                (tenant_id, user_id, action, entity_type, entity_id, old_values, new_values, ip_address, user_agent, created_at)
//                VALUES 
//                (@TenantId, @UserId, @Action, @EntityType, @EntityId, @OldValues::jsonb, @NewValues::jsonb, @IpAddress, @UserAgent, @CreatedAt)
//                RETURNING id";

//			return await _context.Connection.QuerySingleAsync<int>(sql, log);
//		}

//		public async Task<IEnumerable<AuditLog>> GetByTenantAsync(int tenantId, int limit = 100)
//		{
//			string sql = @"
//                SELECT * FROM audit_logs 
//                WHERE tenant_id = @TenantId
//                ORDER BY created_at DESC
//                LIMIT @Limit";

//			return await _context.Connection.QueryAsync<AuditLog>(
//				sql,
//				new { TenantId = tenantId, Limit = limit });
//		}
//	}
//}