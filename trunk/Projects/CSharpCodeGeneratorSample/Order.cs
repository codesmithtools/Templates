using System;
using System.Data;
using System.Data.SqlClient;

namespace CSharpCodeGeneratorSample
{

	#region Order
	/// <summary>
	/// This object represents the properties and methods of a Order.
	/// </summary>
	public class Order
	{
		protected int _id;
		protected string _userId = String.Empty;
		protected DateTime _orderDate;
		protected string _shipAddr1 = String.Empty;
		protected string _shipAddr2 = String.Empty;
		protected string _shipCity = String.Empty;
		protected string _shipState = String.Empty;
		protected string _shipZip = String.Empty;
		protected string _shipCountry = String.Empty;
		protected string _billAddr1 = String.Empty;
		protected string _billAddr2 = String.Empty;
		protected string _billCity = String.Empty;
		protected string _billState = String.Empty;
		protected string _billZip = String.Empty;
		protected string _billCountry = String.Empty;
		protected string _courier = String.Empty;
		protected decimal _totalPrice;
		protected string _billToFirstName = String.Empty;
		protected string _billToLastName = String.Empty;
		protected string _shipToFirstName = String.Empty;
		protected string _shipToLastName = String.Empty;
		protected int _authorizationNumber;
		protected string _locale = String.Empty;
		
		public Order()
		{
		}
		
		public Order(int id)
		{
			SqlService sql = new SqlService();
			sql.AddParameter("@OrderId", SqlDbType.Int, id);
			SqlDataReader reader = sql.ExecuteSqlReader("SELECT * FROM Orders WHERE OrderId = '" + id.ToString() + "'");
			
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
		
		#region Public Properties
		public int Id
		{
			get {return _id;}
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
	}
	#endregion
}

