// ============================================
// QRMenuSaaS.Core/Entities/Plan.cs
// ============================================
using QRMenuSaaS.Core.Entities;
using System;

namespace QRMenuSaaS.Core.Entities
{
	public class Plan : BaseEntity
	{
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string Currency { get; set; }
		public string BillingPeriod { get; set; } // monthly, yearly
		public int MaxProducts { get; set; }
		public int MaxCategories { get; set; }
		public int MaxImagesPerProduct { get; set; }
		public string Features { get; set; } // JSONB
		public bool IsActive { get; set; }
		public int SortOrder { get; set; }
	}

	public class Subscription : BaseEntity
	{
		public int TenantId { get; set; }
		public int PlanId { get; set; }
		public string Status { get; set; } // active, expired, cancelled, suspended
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool AutoRenew { get; set; }
		public DateTime? CancelledAt { get; set; }

		// Navigation
		public string PlanName { get; set; }
	}

	public class Payment : BaseEntity
	{
		public int TenantId { get; set; }
		public int? SubscriptionId { get; set; }
		public string Provider { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; }
		public string Status { get; set; } // pending, completed, failed, refunded
		public string ReferenceId { get; set; }
		public string ProviderResponse { get; set; }
		public DateTime? PaidAt { get; set; }
	}
}
