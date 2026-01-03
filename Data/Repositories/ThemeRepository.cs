// ============================================
// QRMenuSaaS.Data/Repositories/ThemeRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class ThemeRepository : BaseRepository<Theme>
	{
		protected override string TableName => "themes";
		protected override bool RequiresTenantId => false;

		public ThemeRepository(DapperContext context) : base(context) { }

		public async Task<Theme> GetByTenantIdAsync(int tenantId)
		{
			string sql = @"
                SELECT * FROM themes 
                WHERE tenant_id = @TenantId";

			return await _context.Connection.QueryFirstOrDefaultAsync<Theme>(
				sql,
				new { TenantId = tenantId });
		}

		public async Task<int> UpsertAsync(Theme theme)
		{
			var existing = await GetByTenantIdAsync(theme.TenantId);

			if (existing != null)
			{
				theme.Id = existing.Id;
				await UpdateAsync(theme);
				return theme.Id;
			}
			else
			{
				return await InsertAsync(theme);
			}
		}
	}
}
