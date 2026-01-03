// ============================================
// QRMenuSaaS.Core/Entities/BaseEntity.cs
// ============================================
using System;
using System.ComponentModel.DataAnnotations;

namespace QRMenuSaaS.Core.Entities
{
	public abstract class BaseEntity
	{
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedAt { get; set; }
	}

	public abstract class TenantEntity : BaseEntity
	{
		public int TenantId { get; set; }
	}
}