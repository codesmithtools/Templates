using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace VSIntegrationSample
{
	#region Order
	/// <summary>
	/// This object represents the properties and methods of a Order.
	/// </summary>
	public class Order
	{
		private int _id;
		private string _userId = String.Empty;
		private DateTime _orderDate;
		private string _shipAddr1 = String.Empty;
		private string _shipAddr2 = String.Empty;
		private string _shipCity = String.Empty;
		private string _shipState = String.Empty;
		private string _shipZip = String.Empty;
		private string _shipCountry = String.Empty;
		private string _billAddr1 = String.Empty;
		private string _billAddr2 = String.Empty;
		private string _billCity = String.Empty;
		private string _billState = String.Empty;
		private string _billZip = String.Empty;
		private string _billCountry = String.Empty;
		private string _courier = String.Empty;
		private decimal _totalPrice;
		private string _billToFirstName = String.Empty;
		private string _billToLastName = String.Empty;
		private string _shipToFirstName = String.Empty;
		private string _shipToLastName = String.Empty;
		private int _authorizationNumber;
		private string _locale = String.Empty;
		
		public Order()
		{
		}
		
		public Order(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@OrderId", SqlDbType.Int, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Orders WHERE OrderId = @OrderId");
			
			if (reader.Read()) 
			{
				this.LoadFromReader(reader);
				reader.Close();
			}
			else
			{
				if (!reader.IsClosed) reader.Close();
				throw new ApplicationException("Order does not exist.");
			}
		}
		
		public Order(SqlDataReader reader)
		{
			this.LoadFromReader(reader);
		}
		
		protected void LoadFromReader(SqlDataReader reader)
		{
			if (reader != null && !reader.IsClosed)
			{
				_id = reader.GetInt32(0);
				if (!reader.IsDBNull(1)) _userId = reader.GetString(1);
				if (!reader.IsDBNull(2)) _orderDate = reader.GetDateTime(2);
				if (!reader.IsDBNull(3)) _shipAddr1 = reader.GetString(3);
				if (!reader.IsDBNull(4)) _shipAddr2 = reader.GetString(4);
				if (!reader.IsDBNull(5)) _shipCity = reader.GetString(5);
				if (!reader.IsDBNull(6)) _shipState = reader.GetString(6);
				if (!reader.IsDBNull(7)) _shipZip = reader.GetString(7);
				if (!reader.IsDBNull(8)) _shipCountry = reader.GetString(8);
				if (!reader.IsDBNull(9)) _billAddr1 = reader.GetString(9);
				if (!reader.IsDBNull(10)) _billAddr2 = reader.GetString(10);
				if (!reader.IsDBNull(11)) _billCity = reader.GetString(11);
				if (!reader.IsDBNull(12)) _billState = reader.GetString(12);
				if (!reader.IsDBNull(13)) _billZip = reader.GetString(13);
				if (!reader.IsDBNull(14)) _billCountry = reader.GetString(14);
				if (!reader.IsDBNull(15)) _courier = reader.GetString(15);
				if (!reader.IsDBNull(16)) _totalPrice = reader.GetDecimal(16);
				if (!reader.IsDBNull(17)) _billToFirstName = reader.GetString(17);
				if (!reader.IsDBNull(18)) _billToLastName = reader.GetString(18);
				if (!reader.IsDBNull(19)) _shipToFirstName = reader.GetString(19);
				if (!reader.IsDBNull(20)) _shipToLastName = reader.GetString(20);
				if (!reader.IsDBNull(21)) _authorizationNumber = reader.GetInt32(21);
				if (!reader.IsDBNull(22)) _locale = reader.GetString(22);
			}
		}
		
		public void Delete()
		{
			Order.Delete(_id);
		}
		
		public void Update()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@OrderId", SqlDbType.Int, Id);
			queryParameters.Append("OrderId = @OrderId");

			sql.AddParameter("@UserId", SqlDbType.VarChar, UserId);
			queryParameters.Append(", UserId = @UserId");
			sql.AddParameter("@OrderDate", SqlDbType.DateTime, OrderDate);
			queryParameters.Append(", OrderDate = @OrderDate");
			sql.AddParameter("@ShipAddr1", SqlDbType.VarChar, ShipAddr1);
			queryParameters.Append(", ShipAddr1 = @ShipAddr1");
			sql.AddParameter("@ShipAddr2", SqlDbType.VarChar, ShipAddr2);
			queryParameters.Append(", ShipAddr2 = @ShipAddr2");
			sql.AddParameter("@ShipCity", SqlDbType.VarChar, ShipCity);
			queryParameters.Append(", ShipCity = @ShipCity");
			sql.AddParameter("@ShipState", SqlDbType.VarChar, ShipState);
			queryParameters.Append(", ShipState = @ShipState");
			sql.AddParameter("@ShipZip", SqlDbType.VarChar, ShipZip);
			queryParameters.Append(", ShipZip = @ShipZip");
			sql.AddParameter("@ShipCountry", SqlDbType.VarChar, ShipCountry);
			queryParameters.Append(", ShipCountry = @ShipCountry");
			sql.AddParameter("@BillAddr1", SqlDbType.VarChar, BillAddr1);
			queryParameters.Append(", BillAddr1 = @BillAddr1");
			sql.AddParameter("@BillAddr2", SqlDbType.VarChar, BillAddr2);
			queryParameters.Append(", BillAddr2 = @BillAddr2");
			sql.AddParameter("@BillCity", SqlDbType.VarChar, BillCity);
			queryParameters.Append(", BillCity = @BillCity");
			sql.AddParameter("@BillState", SqlDbType.VarChar, BillState);
			queryParameters.Append(", BillState = @BillState");
			sql.AddParameter("@BillZip", SqlDbType.VarChar, BillZip);
			queryParameters.Append(", BillZip = @BillZip");
			sql.AddParameter("@BillCountry", SqlDbType.VarChar, BillCountry);
			queryParameters.Append(", BillCountry = @BillCountry");
			sql.AddParameter("@Courier", SqlDbType.VarChar, Courier);
			queryParameters.Append(", Courier = @Courier");
			sql.AddParameter("@TotalPrice", SqlDbType.Decimal, TotalPrice);
			queryParameters.Append(", TotalPrice = @TotalPrice");
			sql.AddParameter("@BillToFirstName", SqlDbType.VarChar, BillToFirstName);
			queryParameters.Append(", BillToFirstName = @BillToFirstName");
			sql.AddParameter("@BillToLastName", SqlDbType.VarChar, BillToLastName);
			queryParameters.Append(", BillToLastName = @BillToLastName");
			sql.AddParameter("@ShipToFirstName", SqlDbType.VarChar, ShipToFirstName);
			queryParameters.Append(", ShipToFirstName = @ShipToFirstName");
			sql.AddParameter("@ShipToLastName", SqlDbType.VarChar, ShipToLastName);
			queryParameters.Append(", ShipToLastName = @ShipToLastName");
			sql.AddParameter("@AuthorizationNumber", SqlDbType.Int, AuthorizationNumber);
			queryParameters.Append(", AuthorizationNumber = @AuthorizationNumber");
			sql.AddParameter("@Locale", SqlDbType.VarChar, Locale);
			queryParameters.Append(", Locale = @Locale");

			string query = String.Format("Update Orders Set {0} Where OrderId = @OrderId", queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public void Create()
		{
			SqlService sql = new SqlService();
			StringBuilder queryParameters = new StringBuilder();

			sql.AddParameter("@OrderId", SqlDbType.Int, Id);
			queryParameters.Append("@OrderId");

			sql.AddParameter("@UserId", SqlDbType.VarChar, UserId);
			queryParameters.Append(", @UserId");
			sql.AddParameter("@OrderDate", SqlDbType.DateTime, OrderDate);
			queryParameters.Append(", @OrderDate");
			sql.AddParameter("@ShipAddr1", SqlDbType.VarChar, ShipAddr1);
			queryParameters.Append(", @ShipAddr1");
			sql.AddParameter("@ShipAddr2", SqlDbType.VarChar, ShipAddr2);
			queryParameters.Append(", @ShipAddr2");
			sql.AddParameter("@ShipCity", SqlDbType.VarChar, ShipCity);
			queryParameters.Append(", @ShipCity");
			sql.AddParameter("@ShipState", SqlDbType.VarChar, ShipState);
			queryParameters.Append(", @ShipState");
			sql.AddParameter("@ShipZip", SqlDbType.VarChar, ShipZip);
			queryParameters.Append(", @ShipZip");
			sql.AddParameter("@ShipCountry", SqlDbType.VarChar, ShipCountry);
			queryParameters.Append(", @ShipCountry");
			sql.AddParameter("@BillAddr1", SqlDbType.VarChar, BillAddr1);
			queryParameters.Append(", @BillAddr1");
			sql.AddParameter("@BillAddr2", SqlDbType.VarChar, BillAddr2);
			queryParameters.Append(", @BillAddr2");
			sql.AddParameter("@BillCity", SqlDbType.VarChar, BillCity);
			queryParameters.Append(", @BillCity");
			sql.AddParameter("@BillState", SqlDbType.VarChar, BillState);
			queryParameters.Append(", @BillState");
			sql.AddParameter("@BillZip", SqlDbType.VarChar, BillZip);
			queryParameters.Append(", @BillZip");
			sql.AddParameter("@BillCountry", SqlDbType.VarChar, BillCountry);
			queryParameters.Append(", @BillCountry");
			sql.AddParameter("@Courier", SqlDbType.VarChar, Courier);
			queryParameters.Append(", @Courier");
			sql.AddParameter("@TotalPrice", SqlDbType.Decimal, TotalPrice);
			queryParameters.Append(", @TotalPrice");
			sql.AddParameter("@BillToFirstName", SqlDbType.VarChar, BillToFirstName);
			queryParameters.Append(", @BillToFirstName");
			sql.AddParameter("@BillToLastName", SqlDbType.VarChar, BillToLastName);
			queryParameters.Append(", @BillToLastName");
			sql.AddParameter("@ShipToFirstName", SqlDbType.VarChar, ShipToFirstName);
			queryParameters.Append(", @ShipToFirstName");
			sql.AddParameter("@ShipToLastName", SqlDbType.VarChar, ShipToLastName);
			queryParameters.Append(", @ShipToLastName");
			sql.AddParameter("@AuthorizationNumber", SqlDbType.Int, AuthorizationNumber);
			queryParameters.Append(", @AuthorizationNumber");
			sql.AddParameter("@Locale", SqlDbType.VarChar, Locale);
			queryParameters.Append(", @Locale");

			string query = String.Format("Insert Into Orders ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString());
			SqlDataReader reader = sql.ExecuteSqlReader(query);
		}
		
		public static Order NewOrder(int id)
		{
			Order newEntity = new Order();
			newEntity._id = id;

			return newEntity;
		}
		
		#region Public Properties
		public int Id
		{
			get {return _id;}
			set {_id = value;}
		}
		
		public string UserId
		{
			get {return _userId;}
			set {_userId = value;}
		}

		public DateTime OrderDate
		{
			get {return _orderDate;}
			set {_orderDate = value;}
		}

		public string ShipAddr1
		{
			get {return _shipAddr1;}
			set {_shipAddr1 = value;}
		}

		public string ShipAddr2
		{
			get {return _shipAddr2;}
			set {_shipAddr2 = value;}
		}

		public string ShipCity
		{
			get {return _shipCity;}
			set {_shipCity = value;}
		}

		public string ShipState
		{
			get {return _shipState;}
			set {_shipState = value;}
		}

		public string ShipZip
		{
			get {return _shipZip;}
			set {_shipZip = value;}
		}

		public string ShipCountry
		{
			get {return _shipCountry;}
			set {_shipCountry = value;}
		}

		public string BillAddr1
		{
			get {return _billAddr1;}
			set {_billAddr1 = value;}
		}

		public string BillAddr2
		{
			get {return _billAddr2;}
			set {_billAddr2 = value;}
		}

		public string BillCity
		{
			get {return _billCity;}
			set {_billCity = value;}
		}

		public string BillState
		{
			get {return _billState;}
			set {_billState = value;}
		}

		public string BillZip
		{
			get {return _billZip;}
			set {_billZip = value;}
		}

		public string BillCountry
		{
			get {return _billCountry;}
			set {_billCountry = value;}
		}

		public string Courier
		{
			get {return _courier;}
			set {_courier = value;}
		}

		public decimal TotalPrice
		{
			get {return _totalPrice;}
			set {_totalPrice = value;}
		}

		public string BillToFirstName
		{
			get {return _billToFirstName;}
			set {_billToFirstName = value;}
		}

		public string BillToLastName
		{
			get {return _billToLastName;}
			set {_billToLastName = value;}
		}

		public string ShipToFirstName
		{
			get {return _shipToFirstName;}
			set {_shipToFirstName = value;}
		}

		public string ShipToLastName
		{
			get {return _shipToLastName;}
			set {_shipToLastName = value;}
		}

		public int AuthorizationNumber
		{
			get {return _authorizationNumber;}
			set {_authorizationNumber = value;}
		}

		public string Locale
		{
			get {return _locale;}
			set {_locale = value;}
		}
		#endregion
		
		public static Order GetOrder(int id)
		{
			return new Order(id);
		}
		
		public static void Delete(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@OrderId", SqlDbType.Int, id);
	
			SqlDataReader reader = sql.ExecuteSqlReader("Delete Orders Where OrderId = @OrderId");
		}
	}
	#endregion
}

