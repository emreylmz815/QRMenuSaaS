// ============================================
// QRMenuSaaS.Data/Repositories/ProductRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class ProductRepository : BaseRepository<Product>
	{
		protected override string TableName => "products";
		protected override bool RequiresTenantId => true;

		public ProductRepository(DapperContext context) : base(context) { }

		public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, int tenantId)
		{
			string sql = @"
                SELECT p.*, c.name as CategoryName
                FROM products p
                INNER JOIN categories c ON p.category_id = c.id
                WHERE p.tenant_id = @TenantId 
                  AND p.category_id = @CategoryId
                  AND p.is_deleted = FALSE
                  AND p.is_active = TRUE
                ORDER BY p.sort_order, p.name";

			return await _context.Connection.QueryAsync<Product>(
				sql,
				new { TenantId = tenantId, CategoryId = categoryId });
		}

		public async Task<IEnumerable<Product>> GetActiveProductsAsync(int tenantId)
		{
			string sql = @"
                SELECT p.*, c.name as CategoryName
                FROM products p
                INNER JOIN categories c ON p.category_id = c.id
                WHERE p.tenant_id = @TenantId 
                  AND p.is_deleted = FALSE
                  AND p.is_active = TRUE
                ORDER BY c.sort_order, p.sort_order, p.name";

			return await _context.Connection.QueryAsync<Product>(
				sql,
				new { TenantId = tenantId });
		}

		public async Task<Product> GetBySlugAsync(string slug, int tenantId)
		{
			string sql = @"
                SELECT p.*, c.name as CategoryName
                FROM products p
                INNER JOIN categories c ON p.category_id = c.id
                WHERE p.tenant_id = @TenantId 
                  AND p.slug = @Slug
                  AND p.is_deleted = FALSE";

			return await _context.Connection.QueryFirstOrDefaultAsync<Product>(
				sql,
				new { TenantId = tenantId, Slug = slug });
		}

		public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int tenantId)
		{
			string sql = @"
                SELECT p.*, c.name as CategoryName
                FROM products p
                INNER JOIN categories c ON p.category_id = c.id
                WHERE p.tenant_id = @TenantId 
                  AND p.is_deleted = FALSE
                  AND p.is_active = TRUE
                  AND (p.name ILIKE @SearchTerm OR p.description ILIKE @SearchTerm)
                ORDER BY p.name";

			return await _context.Connection.QueryAsync<Product>(
				sql,
				new { TenantId = tenantId, SearchTerm = $"%{searchTerm}%" });
		}

		public async Task<int> GetProductCountByTenantAsync(int tenantId)
		{
			string sql = @"
                SELECT COUNT(*) FROM products 
                WHERE tenant_id = @TenantId 
                  AND is_deleted = FALSE";

			return await _context.Connection.ExecuteScalarAsync<int>(
				sql,
				new { TenantId = tenantId });
		}
	}
}