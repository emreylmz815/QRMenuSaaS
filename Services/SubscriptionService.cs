// ============================================
// QRMenuSaaS.Services/SubscriptionService.cs
// ============================================
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRMenuSaaS.Services
{
	public class SubscriptionService
	{
		private readonly SubscriptionRepository _subscriptionRepository;

		public SubscriptionService(DapperContext context)
		{
			_subscriptionRepository = new SubscriptionRepository(context);
		}

		/// <summary>
		/// Tenant'ın aktif aboneliğini getirir
		/// </summary>
		public async Task<Subscription> GetActiveSubscriptionAsync(int tenantId)
		{
			return await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId);
		}

		/// <summary>
		/// Tenant'ın abonelik durumunu kontrol eder
		/// </summary>
		public async Task<(bool IsActive, string Message, int DaysRemaining)> CheckSubscriptionStatusAsync(int tenantId)
		{
			var subscription = await GetActiveSubscriptionAsync(tenantId);

			if (subscription == null)
			{
				return (false, "Aktif abonelik bulunamadı", 0);
			}

			var daysRemaining = (subscription.EndDate - DateTime.UtcNow).Days;

			if (daysRemaining < 0)
			{
				return (false, "Aboneliğinizin süresi dolmuş", 0);
			}

			if (daysRemaining <= 7)
			{
				return (true, $"Aboneliğiniz {daysRemaining} gün içinde sona erecek", daysRemaining);
			}

			return (true, "Abonelik aktif", daysRemaining);
		}

		/// <summary>
		/// Süresi dolmak üzere olan abonelikleri getirir (background job için)
		/// </summary>
		public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysThreshold = 7)
		{
			return await _subscriptionRepository.GetExpiringSubscriptionsAsync(daysThreshold);
		}
	}
}