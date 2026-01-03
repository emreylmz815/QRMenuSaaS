//// ============================================
//// QRMenuSaaS.Web/App_Start/HangfireConfig.cs
//// ============================================
//using Hangfire;
//using Hangfire.PostgreSql;
//using QRMenuSaaS.Data;
//using QRMenuSaaS.Services;
//using System;
//using System.Configuration;

//namespace QRMenuSaaS.Web.App_Start
//{
//	/// <summary>
//	/// Hangfire konfigürasyonu - Background job scheduler
//	/// NuGet: Install-Package Hangfire.Core -Version 1.7.34
//	/// NuGet: Install-Package Hangfire.PostgreSql -Version 1.9.7
//	/// NuGet: Install-Package Hangfire.AspNet -Version 1.7.34
//	/// </summary>
//	public class HangfireConfig
//	{
//		public static void Configure()
//		{
//			// PostgreSQL storage
//			var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

//			GlobalConfiguration.Configuration
//				.UsePostgreSqlStorage(connectionString)
//				.UseColouredConsoleLogProvider();

//			// Background job server başlat
//			var options = new BackgroundJobServerOptions
//			{
//				ServerName = "QRMenuSaaS",
//				WorkerCount = 2 // Concurrent job sayısı
//			};

//			var server = new BackgroundJobServer(options);

//			// Recurring jobs tanımla
//			RecurringJob.AddOrUpdate(
//				"check-expired-subscriptions",
//				() => CheckExpiredSubscriptions(),
//				Cron.Daily(1)); // Her gün saat 01:00'de çalış

//			RecurringJob.AddOrUpdate(
//				"send-expiring-warnings",
//				() => SendExpiringWarnings(),
//				Cron.Daily(10)); // Her gün saat 10:00'da çalış
//		}

//		/// <summary>
//		/// Süresi dolan abonelikleri kontrol eder ve pasif yapar
//		/// </summary>
//		public static void CheckExpiredSubscriptions()
//		{
//			try
//			{
//				Console.WriteLine($"[{DateTime.Now}] Checking expired subscriptions...");

//				using (var context = new DapperContext())
//				{
//					var subscriptionService = new SubscriptionService(context);
//					var tenantRepository = new Data.Repositories.TenantRepository(context);

//					// Süresi dolmuş aktif abonelikleri getir
//					var expiredSubs = context.Connection.Query<dynamic>(@"
//                        SELECT s.*, t.name as tenant_name
//                        FROM subscriptions s
//                        INNER JOIN tenants t ON s.tenant_id = t.id
//                        WHERE s.status = 'active' 
//                          AND s.end_date < @Now",
//						new { Now = DateTime.UtcNow });

//					foreach (var sub in expiredSubs)
//					{
//						// Aboneliği pasif yap
//						context.Connection.Execute(@"
//                            UPDATE subscriptions 
//                            SET status = 'expired', 
//                                updated_at = @Now
//                            WHERE id = @Id",
//							new { Id = (int)sub.id, Now = DateTime.UtcNow });

//						// Tenant'ı suspend et
//						context.Connection.Execute(@"
//                            UPDATE tenants 
//                            SET status = 'suspended', 
//                                updated_at = @Now
//                            WHERE id = @Id",
//							new { Id = (int)sub.tenant_id, Now = DateTime.UtcNow });

//						Console.WriteLine($"Subscription expired for tenant: {sub.tenant_name}");

//						// TODO: Email gönder (İleride eklenebilir)
//						// SendExpirationEmail(sub.tenant_email, sub.tenant_name);
//					}

//					Console.WriteLine($"[{DateTime.Now}] Expired subscriptions check completed. {expiredSubs.Count()} subscriptions processed.");
//				}
//			}
//			catch (Exception ex)
//			{
//				Console.WriteLine($"Error in CheckExpiredSubscriptions: {ex.Message}");
//				throw;
//			}
//		}

//		/// <summary>
//		/// Süresi dolmak üzere olan aboneliklere uyarı gönderir
//		/// </summary>
//		public static void SendExpiringWarnings()
//		{
//			try
//			{
//				Console.WriteLine($"[{DateTime.Now}] Checking expiring subscriptions...");

//				using (var context = new DapperContext())
//				{
//					var subscriptionService = new SubscriptionService(context);

//					// 7 gün içinde dolacak abonelikleri getir
//					var expiringSubs = subscriptionService.GetExpiringSubscriptionsAsync(7).Result;

//					foreach (var sub in expiringSubs)
//					{
//						var daysRemaining = (sub.EndDate - DateTime.UtcNow).Days;

//						Console.WriteLine($"Subscription expiring in {daysRemaining} days for tenant ID: {sub.TenantId}");

//						// TODO: Email/SMS uyarısı gönder
//						// SendWarningEmail(tenant.Email, daysRemaining);
//					}

//					Console.WriteLine($"[{DateTime.Now}] Expiring subscriptions check completed. {expiringSubs.Count()} warnings processed.");
//				}
//			}
//			catch (Exception ex)
//			{
//				Console.WriteLine($"Error in SendExpiringWarnings: {ex.Message}");
//				throw;
//			}
//		}
//	}
//}