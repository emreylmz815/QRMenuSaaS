// ============================================
// QRMenuSaaS.Data/Repositories/BaseRepository.cs
// ============================================
using Dapper;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QRMenuSaaS.Data.Repositories
{
	/// <summary>
	/// Tenant izolasyonunu zorunlu kılan base repository
	/// Her sorguda tenant_id kontrolü yapar
	/// </summary>
	public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
	{
		protected readonly DapperContext _context;
		protected abstract string TableName { get; }
		protected abstract bool RequiresTenantId { get; }

		protected BaseRepository(DapperContext context)
		{
			_context = context;
		}

		public virtual async Task<T> GetByIdAsync(int id, int? tenantId = null)
		{
			ValidateTenantId(tenantId);

			string sql = $@"
                SELECT * FROM {TableName} 
                WHERE id = @Id 
                  AND is_deleted = FALSE
                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}";

			return await _context.Connection.QueryFirstOrDefaultAsync<T>(
				sql,
				new { Id = id, TenantId = tenantId });
		}

		public virtual async Task<IEnumerable<T>> GetAllAsync(int? tenantId = null)
		{
			ValidateTenantId(tenantId);

			string sql = $@"
                SELECT * FROM {TableName} 
                WHERE is_deleted = FALSE
                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}
                ORDER BY id DESC";

			return await _context.Connection.QueryAsync<T>(
				sql,
				new { TenantId = tenantId });
		}

		public virtual async Task<int> InsertAsync(T entity)
		{
			if (entity is TenantEntity tenantEntity)
			{
				if (tenantEntity.TenantId == 0)
					throw new InvalidOperationException("TenantId is required for tenant entities");
			}

			entity.CreatedAt = DateTime.UtcNow;
			entity.UpdatedAt = DateTime.UtcNow;

			// Dinamik insert sorgusu oluştur
			var properties = typeof(T).GetProperties()
				.Where(p => p.Name != "Id" && p.CanWrite)
				.ToList();

			var columns = string.Join(", ", properties.Select(p => ToSnakeCase(p.Name)));
			var values = string.Join(", ", properties.Select(p => "@" + p.Name));

			string sql = $@"
                INSERT INTO {TableName} ({columns})
                VALUES ({values})
                RETURNING id";

			return await _context.Connection.QuerySingleAsync<int>(sql, entity);
		}

		public virtual async Task<bool> UpdateAsync(T entity)
		{
			if (entity is TenantEntity tenantEntity)
			{
				if (tenantEntity.TenantId == 0)
					throw new InvalidOperationException("TenantId is required for tenant entities");
			}

			entity.UpdatedAt = DateTime.UtcNow;

			var properties = typeof(T).GetProperties()
				.Where(p => p.Name != "Id" && p.Name != "CreatedAt" && p.CanWrite)
				.ToList();

			var setClauses = string.Join(", ", properties.Select(p => $"{ToSnakeCase(p.Name)} = @{p.Name}"));

			string sql = $@"
                UPDATE {TableName} 
                SET {setClauses}
                WHERE id = @Id 
                  AND is_deleted = FALSE
                  {(RequiresTenantId && entity is TenantEntity ? "AND tenant_id = @TenantId" : "")}";

			var affected = await _context.Connection.ExecuteAsync(sql, entity);
			return affected > 0;
		}

		public virtual async Task<bool> DeleteAsync(int id, int? tenantId = null)
		{
			ValidateTenantId(tenantId);

			string sql = $@"
                DELETE FROM {TableName} 
                WHERE id = @Id
                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}";

			var affected = await _context.Connection.ExecuteAsync(
				sql,
				new { Id = id, TenantId = tenantId });

			return affected > 0;
		}

		public virtual async Task<bool> SoftDeleteAsync(int id, int? tenantId = null)
		{
			ValidateTenantId(tenantId);

			string sql = $@"
                UPDATE {TableName} 
                SET is_deleted = TRUE, 
                    deleted_at = @DeletedAt
                WHERE id = @Id 
                  AND is_deleted = FALSE
                  {(RequiresTenantId ? "AND tenant_id = @TenantId" : "")}";

			var affected = await _context.Connection.ExecuteAsync(
				sql,
				new { Id = id, TenantId = tenantId, DeletedAt = DateTime.UtcNow });

			return affected > 0;
		}

		protected void ValidateTenantId(int? tenantId)
		{
			if (RequiresTenantId && !tenantId.HasValue)
			{
				throw new InvalidOperationException("TenantId is required for this operation");
			}
		}

		protected string ToSnakeCase(string str)
		{
			return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
				.ToLower();
		}
	}
}