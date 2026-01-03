// ============================================
// QRMenuSaaS.Core/Entities/AuditLog.cs
// ============================================
using System;

namespace QRMenuSaaS.Core.Entities
{
	public class AuditLog
	{
		public int Id { get; set; }
		public int? TenantId { get; set; }
		public int? UserId { get; set; }
		public string Action { get; set; }
		public string EntityType { get; set; }
		public int? EntityId { get; set; }
		public string OldValues { get; set; }
		public string NewValues { get; set; }
		public string IpAddress { get; set; }
		public string UserAgent { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}