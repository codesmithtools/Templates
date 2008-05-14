using System;
using System.Data;
using System.Data.SqlClient;

namespace CSharpCodeGeneratorSample
{

	#region Product
	/// <summary>
	/// This object represents the properties and methods of a Product.
	/// </summary>
	public class Product
	{
		protected string _id;
		protected string _categoryId = String.Empty;
		protected string _name = String.Empty;
		protected string _descn = String.Empty;
		protected string _image = String.Empty;
		
		public Product()
		{
		}
		
		public Product(string id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@ProductId", SqlDbType.VarChar, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Product WHERE ProductId = '" + id.ToString() + "'");
			
			if (reader.Read()) 
			{
				this.LoadFromReader(reader);
				reader.Close();
			}
			else
			{
				if (!reader.IsClosed) reader.Close();
				throw new ApplicationException("Product does not exist.");
			}
		}
		
		public Product(SqlDataReader reader)
		{
			this.LoadFromReader(reader);
		}
		
		protected void LoadFromReader(SqlDataReader reader)
		{
			if (reader != null && !reader.IsClosed)
			{
				_id = reader.GetString(0);
				if (!reader.IsDBNull(1)) _categoryId = reader.GetString(1);
				if (!reader.IsDBNull(2)) _name = reader.GetString(2);
				if (!reader.IsDBNull(3)) _descn = reader.GetString(3);
				if (!reader.IsDBNull(4)) _image = reader.GetString(4);
			}
		}
		
		#region Public Properties
		public string Id
		{
			get {return _id;}
		}
		
		public string CategoryId
		{
			get {return _categoryId;}
			set {_categoryId = value;}
		}

		public string Name
		{
			get {return _name;}
			set {_name = value;}
		}

		public string Descn
		{
			get {return _descn;}
			set {_descn = value;}
		}

		public string Image
		{
			get {return _image;}
			set {_image = value;}
		}
		#endregion
		
		public static Product GetProduct(string id)
		{
			return new Product(id);
		}
	}
	#endregion
}

