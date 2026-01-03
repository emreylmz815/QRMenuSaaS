// ============================================
// QRMenuSaaS.Core/Enums/UserRole.cs
// ============================================
namespace QRMenuSaaS.Core.Enums
{
	public static class UserRole
	{
		public const string SuperAdmin = "superadmin";
		public const string TenantAdmin = "tenant_admin";
		public const string TenantEditor = "tenant_editor";
	}

	public static class TenantStatus
	{
		public const string Active = "active";
		public const string Suspended = "suspended";
		public const string Cancelled = "cancelled";
	}

	public static class SubscriptionStatus
	{
		public const string Active = "active";
		public const string Expired = "expired";
		public const string Cancelled = "cancelled";
		public const string Suspended = "suspended";
	}
}