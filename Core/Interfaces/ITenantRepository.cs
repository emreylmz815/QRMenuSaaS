// ============================================
// QRMenuSaaS.Core/Interfaces/ITenantRepository.cs
// ============================================
using QRMenuSaaS.Core.Entities;
using System.Threading.Tasks;

namespace QRMenuSaaS.Core.Interfaces
{
	public interface ITenantRepository : IRepository<Tenant>
	{
		Task<Tenant> GetBySubdomainAsync(string subdomain);
		Task<Tenant> GetBySlugAsync(string slug);
		Task<TenantDomain> GetDomainBySubdomainAsync(string subdomain);
	}
}