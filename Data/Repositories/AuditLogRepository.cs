// ============================================
// QRMenuSaaS.Data/Repositories/AuditLogRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class AuditLogRepository
	{
		private readonly DapperContext _context;

		public AuditLogRepository(DapperContext context)
		{
			_context = context;
		}

		public async Task<int> LogAsync(AuditLog log)
		{
			log.CreatedAt = DateTime.UtcNow;

			string sql = @"
                INSERT INTO audit_logs 
                (tenant_id, user_id, action, entity_type, entity_id, old_values, new_values, ip_address, user_agent, created_at)
                VALUES 
                (@TenantId, @UserId, @Action, @EntityType, @EntityId, @OldValues::jsonb, @NewValues::jsonb, @IpAddress, @UserAgent, @CreatedAt)
                RETURNING id";

			return await _context.Connection.QuerySingleAsync<int>(sql, log);
		}

		public async Task<IEnumerable<AuditLog>> GetByTenantAsync(int tenantId, int limit = 100)
		{
			string sql = @"
                SELECT * FROM audit_logs 
                WHERE tenant_id = @TenantId
                ORDER BY created_at DESC
                LIMIT @Limit";

			return await _context.Connection.QueryAsync<AuditLog>(
				sql,
				new { TenantId = tenantId, Limit = limit });
		}
	}
}