// ============================================
// QRMenuSaaS.Common/Extensions/DateTimeExtensions.cs
// ============================================
using System;

namespace QRMenuSaaS.Common.Extensions
{
	public static class DateTimeExtensions
	{
		private static readonly TimeZoneInfo TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

		/// <summary>
		/// UTC'den Türkiye saatine çevirir
		/// </summary>
		public static DateTime ToTurkeyTime(this DateTime utcDateTime)
		{
			if (utcDateTime.Kind != DateTimeKind.Utc)
				utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TurkeyTimeZone);
		}

		/// <summary>
		/// Türkiye saatinden UTC'ye çevirir
		/// </summary>
		public static DateTime ToUtcFromTurkey(this DateTime turkeyDateTime)
		{
			return TimeZoneInfo.ConvertTimeToUtc(turkeyDateTime, TurkeyTimeZone);
		}

		/// <summary>
		/// Kullanıcı dostu tarih formatı
		/// </summary>
		public static string ToFriendlyDate(this DateTime dateTime)
		{
			var localTime = dateTime.ToTurkeyTime();
			var today = DateTime.UtcNow.ToTurkeyTime().Date;
			var targetDate = localTime.Date;

			if (targetDate == today)
				return "Bugün " + localTime.ToString("HH:mm");

			if (targetDate == today.AddDays(-1))
				return "Dün " + localTime.ToString("HH:mm");

			if (targetDate > today.AddDays(-7))
				return localTime.ToString("dddd HH:mm");

			return localTime.ToString("dd.MM.yyyy HH:mm");
		}
	}
}