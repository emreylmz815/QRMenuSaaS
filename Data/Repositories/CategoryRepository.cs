// ============================================
// QRMenuSaaS.Data/Repositories/CategoryRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class CategoryRepository : BaseRepository<Category>
	{
		protected override string TableName => "categories";
		protected override bool RequiresTenantId => true;

		public CategoryRepository(DapperContext context) : base(context) { }

		public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(int tenantId)
		{
			string sql = @"
                SELECT * FROM categories 
                WHERE tenant_id = @TenantId 
                  AND is_deleted = FALSE
                  AND is_active = TRUE
                ORDER BY sort_order, name";

			return await _context.Connection.QueryAsync<Category>(
				sql,
				new { TenantId = tenantId });
		}

		public async Task<Category> GetBySlugAsync(string slug, int tenantId)
		{
			string sql = @"
                SELECT * FROM categories 
                WHERE tenant_id = @TenantId 
                  AND slug = @Slug
                  AND is_deleted = FALSE";

			return await _context.Connection.QueryFirstOrDefaultAsync<Category>(
				sql,
				new { TenantId = tenantId, Slug = slug });
		}

		public async Task<int> GetProductCountAsync(int categoryId, int tenantId)
		{
			string sql = @"
                SELECT COUNT(*) FROM products 
                WHERE category_id = @CategoryId 
                  AND tenant_id = @TenantId
                  AND is_deleted = FALSE";

			return await _context.Connection.ExecuteScalarAsync<int>(
				sql,
				new { CategoryId = categoryId, TenantId = tenantId });
		}
	}
}
