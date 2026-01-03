// ============================================
// QRMenuSaaS.Core/Interfaces/IRepository.cs
// ============================================
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRMenuSaaS.Core.Interfaces
{
	public interface IRepository<T> where T : class
	{
		Task<T> GetByIdAsync(int id, int? tenantId = null);
		Task<IEnumerable<T>> GetAllAsync(int? tenantId = null);
		Task<int> InsertAsync(T entity);
		Task<bool> UpdateAsync(T entity);
		Task<bool> DeleteAsync(int id, int? tenantId = null);
		Task<bool> SoftDeleteAsync(int id, int? tenantId = null);
	}
}