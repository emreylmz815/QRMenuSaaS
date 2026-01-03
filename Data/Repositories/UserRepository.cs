// ============================================
// QRMenuSaaS.Data/Repositories/UserRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	public class UserRepository : BaseRepository<User>, IUserRepository
	{
		protected override string TableName => "users";
		protected override bool RequiresTenantId => false; // Users can be superadmin (no tenant)

		public UserRepository(DapperContext context) : base(context) { }

		public async Task<User> GetByEmailAsync(string email, int? tenantId = null)
		{
			// DİKKAT: @Email etrafındaki tek tırnakları ( ' ) kaldırdık.
			string sql = @"
        SELECT * FROM users 
        WHERE email = @Email 
          AND is_deleted = 0 
          AND is_active = 1";

			if (tenantId.HasValue)
			{
				sql += " AND tenant_id = @TenantId";
			}
			else
			{
				sql += " AND tenant_id IS NULL";
			}

			// Dapper burada @Email yerine otomatik olarak 'email' değişkenini güvenli şekilde yerleştirir.
			return await _context.Connection.QueryFirstOrDefaultAsync<User>(
				sql,
				new { Email = email, TenantId = tenantId });
		}

		public async Task<bool> UpdateLastLoginAsync(int userId)
		{
			string sql = @"
                UPDATE users 
                SET last_login_at = @LastLoginAt,
                    updated_at = @UpdatedAt
                WHERE id = @UserId";

			var affected = await _context.Connection.ExecuteAsync(
				sql,
				new
				{
					UserId = userId,
					LastLoginAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				});

			return affected > 0;
		}
	}
}