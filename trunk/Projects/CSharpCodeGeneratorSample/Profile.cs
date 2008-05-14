using System;
using System.Data;
using System.Data.SqlClient;

namespace CSharpCodeGeneratorSample
{

	#region Profile
	/// <summary>
	/// This object represents the properties and methods of a Profile.
	/// </summary>
	public class Profile
	{
		protected int _id;
		protected string _username = String.Empty;
		protected string _applicationName = String.Empty;
		protected bool _isAnonymous;
		protected DateTime _lastActivityDate;
		protected DateTime _lastUpdatedDate;
		
		public Profile()
		{
		}
		
		public Profile(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@UniqueID", SqlDbType.Int, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Profiles WHERE UniqueID = '" + id.ToString() + "'");
			
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
		
		#region Public Properties
		public int Id
		{
			get {return _id;}
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
	}
	#endregion
}

