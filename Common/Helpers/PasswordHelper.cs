// ============================================
// QRMenuSaaS.Common/Helpers/PasswordHelper.cs
// ============================================
using System;
using System.Security.Cryptography;
using System.Text;

namespace QRMenuSaaS.Common.Helpers
{
	/// <summary>
	/// PBKDF2 kullanarak güvenli password hashing
	/// </summary>
	public static class PasswordHelper
	{
		private const int SaltSize = 16; // 128 bit
		private const int KeySize = 32; // 256 bit
		private const int Iterations = 10000;
		private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

		/// <summary>
		/// Şifre hash'ler ve salt ile birlikte döner
		/// Format: {iterations}.{salt}.{hash}
		/// </summary>
		public static string HashPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));

			using (var rng = new RNGCryptoServiceProvider())
			{
				byte[] salt = new byte[SaltSize];
				rng.GetBytes(salt);

				byte[] hash = GenerateHash(password, salt, Iterations, KeySize);

				return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
			}
		}

		/// <summary>
		/// Girilen şifreyi hash ile karşılaştırır
		/// </summary>
		public static bool VerifyPassword(string password, string hashedPassword)
		{
			if (string.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));

			if (string.IsNullOrEmpty(hashedPassword))
				throw new ArgumentNullException(nameof(hashedPassword));

			try
			{
				string[] parts = hashedPassword.Split('.');
				if (parts.Length != 3)
					return false;

				int iterations = int.Parse(parts[0]);
				byte[] salt = Convert.FromBase64String(parts[1]);
				byte[] hash = Convert.FromBase64String(parts[2]);

				byte[] testHash = GenerateHash(password, salt, iterations, hash.Length);

				return SlowEquals(hash, testHash);
			}
			catch
			{
				return false;
			}
		}

		private static byte[] GenerateHash(string password, byte[] salt, int iterations, int length)
		{
			using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, Algorithm))
			{
				return pbkdf2.GetBytes(length);
			}
		}

		/// <summary>
		/// Timing attack'a karşı güvenli karşılaştırma
		/// </summary>
		private static bool SlowEquals(byte[] a, byte[] b)
		{
			uint diff = (uint)a.Length ^ (uint)b.Length;
			for (int i = 0; i < a.Length && i < b.Length; i++)
			{
				diff |= (uint)(a[i] ^ b[i]);
			}
			return diff == 0;
		}
	}
}