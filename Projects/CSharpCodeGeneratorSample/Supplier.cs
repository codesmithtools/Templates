using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CSharpCodeGeneratorSample
{
	#region Supplier
	/// <summary>
	/// This object represents the properties and methods of a Supplier.
	/// </summary>
	public class Supplier
	{
		private int _id;
		private string _name = String.Empty;
		private string _status = String.Empty;
		private string _addr1 = String.Empty;
		private string _addr2 = String.Empty;
		private string _city = String.Empty;
		private string _state = String.Empty;
		private string _zip = String.Empty;
		private string _phone = String.Empty;
		
		public Supplier()
		{
		}
		
		public Supplier(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@SuppId", SqlDbType.Int, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Supplier WHERE SuppId = @SuppId");
			
			if (reader.Read()) 
			{
				this.LoadFromReader(reader);
				reader.Close();
			}
			else
			{
				if (!reader.IsClosed) reader.Close();
				throw new ApplicationException("Supplier does not exist.");
			}
		}
		
		public Supplier(SqlDataReader reader)
		{
			this.LoadFromReader(reader);
		}
		
		protected void LoadFromReader(SqlDataReader reader)
		{
			if (reader != null && !reader.IsClosed)
			{
				_id = reader.GetInt32(0);
				if (!reader.IsDBNull(1)) _name = reader.GetString(1);
				if (!reader.IsDBNull(2)) _status = reader.GetString(2);
				if (!reader.IsDBNull(3)) _addr1 = reader.GetString(3);
				if (!reader.IsDBNull(4)) _addr2 = reader.GetString(4);
				if (!reader.IsDBNull(5)) _city = reader.GetString(5);
				if (!reader.IsDBNull(6)) _state = reader.GetString(6);
				if (!reader.IsDBNull(7)) _zip = reader.GetString(7);
				if (!reader.IsDBNull(8)) _phone = reader.GetString(8);
			}
		}
		
		public void Delete()
		{
			Supplier.Delete(_id);
		}
		
		public void Update()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@SuppId", SqlDbType.Int, Id);
			queryParameters.Append("SuppId = @SuppId");

			sql.AddParameter("@Name", SqlDbType.VarChar, Name);
			queryParameters.Append(", Name = @Name");
			sql.AddParameter("@Status", SqlDbType.VarChar, Status);
			queryParameters.Append(", Status = @Status");
			sql.AddParameter("@Addr1", SqlDbType.VarChar, Addr1);
			queryParameters.Append(", Addr1 = @Addr1");
			sql.AddParameter("@Addr2", SqlDbType.VarChar, Addr2);
			queryParameters.Append(", Addr2 = @Addr2");
			sql.AddParameter("@City", SqlDbType.VarChar, City);
			queryParameters.Append(", City = @City");
			sql.AddParameter("@State", SqlDbType.VarChar, State);
			queryParameters.Append(", State = @State");
			sql.AddParameter("@Zip", SqlDbType.VarChar, Zip);
			queryParameters.Append(", Zip = @Zip");
			sql.AddParameter("@Phone", SqlDbType.VarChar, Phone);
			queryParameters.Append(", Phone = @Phone");

			string query = String.Format("Update Supplier Set {0} Where SuppId = @SuppId", queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public void Create()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@SuppId", SqlDbType.Int, Id);
			queryParameters.Append("@SuppId");

			sql.AddParameter("@Name", SqlDbType.VarChar, Name);
			queryParameters.Append(", @Name");
			sql.AddParameter("@Status", SqlDbType.VarChar, Status);
			queryParameters.Append(", @Status");
			sql.AddParameter("@Addr1", SqlDbType.VarChar, Addr1);
			queryParameters.Append(", @Addr1");
			sql.AddParameter("@Addr2", SqlDbType.VarChar, Addr2);
			queryParameters.Append(", @Addr2");
			sql.AddParameter("@City", SqlDbType.VarChar, City);
			queryParameters.Append(", @City");
			sql.AddParameter("@State", SqlDbType.VarChar, State);
			queryParameters.Append(", @State");
			sql.AddParameter("@Zip", SqlDbType.VarChar, Zip);
			queryParameters.Append(", @Zip");
			sql.AddParameter("@Phone", SqlDbType.VarChar, Phone);
			queryParameters.Append(", @Phone");

			string query = String.Format("Insert Into Supplier ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public static Supplier NewSupplier(int id)
		{
			Supplier newEntity = new Supplier();
			newEntity._id = id;

			return newEntity;
		}
		
		#region Public Properties
		public int Id
		{
			get {return _id;}
			set {_id = value;}
		}
		
		public string Name
		{
			get {return _name;}
			set {_name = value;}
		}

		public string Status
		{
			get {return _status;}
			set {_status = value;}
		}

		public string Addr1
		{
			get {return _addr1;}
			set {_addr1 = value;}
		}

		public string Addr2
		{
			get {return _addr2;}
			set {_addr2 = value;}
		}

		public string City
		{
			get {return _city;}
			set {_city = value;}
		}

		public string State
		{
			get {return _state;}
			set {_state = value;}
		}

		public string Zip
		{
			get {return _zip;}
			set {_zip = value;}
		}

		public string Phone
		{
			get {return _phone;}
			set {_phone = value;}
		}
		#endregion
		
		public static Supplier GetSupplier(int id)
		{
			return new Supplier(id);
		}
		
		public static void Delete(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@SuppId", SqlDbType.Int, id);
	
			SqlDataReader reader = sql.ExecuteSqlReader("Delete Supplier Where SuppId = @SuppId");
		}
	}
	#endregion
}

