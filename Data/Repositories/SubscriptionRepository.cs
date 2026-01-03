// ============================================
// QRMenuSaaS.Data/Repositories/SubscriptionRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class SubscriptionRepository : BaseRepository<Subscription>
	{
		protected override string TableName => "subscriptions";
		protected override bool RequiresTenantId => false;

		public SubscriptionRepository(DapperContext context) : base(context) { }

		public async Task<Subscription> GetActiveSubscriptionAsync(int tenantId)
		{
			string sql = @"
                SELECT s.*, p.name as PlanName
                FROM subscriptions s
                INNER JOIN plans p ON s.plan_id = p.id
                WHERE s.tenant_id = @TenantId 
                  AND s.status = 'active'
                  AND s.end_date > @Now
                ORDER BY s.created_at DESC
                LIMIT 1";

			return await _context.Connection.QueryFirstOrDefaultAsync<Subscription>(
				sql,
				new { TenantId = tenantId, Now = DateTime.UtcNow });
		}

		public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysThreshold)
		{
			string sql = @"
                SELECT s.*, p.name as PlanName, t.email as TenantEmail
                FROM subscriptions s
                INNER JOIN plans p ON s.plan_id = p.id
                INNER JOIN tenants t ON s.tenant_id = t.id
                WHERE s.status = 'active'
                  AND s.end_date BETWEEN @Now AND @Threshold";

			return await _context.Connection.QueryAsync<Subscription>(
				sql,
				new
				{
					Now = DateTime.UtcNow,
					Threshold = DateTime.UtcNow.AddDays(daysThreshold)
				});
		}
	}
}