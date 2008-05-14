using System;
using System.Data;
using System.Data.SqlClient;

namespace CSharpCodeGeneratorSample
{

	#region Supplier
	/// <summary>
	/// This object represents the properties and methods of a Supplier.
	/// </summary>
	public class Supplier
	{
		protected int _id;
		protected string _name = String.Empty;
		protected string _status = String.Empty;
		protected string _addr1 = String.Empty;
		protected string _addr2 = String.Empty;
		protected string _city = String.Empty;
		protected string _state = String.Empty;
		protected string _zip = String.Empty;
		protected string _phone = String.Empty;
		
		public Supplier()
		{
		}
		
		public Supplier(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@SuppId", SqlDbType.Int, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Supplier WHERE SuppId = '" + id.ToString() + "'");
			
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
		
		#region Public Properties
		public int Id
		{
			get {return _id;}
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
	}
	#endregion
}

