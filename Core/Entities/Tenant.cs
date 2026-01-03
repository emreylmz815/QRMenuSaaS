// ============================================
// QRMenuSaaS.Core/Entities/Tenant.cs
// ============================================
using QRMenuSaaS.Core.Entities;
using System;

namespace QRMenuSaaS.Core.Entities
{
	public class Tenant : BaseEntity
	{
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Status { get; set; } // active, suspended, cancelled
		public string Settings { get; set; } // JSONB as string
	}

	public class TenantDomain
	{
		public int Id { get; set; }
		public int TenantId { get; set; }
		public string Subdomain { get; set; }
		public bool IsPrimary { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}