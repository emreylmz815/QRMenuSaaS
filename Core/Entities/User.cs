
using QRMenuSaaS.Core.Entities;
using System;

namespace QRMenuSaaS.Core.Entities
{
	public class User : BaseEntity
	{
		public int? TenantId { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Role { get; set; } // superadmin, tenant_admin, tenant_editor
		public bool IsActive { get; set; }
		public DateTime? LastLoginAt { get; set; }

		public string FullName => $"{FirstName} {LastName}";
	}
}
