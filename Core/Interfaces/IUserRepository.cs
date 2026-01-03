// ============================================
// QRMenuSaaS.Core/Interfaces/IUserRepository.cs
// ============================================
using QRMenuSaaS.Core.Entities;
using System.Threading.Tasks;

namespace QRMenuSaaS.Core.Interfaces
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User> GetByEmailAsync(string email, int? tenantId = null);
		Task<bool> UpdateLastLoginAsync(int userId);
	}
}