// ============================================
// QRMenuSaaS.Data/DapperContext.cs
// ============================================
using NLog.Internal;
using Npgsql;
using System;
using System.Configuration;
using System.Data;

namespace QRMenuSaaS.Data
{
	public class DapperContext : IDisposable
	{
		private readonly string _connectionString;
		private IDbConnection _connection;

		public DapperContext()
		{
			_connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
		}

		public DapperContext(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IDbConnection Connection
		{
			get
			{
				if (_connection == null || _connection.State != ConnectionState.Open)
				{
					_connection = new NpgsqlConnection(_connectionString);
					_connection.Open();
				}
				return _connection;
			}
		}

		public void Dispose()
		{
			if (_connection != null && _connection.State == ConnectionState.Open)
			{
				_connection.Close();
				_connection.Dispose();
			}
		}
	}
}