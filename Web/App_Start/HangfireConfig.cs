// ============================================
// QRMenuSaaS.Web/App_Start/HangfireConfig.cs
// ============================================
using Dapper;
using Hangfire;
using Hangfire.SqlServer;
using QRMenuSaaS.Data;
using QRMenuSaaS.Services;
using System;
using System.Configuration;
using System.Linq;

namespace QRMenuSaaS.Web.App_Start
{
	/// <summary>
	/// Hangfire konfigürasyonu - Background job scheduler
	/// NuGet:
	/// - Hangfire.Core (örn: 1.7.34)
	/// - Hangfire.AspNet (örn: 1.7.34)
	/// - Hangfire.SqlServer (1.7.x uyumlu)
	/// </summary>
	public class HangfireConfig
	{
		public static void Configure()
		{
			// SQL Server storage
			var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

			GlobalConfiguration.Configuration
				.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
				{
					PrepareSchemaIfNecessary = true,
					CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
					SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
					QueuePollInterval = TimeSpan.FromSeconds(15),
					UseRecommendedIsolationLevel = true
				});

			// Background job server başlat
			var options = new BackgroundJobServerOptions
			{
				ServerName = "QRMenuSaaS",
				WorkerCount = 2
			};

			// Not: Bu server instance'ını Application kapanana kadar canlı tutmak istersin.
			// Hangfire.AspNet kullanıyorsan genelde OWIN/Startup ile UseHangfireServer daha standarttır.
			var server = new BackgroundJobServer(options);

			// Recurring jobs tanımla
			RecurringJob.AddOrUpdate(
				"check-expired-subscriptions",
				() => CheckExpiredSubscriptions(),
				Cron.Daily(1)); // 01:00

			RecurringJob.AddOrUpdate(
				"send-expiring-warnings",
				() => SendExpiringWarnings(),
				Cron.Daily(10)); // 10:00
		}

		/// <summary>
		/// Süresi dolan abonelikleri kontrol eder ve pasif yapar
		/// </summary>
		public static void CheckExpiredSubscriptions()
		{
			try
			{
				Console.WriteLine($"[{DateTime.Now}] Checking expired subscriptions...");

				using (var context = new DapperContext())
				{
					// SQL Server tarafında genelde local time ile çalışmak istersin.
					// İstersen UTC standardına da çekebiliriz; şimdilik UtcNow korunuyor.
					var now = DateTime.UtcNow;

					// Süresi dolmuş aktif abonelikleri getir
					var expiredSubs = context.Connection.Query<dynamic>(@"
SELECT s.*, t.name AS tenant_name
FROM subscriptions s
INNER JOIN tenants t ON s.tenant_id = t.id
WHERE s.status = @Active
  AND s.end_date < @Now;",
						new { Active = "active", Now = now })
						.ToList();

					foreach (var sub in expiredSubs)
					{
						// Aboneliği pasif yap
						context.Connection.Execute(@"
UPDATE subscriptions
SET status = @Expired,
    updated_at = @Now
WHERE id = @Id;",
							new { Id = (int)sub.id, Now = now, Expired = "expired" });

						// Tenant'ı suspend et
						context.Connection.Execute(@"
UPDATE tenants
SET status = @Suspended,
    updated_at = @Now
WHERE id = @Id;",
							new { Id = (int)sub.tenant_id, Now = now, Suspended = "suspended" });

						Console.WriteLine($"Subscription expired for tenant: {sub.tenant_name}");
					}

					Console.WriteLine($"[{DateTime.Now}] Expired subscriptions check completed. {expiredSubs.Count} subscriptions processed.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in CheckExpiredSubscriptions: {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Süresi dolmak üzere olan aboneliklere uyarı gönderir
		/// </summary>
		public static void SendExpiringWarnings()
		{
			try
			{
				Console.WriteLine($"[{DateTime.Now}] Checking expiring subscriptions...");

				using (var context = new DapperContext())
				{
					var subscriptionService = new SubscriptionService(context);

					// 7 gün içinde dolacak abonelikleri getir
					var expiringSubs = subscriptionService.GetExpiringSubscriptionsAsync(7).Result;

					foreach (var sub in expiringSubs)
					{
						var daysRemaining = (sub.EndDate - DateTime.UtcNow).Days;
						Console.WriteLine($"Subscription expiring in {daysRemaining} days for tenant ID: {sub.TenantId}");

						// TODO: Email/SMS uyarısı gönder
					}

					Console.WriteLine($"[{DateTime.Now}] Expiring subscriptions check completed. {expiringSubs.Count()} warnings processed.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in SendExpiringWarnings: {ex.Message}");
				throw;
			}
		}
	}
}
