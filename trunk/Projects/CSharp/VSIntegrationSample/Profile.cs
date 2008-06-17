using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace VSIntegrationSample
{
	#region Profile
	/// <summary>
	/// This object represents the properties and methods of a Profile.
	/// </summary>
	public class Profile
	{
		private int _id;
		private string _username = String.Empty;
		private string _applicationName = String.Empty;
		private bool _isAnonymous;
		private DateTime _lastActivityDate;
		private DateTime _lastUpdatedDate;
		
		public Profile()
		{
		}
		
		public Profile(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@UniqueID", SqlDbType.Int, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Profiles WHERE UniqueID = @UniqueID");
			
			if (reader.Read()) 
			{
				this.LoadFromReader(reader);
				reader.Close();
			}
			else
			{
				if (!reader.IsClosed) reader.Close();
				throw new ApplicationException("Profile does not exist.");
			}
		}
		
		public Profile(SqlDataReader reader)
		{
			this.LoadFromReader(reader);
		}
		
		protected void LoadFromReader(SqlDataReader reader)
		{
			if (reader != null && !reader.IsClosed)
			{
				_id = reader.GetInt32(0);
				if (!reader.IsDBNull(1)) _username = reader.GetString(1);
				if (!reader.IsDBNull(2)) _applicationName = reader.GetString(2);
				if (!reader.IsDBNull(3)) _isAnonymous = reader.GetBoolean(3);
				if (!reader.IsDBNull(4)) _lastActivityDate = reader.GetDateTime(4);
				if (!reader.IsDBNull(5)) _lastUpdatedDate = reader.GetDateTime(5);
			}
		}
		
		public void Delete()
		{
			Profile.Delete(_id);
		}
		
		public void Update()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@UniqueID", SqlDbType.Int, Id);
			queryParameters.Append("UniqueID = @UniqueID");

			sql.AddParameter("@Username", SqlDbType.VarChar, Username);
			queryParameters.Append(", Username = @Username");
			sql.AddParameter("@ApplicationName", SqlDbType.VarChar, ApplicationName);
			queryParameters.Append(", ApplicationName = @ApplicationName");
			sql.AddParameter("@IsAnonymous", SqlDbType.Bit, IsAnonymous);
			queryParameters.Append(", IsAnonymous = @IsAnonymous");
			sql.AddParameter("@LastActivityDate", SqlDbType.DateTime, LastActivityDate);
			queryParameters.Append(", LastActivityDate = @LastActivityDate");
			sql.AddParameter("@LastUpdatedDate", SqlDbType.DateTime, LastUpdatedDate);
			queryParameters.Append(", LastUpdatedDate = @LastUpdatedDate");

			string query = String.Format("Update Profiles Set {0} Where UniqueID = @UniqueID", queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public void Create()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@UniqueID", SqlDbType.Int, Id);
			queryParameters.Append("@UniqueID");

			sql.AddParameter("@Username", SqlDbType.VarChar, Username);
			queryParameters.Append(", @Username");
			sql.AddParameter("@ApplicationName", SqlDbType.VarChar, ApplicationName);
			queryParameters.Append(", @ApplicationName");
			sql.AddParameter("@IsAnonymous", SqlDbType.Bit, IsAnonymous);
			queryParameters.Append(", @IsAnonymous");
			sql.AddParameter("@LastActivityDate", SqlDbType.DateTime, LastActivityDate);
			queryParameters.Append(", @LastActivityDate");
			sql.AddParameter("@LastUpdatedDate", SqlDbType.DateTime, LastUpdatedDate);
			queryParameters.Append(", @LastUpdatedDate");

			string query = String.Format("Insert Into Profiles ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public static Profile NewProfile(int id)
		{
			Profile newEntity = new Profile();
			newEntity._id = id;

			return newEntity;
		}
		
		#region Public Properties
		public int Id
		{
			get {return _id;}
			set {_id = value;}
		}
		
		public string Username
		{
			get {return _username;}
			set {_username = value;}
		}

		public string ApplicationName
		{
			get {return _applicationName;}
			set {_applicationName = value;}
		}

		public bool IsAnonymous
		{
			get {return _isAnonymous;}
			set {_isAnonymous = value;}
		}

		public DateTime LastActivityDate
		{
			get {return _lastActivityDate;}
			set {_lastActivityDate = value;}
		}

		public DateTime LastUpdatedDate
		{
			get {return _lastUpdatedDate;}
			set {_lastUpdatedDate = value;}
		}
		#endregion
		
		public static Profile GetProfile(int id)
		{
			return new Profile(id);
		}
		
		public static void Delete(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@UniqueID", SqlDbType.Int, id);
	
			SqlDataReader reader = sql.ExecuteSqlReader("Delete Profiles Where UniqueID = @UniqueID");
		}
	}
	#endregion
}

