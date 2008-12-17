using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace VSIntegrationSample
{
	#region Product
	/// <summary>
	/// This object represents the properties and methods of a Product.
	/// </summary>
	public class Product
	{
		private string _id;
		private string _categoryId = String.Empty;
		private string _name = String.Empty;
		private string _descn = String.Empty;
		private string _image = String.Empty;
		
		public Product()
		{
		}
		
		public Product(string id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@ProductId", SqlDbType.VarChar, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Product WHERE ProductId = @ProductId");
			
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
		
		public void Delete()
		{
			Product.Delete(_id);
		}
		
		public void Update()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@ProductId", SqlDbType.VarChar, Id);
			queryParameters.Append("ProductId = @ProductId");

			sql.AddParameter("@CategoryId", SqlDbType.VarChar, CategoryId);
			queryParameters.Append(", CategoryId = @CategoryId");
			sql.AddParameter("@Name", SqlDbType.VarChar, Name);
			queryParameters.Append(", Name = @Name");
			sql.AddParameter("@Descn", SqlDbType.VarChar, Descn);
			queryParameters.Append(", Descn = @Descn");
			sql.AddParameter("@Image", SqlDbType.VarChar, Image);
			queryParameters.Append(", Image = @Image");

			string query = String.Format("Update Product Set {0} Where ProductId = @ProductId", queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public void Create()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@ProductId", SqlDbType.VarChar, Id);
			queryParameters.Append("@ProductId");

			sql.AddParameter("@CategoryId", SqlDbType.VarChar, CategoryId);
			queryParameters.Append(", @CategoryId");
			sql.AddParameter("@Name", SqlDbType.VarChar, Name);
			queryParameters.Append(", @Name");
			sql.AddParameter("@Descn", SqlDbType.VarChar, Descn);
			queryParameters.Append(", @Descn");
			sql.AddParameter("@Image", SqlDbType.VarChar, Image);
			queryParameters.Append(", @Image");

			string query = String.Format("Insert Into Product ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public static Product NewProduct(string id)
		{
			Product newEntity = new Product();
			newEntity._id = id;

			return newEntity;
		}
		
		#region Public Properties
		public string Id
		{
			get {return _id;}
			set {_id = value;}
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
		
		public static void Delete(string id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@ProductId", SqlDbType.VarChar, id);
	
			SqlDataReader reader = sql.ExecuteSqlReader("Delete Product Where ProductId = @ProductId");
		}
	}
	#endregion
}

