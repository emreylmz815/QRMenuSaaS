 
namespace QRMenuSaaS.Common.Constants
{
	public static class AppConstants
	{
		public const string SuperAdminCookieName = "SuperAdminAuth";
		public const string TenantCookieName = "TenantAuth";

		public const string TenantCacheKeyPrefix = "Tenant_";
		public const int TenantCacheMinutes = 60;

		public const string DefaultCurrency = "TRY";
		public const string DefaultFontFamily = "Roboto";

		public const int DefaultPageSize = 20;
		public const int MaxPageSize = 100;
	}

	// Aşağıdaki CacheKeys sınıfının bu dosyada yalnızca bir kez tanımlandığından emin olun.
	public static class CacheKeys
	{
		public static string TenantBySubdomain(string subdomain) => $"Tenant_Subdomain_{subdomain}";
		public static string TenantById(int tenantId) => $"Tenant_Id_{tenantId}";
		public static string ActiveSubscription(int tenantId) => $"Subscription_Tenant_{tenantId}";
	}
}