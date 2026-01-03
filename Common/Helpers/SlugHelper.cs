// ============================================
// QRMenuSaaS.Common/Helpers/SlugHelper.cs
// ============================================
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QRMenuSaaS.Common.Helpers
{
	/// <summary>
	/// Türkçe karakter normalize ederek URL-friendly slug üretir
	/// </summary>
	public static class SlugHelper
	{
		public static string GenerateSlug(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			// Türkçe karakterleri normalize et
			text = NormalizeTurkishCharacters(text);

			// Küçük harfe çevir
			text = text.ToLowerInvariant();

			// Özel karakterleri kaldır, sadece harf, rakam ve tire bırak
			text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

			// Birden fazla boşluğu tek boşluğa çevir
			text = Regex.Replace(text, @"\s+", " ").Trim();

			// Boşlukları tire ile değiştir
			text = Regex.Replace(text, @"\s", "-");

			// Birden fazla tireyi tek tire yap
			text = Regex.Replace(text, @"-+", "-");

			// Baş ve son tireleri kaldır
			text = text.Trim('-');

			return text;
		}

		private static string NormalizeTurkishCharacters(string text)
		{
			var normalized = new StringBuilder();

			foreach (char c in text)
			{
				switch (c)
				{
					case 'ş': normalized.Append('s'); break;
					case 'Ş': normalized.Append('S'); break;
					case 'ı': normalized.Append('i'); break;
					case 'İ': normalized.Append('I'); break;
					case 'ğ': normalized.Append('g'); break;
					case 'Ğ': normalized.Append('G'); break;
					case 'ü': normalized.Append('u'); break;
					case 'Ü': normalized.Append('U'); break;
					case 'ö': normalized.Append('o'); break;
					case 'Ö': normalized.Append('O'); break;
					case 'ç': normalized.Append('c'); break;
					case 'Ç': normalized.Append('C'); break;
					default: normalized.Append(c); break;
				}
			}

			return normalized.ToString();
		}

		/// <summary>
		/// Eğer slug zaten varsa sonuna sayı ekler (urun-adi-2)
		/// </summary>
		public static string EnsureUnique(string baseSlug, Func<string, bool> slugExists)
		{
			string slug = baseSlug;
			int counter = 2;

			while (slugExists(slug))
			{
				slug = $"{baseSlug}-{counter}";
				counter++;
			}

			return slug;
		}
	}
}
