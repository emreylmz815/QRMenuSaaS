// ============================================
// QRMenuSaaS.Data/Repositories/TenantRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Core.Interfaces;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
	{
		protected override string TableName => "tenants";
		protected override bool RequiresTenantId => false;

		public TenantRepository(DapperContext context) : base(context) { }

		public async Task<Tenant> GetBySubdomainAsync(string subdomain)
		{
			string sql = @"
                SELECT t.* 
                FROM tenants t
                INNER JOIN tenant_domains td ON t.id = td.tenant_id
                WHERE td.subdomain = @Subdomain
                  AND t.is_deleted = FALSE
                  AND t.status = 'active'";

			return await _context.Connection.QueryFirstOrDefaultAsync<Tenant>(
				sql,
				new { Subdomain = subdomain });
		}

		public async Task<Tenant> GetBySlugAsync(string slug)
		{
			string sql = @"
                SELECT * FROM tenants 
                WHERE slug = @Slug 
                  AND is_deleted = FALSE";

			return await _context.Connection.QueryFirstOrDefaultAsync<Tenant>(
				sql,
				new { Slug = slug });
		}

		public async Task<TenantDomain> GetDomainBySubdomainAsync(string subdomain)
		{
			string sql = @"
                SELECT * FROM tenant_domains 
                WHERE subdomain = @Subdomain";

			return await _context.Connection.QueryFirstOrDefaultAsync<TenantDomain>(
				sql,
				new { Subdomain = subdomain });
		}
	}
}