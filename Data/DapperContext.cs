using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; // SQL Server için gerekli kütüphane

namespace QRMenuSaaS.Data
{
	public class DapperContext : IDisposable
	{
		private readonly string _connectionString;
		private IDbConnection _connection;

		public DapperContext()
		{
			// Web.config'deki DefaultConnection'ı okur
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
				// NpgsqlConnection yerine SqlConnection kullanıyoruz
				if (_connection == null || _connection.State == ConnectionState.Closed)
				{
					_connection = new SqlConnection(_connectionString);
					_connection.Open();
				}
				return _connection;
			}
		}

		public void Dispose()
		{
			if (_connection != null)
			{
				if (_connection.State != ConnectionState.Closed)
				{
					_connection.Close();
				}
				_connection.Dispose();
				_connection = null;
			}
		}
	}
}