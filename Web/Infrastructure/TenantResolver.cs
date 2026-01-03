// ============================================
// QRMenuSaaS.Web/Infrastructure/TenantResolver.cs
// ============================================
using QRMenuSaaS.Common.Constants;
using QRMenuSaaS.Core.Entities;
using QRMenuSaaS.Core.Interfaces;
using QRMenuSaaS.Data;
using QRMenuSaaS.Data.Repositories;
using System;
using System.Web;
using System.Web.Caching;

namespace QRMenuSaaS.Web.Infrastructure
{
	/// <summary>
	/// Subdomain'den tenant'ı tespit eder ve cache'ler
	/// </summary>
	public class TenantResolver
	{
		private readonly DapperContext _context;
		private readonly Cache _cache;

		public TenantResolver(DapperContext context)
		{
			_context = context;
			_cache = HttpRuntime.Cache;
		}

		/// <summary>
		/// Mevcut HTTP request'inden tenant'ı çözümler
		/// </summary>
		public Tenant ResolveTenant()
		{
			var request = HttpContext.Current?.Request;
			if (request == null)
				return null;

			string host = request.Url.Host.ToLowerInvariant();

			// Subdomain çıkar
			string subdomain = ExtractSubdomain(host);

			if (string.IsNullOrEmpty(subdomain))
				return null;

			// Admin veya panel subdomainleri tenant değil
			if (subdomain == "admin" || subdomain == "panel" || subdomain == "www")
				return null;

			// Cache kontrolü
			string cacheKey = CacheKeys.TenantBySubdomain(subdomain);
			var cachedTenant = _cache[cacheKey] as Tenant;

			if (cachedTenant != null)
				return cachedTenant;

			// DB'den getir
			var tenantRepository = new TenantRepository(_context);
			var tenant = tenantRepository.GetBySubdomainAsync(subdomain).Result;

			if (tenant != null)
			{
				// Cache'e ekle (60 dakika)
				_cache.Insert(
					cacheKey,
					tenant,
					null,
					DateTime.Now.AddMinutes(AppConstants.TenantCacheMinutes),
					Cache.NoSlidingExpiration);
			}

			return tenant;
		}

		/// <summary>
		/// Host'tan subdomain çıkarır
		/// örn: demo.domain.com -> demo
		/// örn: domain.com -> null
		/// </summary>
		private string ExtractSubdomain(string host)
		{
			// Localhost kontrolü
			if (host.Contains("localhost") || host.Contains("127.0.0.1"))
			{
				// localtest.me kullanımı: firma1.localtest.me
				if (host.Contains(".localtest.me"))
				{
					return host.Split('.')[0];
				}
				return null;
			}

			// Normal domain: demo.domain.com
			string[] parts = host.Split('.');

			// En az 3 parça olmalı (subdomain.domain.com)
			if (parts.Length >= 3)
			{
				return parts[0];
			}

			return null;
		}

		/// <summary>
		/// Belirli bir subdomain için tenant getirir
		/// </summary>
		public Tenant GetTenantBySubdomain(string subdomain)
		{
			string cacheKey = CacheKeys.TenantBySubdomain(subdomain);
			var cachedTenant = _cache[cacheKey] as Tenant;

			if (cachedTenant != null)
				return cachedTenant;

			var tenantRepository = new TenantRepository(_context);
			var tenant = tenantRepository.GetBySubdomainAsync(subdomain).Result;

			if (tenant != null)
			{
				_cache.Insert(
					cacheKey,
					tenant,
					null,
					DateTime.Now.AddMinutes(AppConstants.TenantCacheMinutes),
					Cache.NoSlidingExpiration);
			}

			return tenant;
		}

		/// <summary>
		/// Tenant cache'ini temizler
		/// </summary>
		public void ClearTenantCache(int tenantId)
		{
			var tenant = new TenantRepository(_context).GetByIdAsync(tenantId).Result;
			if (tenant != null)
			{
				_cache.Remove(CacheKeys.TenantBySubdomain(tenant.Slug));
				_cache.Remove(CacheKeys.TenantById(tenantId));
			}
		}
	}
}
