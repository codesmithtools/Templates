using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace TypedDataSetSample
{
	#region OrdersDataSet
	[Serializable()]
	[DesignerCategoryAttribute("code")]
	[System.Diagnostics.DebuggerStepThrough()]
	[ToolboxItem(true)]
	public class OrdersDataSet: DataSet
	{
		private OrdersDataTable _tableOrders;
		
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		public OrdersDataTable Orders
		{
			get
			{
				return this._tableOrders;
			}
		}
		
		public OrdersDataSet()
		{
			this.InitClass();
		}
		
		protected override XmlSchema GetSchemaSerializable() 
		{
			MemoryStream stream = new MemoryStream();
			this.WriteXmlSchema(new	XmlTextWriter(stream, null));
			stream.Position = 0;
			return XmlSchema.Read(new XmlTextReader(stream), null);
		}
		
		protected override void	ReadXmlSerializable(XmlReader reader) 
		{
			this.Reset();
			DataSet ds = new DataSet();
			ds.ReadXml(reader);
			if ((ds.Tables["Orders"] != null))
			{
				this.Tables.Add(new OrdersDataTable(ds.Tables["Orders"]));
			}
			this.DataSetName = ds.DataSetName;
			this.Prefix = ds.Prefix;
			this.Namespace = ds.Namespace;
			this.Locale = ds.Locale;
			this.CaseSensitive = ds.CaseSensitive;
			this.EnforceConstraints = ds.EnforceConstraints;
			this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
			this.InitVars();
		}
		
		protected OrdersDataSet(SerializationInfo info, StreamingContext context)
		{
			string strSchema = ((string)(info.GetValue("XmlSchema", typeof(string))));
			if ((strSchema != null))
			{
				DataSet ds = new DataSet();
				ds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(strSchema)));
				if ((ds.Tables["Orders"] != null))
				{
					this.Tables.Add(new OrdersDataTable(ds.Tables["Orders"]));
				}
				this.DataSetName = ds.DataSetName;
				this.Prefix = ds.Prefix;
				this.Namespace = ds.Namespace;
				this.Locale = ds.Locale;
				this.CaseSensitive = ds.CaseSensitive;
				this.EnforceConstraints = ds.EnforceConstraints;
				this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
				this.InitVars();
			}
			else
			{
				this.InitClass();
			}
			this.GetSerializationData(info, context);
		}
		
		private void InitClass()
		{
			this.DataSetName = "OrdersDataSet";
			_tableOrders = new OrdersDataTable();
			this.Tables.Add(_tableOrders);
			this.ExtendedProperties.Add("DataAdapterName", "OrdersDataAdapter");
			this.ExtendedProperties.Add("ObjectName", "Orders");
			this.ExtendedProperties.Add("ObjectDescription", "Orders");
			this.ExtendedProperties.Add("NameSpace", "");
		}
		
		public override DataSet Clone()
		{
			OrdersDataSet cln = ((OrdersDataSet)(base.Clone()));
			cln.InitVars();
			return cln;
		}
		
		internal void InitVars()
		{
			_tableOrders = ((OrdersDataTable)(this.Tables["Orders"]));
			if (_tableOrders != null)
			{
				_tableOrders.InitVars();
			}
		}
		
		protected override bool ShouldSerializeTables()
		{
			return false;
		}
		
		protected override bool ShouldSerializeRelations()
		{
			return false;
		}
		
		private bool ShouldSerializeOrders()
		{
			return false;
		}
		
		public delegate void OrdersRowChangeEventHandler(object sender, OrdersRowChangeEventArgs e);
		
		[Serializable()]
		public class OrdersDataTable: DataTable, System.Collections.IEnumerable
		{
			private DataColumn _columnOrderId;
			private DataColumn _columnUserId;
			private DataColumn _columnOrderDate;
			private DataColumn _columnShipAddr1;
			private DataColumn _columnShipAddr2;
			private DataColumn _columnShipCity;
			private DataColumn _columnShipState;
			private DataColumn _columnShipZip;
			private DataColumn _columnShipCountry;
			private DataColumn _columnBillAddr1;
			private DataColumn _columnBillAddr2;
			private DataColumn _columnBillCity;
			private DataColumn _columnBillState;
			private DataColumn _columnBillZip;
			private DataColumn _columnBillCountry;
			private DataColumn _columnCourier;
			private DataColumn _columnTotalPrice;
			private DataColumn _columnBillToFirstName;
			private DataColumn _columnBillToLastName;
			private DataColumn _columnShipToFirstName;
			private DataColumn _columnShipToLastName;
			private DataColumn _columnAuthorizationNumber;
			private DataColumn _columnLocale;
			
			internal OrdersDataTable(): base("Orders")
			{
				this.InitClass();
			}
			
			protected OrdersDataTable(SerializationInfo info, StreamingContext context): base(info, context)
			{
				this.InitVars();
			}
			
			internal OrdersDataTable(DataTable table): base(table.TableName)
			{
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					this.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					this.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					this.Namespace = table.Namespace;
				}
				this.Prefix = table.Prefix;
				this.MinimumCapacity = table.MinimumCapacity;
				this.DisplayExpression = table.DisplayExpression;
			}
			
			public int Count
			{
				get
				{
					return this.Rows.Count;
				}
			}
			
			public DataColumn OrderIdColumn
			{
				get
				{
					return _columnOrderId;
				}
			}
			
			public DataColumn UserIdColumn
			{
				get
				{
					return _columnUserId;
				}
			}
			
			public DataColumn OrderDateColumn
			{
				get
				{
					return _columnOrderDate;
				}
			}
			
			public DataColumn ShipAddr1Column
			{
				get
				{
					return _columnShipAddr1;
				}
			}
			
			public DataColumn ShipAddr2Column
			{
				get
				{
					return _columnShipAddr2;
				}
			}
			
			public DataColumn ShipCityColumn
			{
				get
				{
					return _columnShipCity;
				}
			}
			
			public DataColumn ShipStateColumn
			{
				get
				{
					return _columnShipState;
				}
			}
			
			public DataColumn ShipZipColumn
			{
				get
				{
					return _columnShipZip;
				}
			}
			
			public DataColumn ShipCountryColumn
			{
				get
				{
					return _columnShipCountry;
				}
			}
			
			public DataColumn BillAddr1Column
			{
				get
				{
					return _columnBillAddr1;
				}
			}
			
			public DataColumn BillAddr2Column
			{
				get
				{
					return _columnBillAddr2;
				}
			}
			
			public DataColumn BillCityColumn
			{
				get
				{
					return _columnBillCity;
				}
			}
			
			public DataColumn BillStateColumn
			{
				get
				{
					return _columnBillState;
				}
			}
			
			public DataColumn BillZipColumn
			{
				get
				{
					return _columnBillZip;
				}
			}
			
			public DataColumn BillCountryColumn
			{
				get
				{
					return _columnBillCountry;
				}
			}
			
			public DataColumn CourierColumn
			{
				get
				{
					return _columnCourier;
				}
			}
			
			public DataColumn TotalPriceColumn
			{
				get
				{
					return _columnTotalPrice;
				}
			}
			
			public DataColumn BillToFirstNameColumn
			{
				get
				{
					return _columnBillToFirstName;
				}
			}
			
			public DataColumn BillToLastNameColumn
			{
				get
				{
					return _columnBillToLastName;
				}
			}
			
			public DataColumn ShipToFirstNameColumn
			{
				get
				{
					return _columnShipToFirstName;
				}
			}
			
			public DataColumn ShipToLastNameColumn
			{
				get
				{
					return _columnShipToLastName;
				}
			}
			
			public DataColumn AuthorizationNumberColumn
			{
				get
				{
					return _columnAuthorizationNumber;
				}
			}
			
			public DataColumn LocaleColumn
			{
				get
				{
					return _columnLocale;
				}
			}
			
			public OrdersRow this[int index]
			{
				get
				{
					return ((OrdersRow)(this.Rows[index]));
				}
			}
			
			public event OrdersRowChangeEventHandler OrdersRowChanged;
			public event OrdersRowChangeEventHandler OrdersRowChanging;
			public event OrdersRowChangeEventHandler OrdersRowDeleted;
			public event OrdersRowChangeEventHandler OrdersRowDeleting;
			
			public void AddOrdersRow(OrdersRow row)
			{
				this.Rows.Add(row);
			}
			public OrdersRow AddOrdersRow(
					int orderId,
					string userId,
					DateTime orderDate,
					string shipAddr1,
					string shipAddr2,
					string shipCity,
					string shipState,
					string shipZip,
					string shipCountry,
					string billAddr1,
					string billAddr2,
					string billCity,
					string billState,
					string billZip,
					string billCountry,
					string courier,
					decimal totalPrice,
					string billToFirstName,
					string billToLastName,
					string shipToFirstName,
					string shipToLastName,
					int authorizationNumber,
					string locale
				)
			{
				OrdersRow rowOrdersRow = ((OrdersRow)(this.NewRow()));
				rowOrdersRow["OrderId"] = orderId;
				rowOrdersRow["UserId"] = userId;
				rowOrdersRow["OrderDate"] = orderDate;
				rowOrdersRow["ShipAddr1"] = shipAddr1;
				rowOrdersRow["ShipAddr2"] = shipAddr2;
				rowOrdersRow["ShipCity"] = shipCity;
				rowOrdersRow["ShipState"] = shipState;
				rowOrdersRow["ShipZip"] = shipZip;
				rowOrdersRow["ShipCountry"] = shipCountry;
				rowOrdersRow["BillAddr1"] = billAddr1;
				rowOrdersRow["BillAddr2"] = billAddr2;
				rowOrdersRow["BillCity"] = billCity;
				rowOrdersRow["BillState"] = billState;
				rowOrdersRow["BillZip"] = billZip;
				rowOrdersRow["BillCountry"] = billCountry;
				rowOrdersRow["Courier"] = courier;
				rowOrdersRow["TotalPrice"] = totalPrice;
				rowOrdersRow["BillToFirstName"] = billToFirstName;
				rowOrdersRow["BillToLastName"] = billToLastName;
				rowOrdersRow["ShipToFirstName"] = shipToFirstName;
				rowOrdersRow["ShipToLastName"] = shipToLastName;
				rowOrdersRow["AuthorizationNumber"] = authorizationNumber;
				rowOrdersRow["Locale"] = locale;
				this.Rows.Add(rowOrdersRow);
				return rowOrdersRow;
			}
			
			public OrdersRow FindByOrderId(int orderId)
			{
				return ((OrdersRow)(this.Rows.Find(new object[] {orderId})));
			}
			
			public IEnumerator GetEnumerator()
			{
				return this.Rows.GetEnumerator();
			}
			
			public override DataTable Clone()
			{
				OrdersDataTable cln = ((OrdersDataTable)(base.Clone()));
				cln.InitVars();
				return cln;
			}
			
			internal void InitVars()
			{
				_columnOrderId = this.Columns["OrderId"];
				_columnUserId = this.Columns["UserId"];
				_columnOrderDate = this.Columns["OrderDate"];
				_columnShipAddr1 = this.Columns["ShipAddr1"];
				_columnShipAddr2 = this.Columns["ShipAddr2"];
				_columnShipCity = this.Columns["ShipCity"];
				_columnShipState = this.Columns["ShipState"];
				_columnShipZip = this.Columns["ShipZip"];
				_columnShipCountry = this.Columns["ShipCountry"];
				_columnBillAddr1 = this.Columns["BillAddr1"];
				_columnBillAddr2 = this.Columns["BillAddr2"];
				_columnBillCity = this.Columns["BillCity"];
				_columnBillState = this.Columns["BillState"];
				_columnBillZip = this.Columns["BillZip"];
				_columnBillCountry = this.Columns["BillCountry"];
				_columnCourier = this.Columns["Courier"];
				_columnTotalPrice = this.Columns["TotalPrice"];
				_columnBillToFirstName = this.Columns["BillToFirstName"];
				_columnBillToLastName = this.Columns["BillToLastName"];
				_columnShipToFirstName = this.Columns["ShipToFirstName"];
				_columnShipToLastName = this.Columns["ShipToLastName"];
				_columnAuthorizationNumber = this.Columns["AuthorizationNumber"];
				_columnLocale = this.Columns["Locale"];
			}
			
			public void InitClass()
			{
				_columnOrderId = new DataColumn("OrderId", typeof(int), "", MappingType.Element);
				_columnOrderId.AllowDBNull = false;
				_columnOrderId.Caption = "Order Id";
				_columnOrderId.Unique = true;
				_columnOrderId.DefaultValue = Int32.MinValue ;
				_columnOrderId.ExtendedProperties.Add("IsKey", "true");
				_columnOrderId.ExtendedProperties.Add("ReadOnly", "false");
				_columnOrderId.ExtendedProperties.Add("Description", "Order Id");
				_columnOrderId.ExtendedProperties.Add("Decimals", "0");
				_columnOrderId.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnOrderId);
				
				_columnUserId = new DataColumn("UserId", typeof(string), "", MappingType.Element);
				_columnUserId.AllowDBNull = false;
				_columnUserId.Caption = "User Id";
				_columnUserId.MaxLength = 20;
				_columnUserId.Unique = false;
				_columnUserId.DefaultValue = Convert.DBNull ;
				_columnUserId.ExtendedProperties.Add("IsKey", "false");
				_columnUserId.ExtendedProperties.Add("ReadOnly", "false");
				_columnUserId.ExtendedProperties.Add("Description", "User Id");
				_columnUserId.ExtendedProperties.Add("Length", "20");
				_columnUserId.ExtendedProperties.Add("Decimals", "0");
				_columnUserId.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnUserId);
				
				_columnOrderDate = new DataColumn("OrderDate", typeof(DateTime), "", MappingType.Element);
				_columnOrderDate.AllowDBNull = false;
				_columnOrderDate.Caption = "Order Date";
				_columnOrderDate.Unique = false;
				_columnOrderDate.DefaultValue = Convert.DBNull ;
				_columnOrderDate.ExtendedProperties.Add("IsKey", "false");
				_columnOrderDate.ExtendedProperties.Add("ReadOnly", "false");
				_columnOrderDate.ExtendedProperties.Add("Description", "Order Date");
				_columnOrderDate.ExtendedProperties.Add("Decimals", "0");
				_columnOrderDate.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnOrderDate);
				
				_columnShipAddr1 = new DataColumn("ShipAddr1", typeof(string), "", MappingType.Element);
				_columnShipAddr1.AllowDBNull = false;
				_columnShipAddr1.Caption = "Ship Addr1";
				_columnShipAddr1.MaxLength = 80;
				_columnShipAddr1.Unique = false;
				_columnShipAddr1.DefaultValue = Convert.DBNull ;
				_columnShipAddr1.ExtendedProperties.Add("IsKey", "false");
				_columnShipAddr1.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipAddr1.ExtendedProperties.Add("Description", "Ship Addr1");
				_columnShipAddr1.ExtendedProperties.Add("Length", "80");
				_columnShipAddr1.ExtendedProperties.Add("Decimals", "0");
				_columnShipAddr1.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipAddr1);
				
				_columnShipAddr2 = new DataColumn("ShipAddr2", typeof(string), "", MappingType.Element);
				_columnShipAddr2.AllowDBNull = true;
				_columnShipAddr2.Caption = "Ship Addr2";
				_columnShipAddr2.MaxLength = 80;
				_columnShipAddr2.Unique = false;
				_columnShipAddr2.DefaultValue = Convert.DBNull ;
				_columnShipAddr2.ExtendedProperties.Add("IsKey", "false");
				_columnShipAddr2.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipAddr2.ExtendedProperties.Add("Description", "Ship Addr2");
				_columnShipAddr2.ExtendedProperties.Add("Length", "80");
				_columnShipAddr2.ExtendedProperties.Add("Decimals", "0");
				_columnShipAddr2.ExtendedProperties.Add("AllowDBNulls", "true");
				this.Columns.Add(_columnShipAddr2);
				
				_columnShipCity = new DataColumn("ShipCity", typeof(string), "", MappingType.Element);
				_columnShipCity.AllowDBNull = false;
				_columnShipCity.Caption = "Ship City";
				_columnShipCity.MaxLength = 80;
				_columnShipCity.Unique = false;
				_columnShipCity.DefaultValue = Convert.DBNull ;
				_columnShipCity.ExtendedProperties.Add("IsKey", "false");
				_columnShipCity.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipCity.ExtendedProperties.Add("Description", "Ship City");
				_columnShipCity.ExtendedProperties.Add("Length", "80");
				_columnShipCity.ExtendedProperties.Add("Decimals", "0");
				_columnShipCity.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipCity);
				
				_columnShipState = new DataColumn("ShipState", typeof(string), "", MappingType.Element);
				_columnShipState.AllowDBNull = false;
				_columnShipState.Caption = "Ship State";
				_columnShipState.MaxLength = 80;
				_columnShipState.Unique = false;
				_columnShipState.DefaultValue = Convert.DBNull ;
				_columnShipState.ExtendedProperties.Add("IsKey", "false");
				_columnShipState.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipState.ExtendedProperties.Add("Description", "Ship State");
				_columnShipState.ExtendedProperties.Add("Length", "80");
				_columnShipState.ExtendedProperties.Add("Decimals", "0");
				_columnShipState.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipState);
				
				_columnShipZip = new DataColumn("ShipZip", typeof(string), "", MappingType.Element);
				_columnShipZip.AllowDBNull = false;
				_columnShipZip.Caption = "Ship Zip";
				_columnShipZip.MaxLength = 20;
				_columnShipZip.Unique = false;
				_columnShipZip.DefaultValue = Convert.DBNull ;
				_columnShipZip.ExtendedProperties.Add("IsKey", "false");
				_columnShipZip.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipZip.ExtendedProperties.Add("Description", "Ship Zip");
				_columnShipZip.ExtendedProperties.Add("Length", "20");
				_columnShipZip.ExtendedProperties.Add("Decimals", "0");
				_columnShipZip.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipZip);
				
				_columnShipCountry = new DataColumn("ShipCountry", typeof(string), "", MappingType.Element);
				_columnShipCountry.AllowDBNull = false;
				_columnShipCountry.Caption = "Ship Country";
				_columnShipCountry.MaxLength = 20;
				_columnShipCountry.Unique = false;
				_columnShipCountry.DefaultValue = Convert.DBNull ;
				_columnShipCountry.ExtendedProperties.Add("IsKey", "false");
				_columnShipCountry.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipCountry.ExtendedProperties.Add("Description", "Ship Country");
				_columnShipCountry.ExtendedProperties.Add("Length", "20");
				_columnShipCountry.ExtendedProperties.Add("Decimals", "0");
				_columnShipCountry.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipCountry);
				
				_columnBillAddr1 = new DataColumn("BillAddr1", typeof(string), "", MappingType.Element);
				_columnBillAddr1.AllowDBNull = false;
				_columnBillAddr1.Caption = "Bill Addr1";
				_columnBillAddr1.MaxLength = 80;
				_columnBillAddr1.Unique = false;
				_columnBillAddr1.DefaultValue = Convert.DBNull ;
				_columnBillAddr1.ExtendedProperties.Add("IsKey", "false");
				_columnBillAddr1.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillAddr1.ExtendedProperties.Add("Description", "Bill Addr1");
				_columnBillAddr1.ExtendedProperties.Add("Length", "80");
				_columnBillAddr1.ExtendedProperties.Add("Decimals", "0");
				_columnBillAddr1.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillAddr1);
				
				_columnBillAddr2 = new DataColumn("BillAddr2", typeof(string), "", MappingType.Element);
				_columnBillAddr2.AllowDBNull = true;
				_columnBillAddr2.Caption = "Bill Addr2";
				_columnBillAddr2.MaxLength = 80;
				_columnBillAddr2.Unique = false;
				_columnBillAddr2.DefaultValue = Convert.DBNull ;
				_columnBillAddr2.ExtendedProperties.Add("IsKey", "false");
				_columnBillAddr2.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillAddr2.ExtendedProperties.Add("Description", "Bill Addr2");
				_columnBillAddr2.ExtendedProperties.Add("Length", "80");
				_columnBillAddr2.ExtendedProperties.Add("Decimals", "0");
				_columnBillAddr2.ExtendedProperties.Add("AllowDBNulls", "true");
				this.Columns.Add(_columnBillAddr2);
				
				_columnBillCity = new DataColumn("BillCity", typeof(string), "", MappingType.Element);
				_columnBillCity.AllowDBNull = false;
				_columnBillCity.Caption = "Bill City";
				_columnBillCity.MaxLength = 80;
				_columnBillCity.Unique = false;
				_columnBillCity.DefaultValue = Convert.DBNull ;
				_columnBillCity.ExtendedProperties.Add("IsKey", "false");
				_columnBillCity.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillCity.ExtendedProperties.Add("Description", "Bill City");
				_columnBillCity.ExtendedProperties.Add("Length", "80");
				_columnBillCity.ExtendedProperties.Add("Decimals", "0");
				_columnBillCity.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillCity);
				
				_columnBillState = new DataColumn("BillState", typeof(string), "", MappingType.Element);
				_columnBillState.AllowDBNull = false;
				_columnBillState.Caption = "Bill State";
				_columnBillState.MaxLength = 80;
				_columnBillState.Unique = false;
				_columnBillState.DefaultValue = Convert.DBNull ;
				_columnBillState.ExtendedProperties.Add("IsKey", "false");
				_columnBillState.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillState.ExtendedProperties.Add("Description", "Bill State");
				_columnBillState.ExtendedProperties.Add("Length", "80");
				_columnBillState.ExtendedProperties.Add("Decimals", "0");
				_columnBillState.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillState);
				
				_columnBillZip = new DataColumn("BillZip", typeof(string), "", MappingType.Element);
				_columnBillZip.AllowDBNull = false;
				_columnBillZip.Caption = "Bill Zip";
				_columnBillZip.MaxLength = 20;
				_columnBillZip.Unique = false;
				_columnBillZip.DefaultValue = Convert.DBNull ;
				_columnBillZip.ExtendedProperties.Add("IsKey", "false");
				_columnBillZip.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillZip.ExtendedProperties.Add("Description", "Bill Zip");
				_columnBillZip.ExtendedProperties.Add("Length", "20");
				_columnBillZip.ExtendedProperties.Add("Decimals", "0");
				_columnBillZip.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillZip);
				
				_columnBillCountry = new DataColumn("BillCountry", typeof(string), "", MappingType.Element);
				_columnBillCountry.AllowDBNull = false;
				_columnBillCountry.Caption = "Bill Country";
				_columnBillCountry.MaxLength = 20;
				_columnBillCountry.Unique = false;
				_columnBillCountry.DefaultValue = Convert.DBNull ;
				_columnBillCountry.ExtendedProperties.Add("IsKey", "false");
				_columnBillCountry.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillCountry.ExtendedProperties.Add("Description", "Bill Country");
				_columnBillCountry.ExtendedProperties.Add("Length", "20");
				_columnBillCountry.ExtendedProperties.Add("Decimals", "0");
				_columnBillCountry.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillCountry);
				
				_columnCourier = new DataColumn("Courier", typeof(string), "", MappingType.Element);
				_columnCourier.AllowDBNull = false;
				_columnCourier.Caption = "Courier";
				_columnCourier.MaxLength = 80;
				_columnCourier.Unique = false;
				_columnCourier.DefaultValue = Convert.DBNull ;
				_columnCourier.ExtendedProperties.Add("IsKey", "false");
				_columnCourier.ExtendedProperties.Add("ReadOnly", "false");
				_columnCourier.ExtendedProperties.Add("Description", "Courier");
				_columnCourier.ExtendedProperties.Add("Length", "80");
				_columnCourier.ExtendedProperties.Add("Decimals", "0");
				_columnCourier.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnCourier);
				
				_columnTotalPrice = new DataColumn("TotalPrice", typeof(decimal), "", MappingType.Element);
				_columnTotalPrice.AllowDBNull = false;
				_columnTotalPrice.Caption = "Total Price";
				_columnTotalPrice.Unique = false;
				_columnTotalPrice.DefaultValue = Convert.DBNull ;
				_columnTotalPrice.ExtendedProperties.Add("IsKey", "false");
				_columnTotalPrice.ExtendedProperties.Add("ReadOnly", "false");
				_columnTotalPrice.ExtendedProperties.Add("Description", "Total Price");
				_columnTotalPrice.ExtendedProperties.Add("Decimals", "0");
				_columnTotalPrice.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnTotalPrice);
				
				_columnBillToFirstName = new DataColumn("BillToFirstName", typeof(string), "", MappingType.Element);
				_columnBillToFirstName.AllowDBNull = false;
				_columnBillToFirstName.Caption = "Bill To First Name";
				_columnBillToFirstName.MaxLength = 80;
				_columnBillToFirstName.Unique = false;
				_columnBillToFirstName.DefaultValue = Convert.DBNull ;
				_columnBillToFirstName.ExtendedProperties.Add("IsKey", "false");
				_columnBillToFirstName.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillToFirstName.ExtendedProperties.Add("Description", "Bill To First Name");
				_columnBillToFirstName.ExtendedProperties.Add("Length", "80");
				_columnBillToFirstName.ExtendedProperties.Add("Decimals", "0");
				_columnBillToFirstName.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillToFirstName);
				
				_columnBillToLastName = new DataColumn("BillToLastName", typeof(string), "", MappingType.Element);
				_columnBillToLastName.AllowDBNull = false;
				_columnBillToLastName.Caption = "Bill To Last Name";
				_columnBillToLastName.MaxLength = 80;
				_columnBillToLastName.Unique = false;
				_columnBillToLastName.DefaultValue = Convert.DBNull ;
				_columnBillToLastName.ExtendedProperties.Add("IsKey", "false");
				_columnBillToLastName.ExtendedProperties.Add("ReadOnly", "false");
				_columnBillToLastName.ExtendedProperties.Add("Description", "Bill To Last Name");
				_columnBillToLastName.ExtendedProperties.Add("Length", "80");
				_columnBillToLastName.ExtendedProperties.Add("Decimals", "0");
				_columnBillToLastName.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnBillToLastName);
				
				_columnShipToFirstName = new DataColumn("ShipToFirstName", typeof(string), "", MappingType.Element);
				_columnShipToFirstName.AllowDBNull = false;
				_columnShipToFirstName.Caption = "Ship To First Name";
				_columnShipToFirstName.MaxLength = 80;
				_columnShipToFirstName.Unique = false;
				_columnShipToFirstName.DefaultValue = Convert.DBNull ;
				_columnShipToFirstName.ExtendedProperties.Add("IsKey", "false");
				_columnShipToFirstName.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipToFirstName.ExtendedProperties.Add("Description", "Ship To First Name");
				_columnShipToFirstName.ExtendedProperties.Add("Length", "80");
				_columnShipToFirstName.ExtendedProperties.Add("Decimals", "0");
				_columnShipToFirstName.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipToFirstName);
				
				_columnShipToLastName = new DataColumn("ShipToLastName", typeof(string), "", MappingType.Element);
				_columnShipToLastName.AllowDBNull = false;
				_columnShipToLastName.Caption = "Ship To Last Name";
				_columnShipToLastName.MaxLength = 80;
				_columnShipToLastName.Unique = false;
				_columnShipToLastName.DefaultValue = Convert.DBNull ;
				_columnShipToLastName.ExtendedProperties.Add("IsKey", "false");
				_columnShipToLastName.ExtendedProperties.Add("ReadOnly", "false");
				_columnShipToLastName.ExtendedProperties.Add("Description", "Ship To Last Name");
				_columnShipToLastName.ExtendedProperties.Add("Length", "80");
				_columnShipToLastName.ExtendedProperties.Add("Decimals", "0");
				_columnShipToLastName.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnShipToLastName);
				
				_columnAuthorizationNumber = new DataColumn("AuthorizationNumber", typeof(int), "", MappingType.Element);
				_columnAuthorizationNumber.AllowDBNull = false;
				_columnAuthorizationNumber.Caption = "Authorization Number";
				_columnAuthorizationNumber.Unique = false;
				_columnAuthorizationNumber.DefaultValue = Convert.DBNull ;
				_columnAuthorizationNumber.ExtendedProperties.Add("IsKey", "false");
				_columnAuthorizationNumber.ExtendedProperties.Add("ReadOnly", "false");
				_columnAuthorizationNumber.ExtendedProperties.Add("Description", "Authorization Number");
				_columnAuthorizationNumber.ExtendedProperties.Add("Decimals", "0");
				_columnAuthorizationNumber.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnAuthorizationNumber);
				
				_columnLocale = new DataColumn("Locale", typeof(string), "", MappingType.Element);
				_columnLocale.AllowDBNull = false;
				_columnLocale.Caption = "Locale";
				_columnLocale.MaxLength = 20;
				_columnLocale.Unique = false;
				_columnLocale.DefaultValue = Convert.DBNull ;
				_columnLocale.ExtendedProperties.Add("IsKey", "false");
				_columnLocale.ExtendedProperties.Add("ReadOnly", "false");
				_columnLocale.ExtendedProperties.Add("Description", "Locale");
				_columnLocale.ExtendedProperties.Add("Length", "20");
				_columnLocale.ExtendedProperties.Add("Decimals", "0");
				_columnLocale.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnLocale);
				
				this.PrimaryKey = new DataColumn[] {_columnOrderId};
			}
			
			public OrdersRow NewOrdersRow()
			{
				OrdersRow rowOrdersRow = ((OrdersRow)(this.NewRow()));
				return rowOrdersRow;
			}
			
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new OrdersRow(builder);
			}
			
			protected override Type GetRowType()
			{
				return typeof(OrdersRow);
			}
			
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.OrdersRowChanged != null)
				{
					this.OrdersRowChanged(this, new OrdersRowChangeEventArgs(((OrdersRow)(e.Row)), e.Action));
				}
			}
			
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.OrdersRowChanging != null)
				{
					this.OrdersRowChanging(this, new OrdersRowChangeEventArgs(((OrdersRow)(e.Row)), e.Action));
				}
			}
			
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.OrdersRowDeleted != null)
				{
					this.OrdersRowDeleted(this, new OrdersRowChangeEventArgs(((OrdersRow)(e.Row)), e.Action));
				}
			}
			
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.OrdersRowDeleting != null)
				{
					this.OrdersRowDeleting(this, new OrdersRowChangeEventArgs(((OrdersRow)(e.Row)), e.Action));
				}
			}
			
			public void RemoveOrdersRow(OrdersRow row)
			{
				this.Rows.Remove(row);
			}
		}
		
		public class OrdersRow: DataRow
		{
			private OrdersDataTable _tableOrders;
			
			internal OrdersRow(DataRowBuilder rb): base(rb)
			{
				_tableOrders = ((OrdersDataTable)(this.Table));
			}
			
			/// <summary>
			/// Gets or sets the value of OrderId property
			/// </summary>
			
			public int OrderId
			{
				get
				{
					try
					{
						return ((int)(this[_tableOrders.OrderIdColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value OrderId because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.OrderIdColumn] = value;
				}
			}
			
			public bool IsOrderIdNull()
			{
				return this.IsNull(_tableOrders.OrderIdColumn);
			}
			
			public void SetOrderIdNull()
			{
				this[_tableOrders.OrderIdColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of UserId property
			/// </summary>
			
			public string UserId
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.UserIdColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value UserId because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.UserIdColumn] = value;
				}
			}
			
			public bool IsUserIdNull()
			{
				return this.IsNull(_tableOrders.UserIdColumn);
			}
			
			public void SetUserIdNull()
			{
				this[_tableOrders.UserIdColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of OrderDate property
			/// </summary>
			
			public DateTime OrderDate
			{
				get
				{
					try
					{
						return ((DateTime)(this[_tableOrders.OrderDateColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value OrderDate because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.OrderDateColumn] = value;
				}
			}
			
			public bool IsOrderDateNull()
			{
				return this.IsNull(_tableOrders.OrderDateColumn);
			}
			
			public void SetOrderDateNull()
			{
				this[_tableOrders.OrderDateColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipAddr1 property
			/// </summary>
			
			public string ShipAddr1
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipAddr1Column]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipAddr1 because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipAddr1Column] = value;
				}
			}
			
			public bool IsShipAddr1Null()
			{
				return this.IsNull(_tableOrders.ShipAddr1Column);
			}
			
			public void SetShipAddr1Null()
			{
				this[_tableOrders.ShipAddr1Column] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipAddr2 property
			/// </summary>
			
			public string ShipAddr2
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipAddr2Column]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipAddr2 because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipAddr2Column] = value;
				}
			}
			
			public bool IsShipAddr2Null()
			{
				return this.IsNull(_tableOrders.ShipAddr2Column);
			}
			
			public void SetShipAddr2Null()
			{
				this[_tableOrders.ShipAddr2Column] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipCity property
			/// </summary>
			
			public string ShipCity
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipCityColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipCity because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipCityColumn] = value;
				}
			}
			
			public bool IsShipCityNull()
			{
				return this.IsNull(_tableOrders.ShipCityColumn);
			}
			
			public void SetShipCityNull()
			{
				this[_tableOrders.ShipCityColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipState property
			/// </summary>
			
			public string ShipState
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipStateColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipState because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipStateColumn] = value;
				}
			}
			
			public bool IsShipStateNull()
			{
				return this.IsNull(_tableOrders.ShipStateColumn);
			}
			
			public void SetShipStateNull()
			{
				this[_tableOrders.ShipStateColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipZip property
			/// </summary>
			
			public string ShipZip
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipZipColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipZip because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipZipColumn] = value;
				}
			}
			
			public bool IsShipZipNull()
			{
				return this.IsNull(_tableOrders.ShipZipColumn);
			}
			
			public void SetShipZipNull()
			{
				this[_tableOrders.ShipZipColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipCountry property
			/// </summary>
			
			public string ShipCountry
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipCountryColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipCountry because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipCountryColumn] = value;
				}
			}
			
			public bool IsShipCountryNull()
			{
				return this.IsNull(_tableOrders.ShipCountryColumn);
			}
			
			public void SetShipCountryNull()
			{
				this[_tableOrders.ShipCountryColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillAddr1 property
			/// </summary>
			
			public string BillAddr1
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillAddr1Column]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillAddr1 because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillAddr1Column] = value;
				}
			}
			
			public bool IsBillAddr1Null()
			{
				return this.IsNull(_tableOrders.BillAddr1Column);
			}
			
			public void SetBillAddr1Null()
			{
				this[_tableOrders.BillAddr1Column] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillAddr2 property
			/// </summary>
			
			public string BillAddr2
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillAddr2Column]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillAddr2 because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillAddr2Column] = value;
				}
			}
			
			public bool IsBillAddr2Null()
			{
				return this.IsNull(_tableOrders.BillAddr2Column);
			}
			
			public void SetBillAddr2Null()
			{
				this[_tableOrders.BillAddr2Column] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillCity property
			/// </summary>
			
			public string BillCity
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillCityColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillCity because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillCityColumn] = value;
				}
			}
			
			public bool IsBillCityNull()
			{
				return this.IsNull(_tableOrders.BillCityColumn);
			}
			
			public void SetBillCityNull()
			{
				this[_tableOrders.BillCityColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillState property
			/// </summary>
			
			public string BillState
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillStateColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillState because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillStateColumn] = value;
				}
			}
			
			public bool IsBillStateNull()
			{
				return this.IsNull(_tableOrders.BillStateColumn);
			}
			
			public void SetBillStateNull()
			{
				this[_tableOrders.BillStateColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillZip property
			/// </summary>
			
			public string BillZip
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillZipColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillZip because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillZipColumn] = value;
				}
			}
			
			public bool IsBillZipNull()
			{
				return this.IsNull(_tableOrders.BillZipColumn);
			}
			
			public void SetBillZipNull()
			{
				this[_tableOrders.BillZipColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillCountry property
			/// </summary>
			
			public string BillCountry
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillCountryColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillCountry because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillCountryColumn] = value;
				}
			}
			
			public bool IsBillCountryNull()
			{
				return this.IsNull(_tableOrders.BillCountryColumn);
			}
			
			public void SetBillCountryNull()
			{
				this[_tableOrders.BillCountryColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of Courier property
			/// </summary>
			
			public string Courier
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.CourierColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value Courier because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.CourierColumn] = value;
				}
			}
			
			public bool IsCourierNull()
			{
				return this.IsNull(_tableOrders.CourierColumn);
			}
			
			public void SetCourierNull()
			{
				this[_tableOrders.CourierColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of TotalPrice property
			/// </summary>
			
			public decimal TotalPrice
			{
				get
				{
					try
					{
						return ((decimal)(this[_tableOrders.TotalPriceColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value TotalPrice because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.TotalPriceColumn] = value;
				}
			}
			
			public bool IsTotalPriceNull()
			{
				return this.IsNull(_tableOrders.TotalPriceColumn);
			}
			
			public void SetTotalPriceNull()
			{
				this[_tableOrders.TotalPriceColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillToFirstName property
			/// </summary>
			
			public string BillToFirstName
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillToFirstNameColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillToFirstName because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillToFirstNameColumn] = value;
				}
			}
			
			public bool IsBillToFirstNameNull()
			{
				return this.IsNull(_tableOrders.BillToFirstNameColumn);
			}
			
			public void SetBillToFirstNameNull()
			{
				this[_tableOrders.BillToFirstNameColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of BillToLastName property
			/// </summary>
			
			public string BillToLastName
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.BillToLastNameColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value BillToLastName because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.BillToLastNameColumn] = value;
				}
			}
			
			public bool IsBillToLastNameNull()
			{
				return this.IsNull(_tableOrders.BillToLastNameColumn);
			}
			
			public void SetBillToLastNameNull()
			{
				this[_tableOrders.BillToLastNameColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipToFirstName property
			/// </summary>
			
			public string ShipToFirstName
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipToFirstNameColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipToFirstName because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipToFirstNameColumn] = value;
				}
			}
			
			public bool IsShipToFirstNameNull()
			{
				return this.IsNull(_tableOrders.ShipToFirstNameColumn);
			}
			
			public void SetShipToFirstNameNull()
			{
				this[_tableOrders.ShipToFirstNameColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of ShipToLastName property
			/// </summary>
			
			public string ShipToLastName
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.ShipToLastNameColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ShipToLastName because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.ShipToLastNameColumn] = value;
				}
			}
			
			public bool IsShipToLastNameNull()
			{
				return this.IsNull(_tableOrders.ShipToLastNameColumn);
			}
			
			public void SetShipToLastNameNull()
			{
				this[_tableOrders.ShipToLastNameColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of AuthorizationNumber property
			/// </summary>
			
			public int AuthorizationNumber
			{
				get
				{
					try
					{
						return ((int)(this[_tableOrders.AuthorizationNumberColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value AuthorizationNumber because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.AuthorizationNumberColumn] = value;
				}
			}
			
			public bool IsAuthorizationNumberNull()
			{
				return this.IsNull(_tableOrders.AuthorizationNumberColumn);
			}
			
			public void SetAuthorizationNumberNull()
			{
				this[_tableOrders.AuthorizationNumberColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of Locale property
			/// </summary>
			
			public string Locale
			{
				get
				{
					try
					{
						return ((string)(this[_tableOrders.LocaleColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value Locale because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableOrders.LocaleColumn] = value;
				}
			}
			
			public bool IsLocaleNull()
			{
				return this.IsNull(_tableOrders.LocaleColumn);
			}
			
			public void SetLocaleNull()
			{
				this[_tableOrders.LocaleColumn] = Convert.DBNull;
			}
			
		}
		
		public class OrdersRowChangeEventArgs: EventArgs
		{
			private OrdersRow _eventRow;
			private System.Data.DataRowAction _eventAction;
			
			public OrdersRowChangeEventArgs(OrdersRow row, DataRowAction action)
			{
				_eventRow = row;
				_eventAction = action;
			}
			
			public OrdersRow Row
			{
				get
				{
					return _eventRow;
				}
			}
			
			public DataRowAction Action
			{
				get
				{
					return _eventAction;
				}
			}
		}
	}
	#endregion
	
	#region OrdersDataAdapter
	public class OrdersDataAdapter: MarshalByRefObject, IDataAdapter
	{
		#region Member Variables
		private IDbConnection _connection;
		private IDbTransaction _transaction;
		private IDbCommand _command;
		private IDataReader _reader;
		private int _connectionTimeout = 30;
		private int _commandTimeout = 30;
		private string _connectionStringKey;
		private bool _autoCloseConnection = true;
		private bool _autoCommitTransaction = true;
		private bool _convertEmptyValuesToDBNull = true;
		//private IDataParameter[] _fillDataParameters;
		#endregion
		
		#region Constructors
		public OrdersDataAdapter()
		{
			_connectionStringKey = "ConnectionString";
		}
		
		public OrdersDataAdapter(string connectionStringKey)
		{
			_connectionStringKey = connectionStringKey + "_ConnectionString";
		}
		
		public OrdersDataAdapter(IDbConnection connection)
		{
			this.Connection = connection;
		}
		
		public OrdersDataAdapter(IDbTransaction transaction)
		{
			this.Transaction = transaction;
		}
		#endregion
		
		#region Properties
		public IDbConnection Connection
		{
			get
			{
				if (_connection == null)
				{
					_connection = new SqlConnection();
					_connection.ConnectionString = System.Configuration.ConfigurationSettings.AppSettings[this.ConnectionStringKey];
					//_connection.ConnectionTimeout = this.ConnectionTimeout;
				}
				return _connection;
			}
			set
			{
				_connection = value;
				// if the connection is passed in then it should be up to the owner to close the connection.
				_autoCloseConnection = false;
			}
		}
		
		public IDbTransaction Transaction
		{
			get	{return _transaction;}
			set
			{
				_transaction = value;
				_connection = _transaction.Connection;
				// if the connection is passed in then it should be up to the owner to close the connection.
				_autoCloseConnection = false;
				// if the transaction is passed in then it should be up to the owner of the transaction to commit.
				_autoCommitTransaction = false;
			}
		}
		
		public bool AutoCloseConnection
		{
			get {return _autoCloseConnection;}
			set {_autoCloseConnection = value;}
		}
		
		public bool AutoCommitTransaction
		{
			get {return _autoCommitTransaction;}
			set {_autoCommitTransaction = value;}
		}
		
		public bool ConvertEmptyValuesToDBNull
		{
			get {return _convertEmptyValuesToDBNull;}
			set {_convertEmptyValuesToDBNull = value;}
		}
		
		public string ConnectionStringKey
		{
			get {return _connectionStringKey;}
		}
		
		public int ConnectionTimeout
		{
			get	{return _connectionTimeout;}
			set	{_connectionTimeout = value;}
		}
		
		public int CommandTimeout
		{
			get	{return _commandTimeout;}
			set	{_commandTimeout = value;}
		}
		
		public MissingMappingAction MissingMappingAction
		{
			get	{return MissingMappingAction.Passthrough;}
			set {}
		}
		
		public MissingSchemaAction MissingSchemaAction
		{
			get	{return MissingSchemaAction.Ignore;}
			set	{}
		}
		
		public ITableMappingCollection TableMappings
		{
			get
			{
				System.Data.Common.DataTableMappingCollection tableMaps;
				tableMaps = new System.Data.Common.DataTableMappingCollection();
				return tableMaps;
			}
		}
		#endregion
		
		#region Helper Methods
		private IDbCommand GetCommand()
		{
			if (this.Connection != null)
			{
				_command = this.Connection.CreateCommand();
				_command.CommandTimeout = this.CommandTimeout;
				_command.CommandType = CommandType.Text;
				_command.Connection = this.Connection;
				if (_transaction != null) _command.Transaction = _transaction;
				
				return _command;
			}
			else
			{
				throw new InvalidOperationException("You must have a valid Connection object before calling GetCommand.");
			}
		}
		
		private void OpenConnection()
		{
			if (this.Connection != null)
			{
				if (this.Connection.State == ConnectionState.Closed) _connection.Open();
			}
			else
			{
				throw new InvalidOperationException("You must have a valid Connection object before calling GetCommand.");
			}
		}

		private void Cleanup()
		{
			try
			{
				if (_reader != null)
				{
					if (!_reader.IsClosed) _reader.Close();
					_reader.Dispose();
					_reader = null;
				}
				
				if (_command != null)
				{
					_command.Dispose();
					_command = null;
				}
				
				if (_connection != null && this.AutoCloseConnection == true)
				{
					if (_connection.State == ConnectionState.Open) _connection.Close();
					_connection.Dispose();
					_connection = null;
				}
			}
			catch {}
		}
		#endregion
		
		#region CreateParameter
		public IDbDataParameter CreateParameter(string name, DbType type, object value)
		{
			IDbDataParameter prm = _command.CreateParameter();
			prm.Direction = ParameterDirection.Input;
			prm.ParameterName = name;
			prm.DbType = type;
			prm.Value = this.PrepareParameterValue(value);
			
			return prm;
		}
		
		public IDbDataParameter CreateParameter(string name, DbType type, object value, int size)
		{
			IDbDataParameter prm = _command.CreateParameter();
			prm.Direction = ParameterDirection.Input;
			prm.ParameterName = name;
			prm.DbType = type;
			prm.Size = size;
			prm.Value = this.PrepareParameterValue(value);
			
			return prm;
		}
		
		public IDbDataParameter CreateParameter(string name, DbType type, object value, ParameterDirection direction)
		{
			IDbDataParameter prm = _command.CreateParameter();
			prm.Direction = direction;
			prm.ParameterName = name;
			prm.DbType = type;
			prm.Value = this.PrepareParameterValue(value);
			
			return prm;
		}
		
		public IDbDataParameter CreateParameter(string name, DbType type, object value, int size, ParameterDirection direction)
		{
			IDbDataParameter prm = _command.CreateParameter();
			prm.Direction = direction;
			prm.ParameterName = name;
			prm.DbType = type;
			prm.Size = size;
			prm.Value = this.PrepareParameterValue(value);
			
			return prm;
		}
		
		private object PrepareParameterValue(object value)
		{
			return PrepareParameterValue(value, false);
		}
		
		private object PrepareParameterValue(object value, bool convertZeroToDBNull)
		{
			if (!_convertEmptyValuesToDBNull) return value;
			
			switch (value.GetType().ToString())
			{
				case "System.String":
					if (Convert.ToString(value) == String.Empty)
					{
						return DBNull.Value;
					}
					else
					{
						return value;
					}
				case "System.Guid":
					if (new Guid(Convert.ToString(value)) == Guid.Empty)
					{
						return DBNull.Value;
					}
					else
					{
						return value;
					}
				case "System.DateTime":
					if (Convert.ToDateTime(value) == DateTime.MinValue)
					{
						return DBNull.Value;
					}
					else
					{
						return value;
					}
				case "System.Int16":
					if (Convert.ToInt16(value) == 0)
					{
						if (convertZeroToDBNull)
						{
							return DBNull.Value;
						}
						else
						{
							return value;
						}
					}
					else
					{
						return value;
					}
				case "System.Int32":
					if (Convert.ToInt32(value) == 0)
					{
						if (convertZeroToDBNull)
						{
							return DBNull.Value;
						}
						else
						{
							return value;
						}
					}
					else
					{
						return value;
					}
				case "System.Int64":
					if (Convert.ToInt64(value) == 0)
					{
						if (convertZeroToDBNull)
						{
							return DBNull.Value;
						}
						else
						{
							return value;
						}
					}
					else
					{
						return value;
					}
				case "System.Single":
					if (Convert.ToSingle(value) == 0)
					{
						if (convertZeroToDBNull)
						{
							return DBNull.Value;
						}
						else
						{
							return value;
						}
					}
					else
					{
						return value;
					}
				case "System.Double":
					if (Convert.ToDouble(value) == 0)
					{
						if (convertZeroToDBNull)
						{
							return DBNull.Value;
						}
						else
						{
							return value;
						}
					}
					else
					{
						return value;
					}
				case "System.Decimal":
					if (Convert.ToDecimal(value) == 0)
					{
						if (convertZeroToDBNull)
						{
							return DBNull.Value;
						}
						else
						{
							return value;
						}
					}
					else
					{
						return value;
					}
				default:
					return value;
			}
		}
		#endregion AddParameter
		
		#region Fill Methods
		
		public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			DataTable[] dataTables;
			dataTables = new DataTable[dataSet.Tables.Count];
			dataSet.Tables.CopyTo(dataTables, dataSet.Tables.Count);
			return dataTables;
		}
		
		public int Fill(OrdersDataSet dataSet, IDataRecord dataRecord)
		{
			return Fill(dataSet, ((int)(dataRecord["OrderId"])));
		}
		
		public int Fill(OrdersDataSet dataSet, DataRow dataRecord)
		{
			return Fill(dataSet, ((int)(dataRecord["OrderId"])));
		}
		
		public int Fill(OrdersDataSet dataSet, int orderId)
		{
			try
			{
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[OrderId],
						[UserId],
						[OrderDate],
						[ShipAddr1],
						[ShipAddr2],
						[ShipCity],
						[ShipState],
						[ShipZip],
						[ShipCountry],
						[BillAddr1],
						[BillAddr2],
						[BillCity],
						[BillState],
						[BillZip],
						[BillCountry],
						[Courier],
						[TotalPrice],
						[BillToFirstName],
						[BillToLastName],
						[ShipToFirstName],
						[ShipToLastName],
						[AuthorizationNumber],
						[Locale]
					FROM
						[Orders]
					WHERE
						[OrderId] = @OrderId
					";
				_command.Parameters.Add(this.CreateParameter("@OrderId", DbType.Int32, orderId));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
				if (_reader.Read())
				{
					OrdersDataSet.OrdersRow row = dataSet.Orders.NewOrdersRow();
					this.PopulateOrdersDataRow(_reader, row);
					dataSet.Orders.AddOrdersRow(row);
					dataSet.AcceptChanges();
					
					return 1;
				}
				else
				{
					throw new OrdersNotFoundException();
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return 0;
			}
			finally
			{
				this.Cleanup();
			}
		}
		
		private void PopulateOrdersDataRow(IDataReader reader, OrdersDataSet.OrdersRow row)
		{
			if (!reader.IsDBNull(0)) row.OrderId = reader.GetInt32(0);
			if (!reader.IsDBNull(1)) row.UserId = reader.GetString(1);
			if (!reader.IsDBNull(2)) row.OrderDate = reader.GetDateTime(2);
			if (!reader.IsDBNull(3)) row.ShipAddr1 = reader.GetString(3);
			if (!reader.IsDBNull(4)) row.ShipAddr2 = reader.GetString(4);
			if (!reader.IsDBNull(5)) row.ShipCity = reader.GetString(5);
			if (!reader.IsDBNull(6)) row.ShipState = reader.GetString(6);
			if (!reader.IsDBNull(7)) row.ShipZip = reader.GetString(7);
			if (!reader.IsDBNull(8)) row.ShipCountry = reader.GetString(8);
			if (!reader.IsDBNull(9)) row.BillAddr1 = reader.GetString(9);
			if (!reader.IsDBNull(10)) row.BillAddr2 = reader.GetString(10);
			if (!reader.IsDBNull(11)) row.BillCity = reader.GetString(11);
			if (!reader.IsDBNull(12)) row.BillState = reader.GetString(12);
			if (!reader.IsDBNull(13)) row.BillZip = reader.GetString(13);
			if (!reader.IsDBNull(14)) row.BillCountry = reader.GetString(14);
			if (!reader.IsDBNull(15)) row.Courier = reader.GetString(15);
			if (!reader.IsDBNull(16)) row.TotalPrice = reader.GetDecimal(16);
			if (!reader.IsDBNull(17)) row.BillToFirstName = reader.GetString(17);
			if (!reader.IsDBNull(18)) row.BillToLastName = reader.GetString(18);
			if (!reader.IsDBNull(19)) row.ShipToFirstName = reader.GetString(19);
			if (!reader.IsDBNull(20)) row.ShipToLastName = reader.GetString(20);
			if (!reader.IsDBNull(21)) row.AuthorizationNumber = reader.GetInt32(21);
			if (!reader.IsDBNull(22)) row.Locale = reader.GetString(22);
		}
		
		public int Fill(DataSet dataSet)
		{
			OrdersDataSet pageDataSet = dataSet as OrdersDataSet;
			if (pageDataSet != null)
			{
				return this.Fill(pageDataSet);
			}
			else
			{
				throw new ApplicationException();
			}
		}
		
		public int Fill(OrdersDataSet dataSet, string[] columns, string[] values, DbType[] types)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[OrderId],
						[UserId],
						[OrderDate],
						[ShipAddr1],
						[ShipAddr2],
						[ShipCity],
						[ShipState],
						[ShipZip],
						[ShipCountry],
						[BillAddr1],
						[BillAddr2],
						[BillCity],
						[BillState],
						[BillZip],
						[BillCountry],
						[Courier],
						[TotalPrice],
						[BillToFirstName],
						[BillToLastName],
						[ShipToFirstName],
						[ShipToLastName],
						[AuthorizationNumber],
						[Locale]
					FROM
						[Orders]
					WHERE ";
				
				for(int i = 0;i < columns.Length; i++)
				{
					_command.CommandText += columns[i] + " = " + (types[i] == DbType.AnsiString ? "'" + values[i] + "'" : values[i]);
					if(i < columns.Length - 1)
						_command.CommandText += " AND ";
				}
				for(int i = 0;i < columns.Length; i++)
					_command.Parameters.Add(this.CreateParameter("@" + columns[i], types[i], columns[i]));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					OrdersDataSet.OrdersRow row = dataSet.Orders.NewOrdersRow();
					this.PopulateOrdersDataRow(_reader, row);
					dataSet.Orders.AddOrdersRow(row);
					
					recordcount++;
				}
				dataSet.AcceptChanges();
				
				return recordcount;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return 0;
			}
			finally
			{
				this.Cleanup();
			}
		}
		
		public int Fill(OrdersDataSet dataSet)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[OrderId],
						[UserId],
						[OrderDate],
						[ShipAddr1],
						[ShipAddr2],
						[ShipCity],
						[ShipState],
						[ShipZip],
						[ShipCountry],
						[BillAddr1],
						[BillAddr2],
						[BillCity],
						[BillState],
						[BillZip],
						[BillCountry],
						[Courier],
						[TotalPrice],
						[BillToFirstName],
						[BillToLastName],
						[ShipToFirstName],
						[ShipToLastName],
						[AuthorizationNumber],
						[Locale]
					FROM
						[Orders]";
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					OrdersDataSet.OrdersRow row = dataSet.Orders.NewOrdersRow();
					this.PopulateOrdersDataRow(_reader, row);
					dataSet.Orders.AddOrdersRow(row);
					
					recordcount++;
				}
				dataSet.AcceptChanges();
				
				return recordcount;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return 0;
			}
			finally
			{
				this.Cleanup();
			}
		}
		public int FillByOrderId(OrdersDataSet dataSet, int orderId)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[OrderId],
						[UserId],
						[OrderDate],
						[ShipAddr1],
						[ShipAddr2],
						[ShipCity],
						[ShipState],
						[ShipZip],
						[ShipCountry],
						[BillAddr1],
						[BillAddr2],
						[BillCity],
						[BillState],
						[BillZip],
						[BillCountry],
						[Courier],
						[TotalPrice],
						[BillToFirstName],
						[BillToLastName],
						[ShipToFirstName],
						[ShipToLastName],
						[AuthorizationNumber],
						[Locale]
					FROM
						[Orders]
					WHERE
						[OrderId] = @OrderId
						";
				
				_command.Parameters.Add(this.CreateParameter("@OrderId", DbType.Int32, orderId));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					OrdersDataSet.OrdersRow row = dataSet.Orders.NewOrdersRow();
					this.PopulateOrdersDataRow(_reader, row);
					dataSet.Orders.AddOrdersRow(row);
					
					recordcount++;
				}
				dataSet.AcceptChanges();
				
				return recordcount;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return 0;
			}
			finally
			{
				this.Cleanup();
			}
		}
		
		public IDataParameter[] GetFillParameters()
		{
			// not sure if I should create a OrdersId parameter here or not.
			return null; //_fillDataParameters;
		}
		#endregion
		
		#region Update Methods
		public int Update(DataSet dataSet)
		{
			OrdersDataSet pageDataSet = dataSet as OrdersDataSet;
			if (pageDataSet != null)
			{
				return this.Update(pageDataSet);
			}
			else
			{
				throw new ApplicationException();
			}
		}
		
		public int Update(OrdersDataSet dataSet)
		{
			if (dataSet != null)
			{
				try
				{
					int updatedRowCount = 0;
					
					foreach(OrdersDataSet.OrdersRow row in dataSet.Orders)
					{
						switch (row.RowState)
						{
							case DataRowState.Added:
							{
								OnOrdersUpdating(new OrdersEventArgs(row, StatementType.Insert)) ;
								_command = this.GetCommand();
								_command.CommandText = @"
									INSERT INTO [Orders] (
										[UserId],
										[OrderDate],
										[ShipAddr1],
										[ShipAddr2],
										[ShipCity],
										[ShipState],
										[ShipZip],
										[ShipCountry],
										[BillAddr1],
										[BillAddr2],
										[BillCity],
										[BillState],
										[BillZip],
										[BillCountry],
										[Courier],
										[TotalPrice],
										[BillToFirstName],
										[BillToLastName],
										[ShipToFirstName],
										[ShipToLastName],
										[AuthorizationNumber],
										[Locale]
									) VALUES (
										@UserId,
										@OrderDate,
										@ShipAddr1,
										@ShipAddr2,
										@ShipCity,
										@ShipState,
										@ShipZip,
										@ShipCountry,
										@BillAddr1,
										@BillAddr2,
										@BillCity,
										@BillState,
										@BillZip,
										@BillCountry,
										@Courier,
										@TotalPrice,
										@BillToFirstName,
										@BillToLastName,
										@ShipToFirstName,
										@ShipToLastName,
										@AuthorizationNumber,
										@Locale
									)";
								_command.Parameters.Add(this.CreateParameter("@UserId", DbType.AnsiString, row.IsUserIdNull() ? (object)DBNull.Value : (object)row.UserId));
								_command.Parameters.Add(this.CreateParameter("@OrderDate", DbType.DateTime, row.IsOrderDateNull() ? (object)DBNull.Value : (object)row.OrderDate));
								_command.Parameters.Add(this.CreateParameter("@ShipAddr1", DbType.AnsiString, row.IsShipAddr1Null() ? (object)DBNull.Value : (object)row.ShipAddr1));
								_command.Parameters.Add(this.CreateParameter("@ShipAddr2", DbType.AnsiString, row.IsShipAddr2Null() ? (object)DBNull.Value : (object)row.ShipAddr2));
								_command.Parameters.Add(this.CreateParameter("@ShipCity", DbType.AnsiString, row.IsShipCityNull() ? (object)DBNull.Value : (object)row.ShipCity));
								_command.Parameters.Add(this.CreateParameter("@ShipState", DbType.AnsiString, row.IsShipStateNull() ? (object)DBNull.Value : (object)row.ShipState));
								_command.Parameters.Add(this.CreateParameter("@ShipZip", DbType.AnsiString, row.IsShipZipNull() ? (object)DBNull.Value : (object)row.ShipZip));
								_command.Parameters.Add(this.CreateParameter("@ShipCountry", DbType.AnsiString, row.IsShipCountryNull() ? (object)DBNull.Value : (object)row.ShipCountry));
								_command.Parameters.Add(this.CreateParameter("@BillAddr1", DbType.AnsiString, row.IsBillAddr1Null() ? (object)DBNull.Value : (object)row.BillAddr1));
								_command.Parameters.Add(this.CreateParameter("@BillAddr2", DbType.AnsiString, row.IsBillAddr2Null() ? (object)DBNull.Value : (object)row.BillAddr2));
								_command.Parameters.Add(this.CreateParameter("@BillCity", DbType.AnsiString, row.IsBillCityNull() ? (object)DBNull.Value : (object)row.BillCity));
								_command.Parameters.Add(this.CreateParameter("@BillState", DbType.AnsiString, row.IsBillStateNull() ? (object)DBNull.Value : (object)row.BillState));
								_command.Parameters.Add(this.CreateParameter("@BillZip", DbType.AnsiString, row.IsBillZipNull() ? (object)DBNull.Value : (object)row.BillZip));
								_command.Parameters.Add(this.CreateParameter("@BillCountry", DbType.AnsiString, row.IsBillCountryNull() ? (object)DBNull.Value : (object)row.BillCountry));
								_command.Parameters.Add(this.CreateParameter("@Courier", DbType.AnsiString, row.IsCourierNull() ? (object)DBNull.Value : (object)row.Courier));
								_command.Parameters.Add(this.CreateParameter("@TotalPrice", DbType.Decimal, row.IsTotalPriceNull() ? (object)DBNull.Value : (object)row.TotalPrice));
								_command.Parameters.Add(this.CreateParameter("@BillToFirstName", DbType.AnsiString, row.IsBillToFirstNameNull() ? (object)DBNull.Value : (object)row.BillToFirstName));
								_command.Parameters.Add(this.CreateParameter("@BillToLastName", DbType.AnsiString, row.IsBillToLastNameNull() ? (object)DBNull.Value : (object)row.BillToLastName));
								_command.Parameters.Add(this.CreateParameter("@ShipToFirstName", DbType.AnsiString, row.IsShipToFirstNameNull() ? (object)DBNull.Value : (object)row.ShipToFirstName));
								_command.Parameters.Add(this.CreateParameter("@ShipToLastName", DbType.AnsiString, row.IsShipToLastNameNull() ? (object)DBNull.Value : (object)row.ShipToLastName));
								_command.Parameters.Add(this.CreateParameter("@AuthorizationNumber", DbType.Int32, row.IsAuthorizationNumberNull() ? (object)DBNull.Value : (object)row.AuthorizationNumber));
								_command.Parameters.Add(this.CreateParameter("@Locale", DbType.AnsiString, row.IsLocaleNull() ? (object)DBNull.Value : (object)row.Locale));
								this.OpenConnection();
								_command.ExecuteNonQuery();
								OnOrdersUpdated(new OrdersEventArgs(row, StatementType.Insert)) ;
								
								updatedRowCount++;
								break;
							}
							case DataRowState.Modified:
							{
								OnOrdersUpdating(new OrdersEventArgs(row, StatementType.Update)) ;
								_command = this.GetCommand();
								_command.CommandText = @"
									UPDATE [Orders] SET
										[UserId] = @UserId,
										[OrderDate] = @OrderDate,
										[ShipAddr1] = @ShipAddr1,
										[ShipAddr2] = @ShipAddr2,
										[ShipCity] = @ShipCity,
										[ShipState] = @ShipState,
										[ShipZip] = @ShipZip,
										[ShipCountry] = @ShipCountry,
										[BillAddr1] = @BillAddr1,
										[BillAddr2] = @BillAddr2,
										[BillCity] = @BillCity,
										[BillState] = @BillState,
										[BillZip] = @BillZip,
										[BillCountry] = @BillCountry,
										[Courier] = @Courier,
										[TotalPrice] = @TotalPrice,
										[BillToFirstName] = @BillToFirstName,
										[BillToLastName] = @BillToLastName,
										[ShipToFirstName] = @ShipToFirstName,
										[ShipToLastName] = @ShipToLastName,
										[AuthorizationNumber] = @AuthorizationNumber,
										[Locale] = @Locale
									WHERE
										[OrderId] = @OrderId
									";
								_command.Parameters.Add(this.CreateParameter("@OrderId", DbType.Int32, row.IsOrderIdNull() ? (object)DBNull.Value : (object)row.OrderId));
								_command.Parameters.Add(this.CreateParameter("@UserId", DbType.AnsiString, row.IsUserIdNull() ? (object)DBNull.Value : (object)row.UserId));
								_command.Parameters.Add(this.CreateParameter("@OrderDate", DbType.DateTime, row.IsOrderDateNull() ? (object)DBNull.Value : (object)row.OrderDate));
								_command.Parameters.Add(this.CreateParameter("@ShipAddr1", DbType.AnsiString, row.IsShipAddr1Null() ? (object)DBNull.Value : (object)row.ShipAddr1));
								_command.Parameters.Add(this.CreateParameter("@ShipAddr2", DbType.AnsiString, row.IsShipAddr2Null() ? (object)DBNull.Value : (object)row.ShipAddr2));
								_command.Parameters.Add(this.CreateParameter("@ShipCity", DbType.AnsiString, row.IsShipCityNull() ? (object)DBNull.Value : (object)row.ShipCity));
								_command.Parameters.Add(this.CreateParameter("@ShipState", DbType.AnsiString, row.IsShipStateNull() ? (object)DBNull.Value : (object)row.ShipState));
								_command.Parameters.Add(this.CreateParameter("@ShipZip", DbType.AnsiString, row.IsShipZipNull() ? (object)DBNull.Value : (object)row.ShipZip));
								_command.Parameters.Add(this.CreateParameter("@ShipCountry", DbType.AnsiString, row.IsShipCountryNull() ? (object)DBNull.Value : (object)row.ShipCountry));
								_command.Parameters.Add(this.CreateParameter("@BillAddr1", DbType.AnsiString, row.IsBillAddr1Null() ? (object)DBNull.Value : (object)row.BillAddr1));
								_command.Parameters.Add(this.CreateParameter("@BillAddr2", DbType.AnsiString, row.IsBillAddr2Null() ? (object)DBNull.Value : (object)row.BillAddr2));
								_command.Parameters.Add(this.CreateParameter("@BillCity", DbType.AnsiString, row.IsBillCityNull() ? (object)DBNull.Value : (object)row.BillCity));
								_command.Parameters.Add(this.CreateParameter("@BillState", DbType.AnsiString, row.IsBillStateNull() ? (object)DBNull.Value : (object)row.BillState));
								_command.Parameters.Add(this.CreateParameter("@BillZip", DbType.AnsiString, row.IsBillZipNull() ? (object)DBNull.Value : (object)row.BillZip));
								_command.Parameters.Add(this.CreateParameter("@BillCountry", DbType.AnsiString, row.IsBillCountryNull() ? (object)DBNull.Value : (object)row.BillCountry));
								_command.Parameters.Add(this.CreateParameter("@Courier", DbType.AnsiString, row.IsCourierNull() ? (object)DBNull.Value : (object)row.Courier));
								_command.Parameters.Add(this.CreateParameter("@TotalPrice", DbType.Decimal, row.IsTotalPriceNull() ? (object)DBNull.Value : (object)row.TotalPrice));
								_command.Parameters.Add(this.CreateParameter("@BillToFirstName", DbType.AnsiString, row.IsBillToFirstNameNull() ? (object)DBNull.Value : (object)row.BillToFirstName));
								_command.Parameters.Add(this.CreateParameter("@BillToLastName", DbType.AnsiString, row.IsBillToLastNameNull() ? (object)DBNull.Value : (object)row.BillToLastName));
								_command.Parameters.Add(this.CreateParameter("@ShipToFirstName", DbType.AnsiString, row.IsShipToFirstNameNull() ? (object)DBNull.Value : (object)row.ShipToFirstName));
								_command.Parameters.Add(this.CreateParameter("@ShipToLastName", DbType.AnsiString, row.IsShipToLastNameNull() ? (object)DBNull.Value : (object)row.ShipToLastName));
								_command.Parameters.Add(this.CreateParameter("@AuthorizationNumber", DbType.Int32, row.IsAuthorizationNumberNull() ? (object)DBNull.Value : (object)row.AuthorizationNumber));
								_command.Parameters.Add(this.CreateParameter("@Locale", DbType.AnsiString, row.IsLocaleNull() ? (object)DBNull.Value : (object)row.Locale));
								this.OpenConnection();
								_command.ExecuteNonQuery();
								OnOrdersUpdated(new OrdersEventArgs(row, StatementType.Update)) ;
								
								updatedRowCount++;
								break;
							}
							case DataRowState.Deleted:
							{
								OnOrdersUpdating(new OrdersEventArgs(row, StatementType.Delete)) ;
								_command = this.GetCommand();
								_command.CommandText = @"
									DELETE FROM [Orders]
									WHERE
										[OrderId] = @OrderId
									";
								_command.Parameters.Add(this.CreateParameter("@OrderId", DbType.Int32, row[dataSet.Orders.OrderIdColumn, DataRowVersion.Original]));
								this.OpenConnection();
								_command.ExecuteNonQuery();
								OnOrdersUpdated(new OrdersEventArgs(row, StatementType.Delete)) ;
								
								updatedRowCount++;
								break;
							}
						}
					}
					dataSet.AcceptChanges();
					
					return updatedRowCount;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.ToString());
					return 0;
				}
				finally
				{
					this.Cleanup();
				}
			}
			else
			{
				this.Cleanup();
				throw new ArgumentException("DataSet null");
			}
		}
		#endregion
		
		#region Events
		public delegate void OrdersUpdateEventHandler(object sender, OrdersEventArgs e);
		
		public event OrdersUpdateEventHandler OrdersUpdated;
		private void OnOrdersUpdated(OrdersEventArgs e)
		{
			if ((this.OrdersUpdated != null))
			{
				this.OrdersUpdated(this, e);
			}
		}
		
		public event OrdersUpdateEventHandler OrdersUpdating;
		private void OnOrdersUpdating(OrdersEventArgs e)
		{
			if ((this.OrdersUpdating != null))
			{
				this.OrdersUpdating(this, e);
			}
		}
		
		public class OrdersEventArgs : EventArgs
		{
			private StatementType _statementType;
			private OrdersDataSet.OrdersRow _dataRow;
			
			public OrdersEventArgs(OrdersDataSet.OrdersRow row, StatementType statementType)
			{
				_dataRow = row;
				_statementType = statementType;
			}
			
			public StatementType StatementType
			{
				get {return _statementType;}

			}
			
			public OrdersDataSet.OrdersRow Row
			{
				get {return _dataRow;}
				set	{_dataRow = value;}
			}
		}
		#endregion
		
		#region Custom Exceptions
		[Serializable()]
		public class OrdersNotFoundException: ApplicationException
		{
			public OrdersNotFoundException()
			{
			}
			
			public OrdersNotFoundException(string message) : base(message)
			{
			}
			
			public OrdersNotFoundException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected OrdersNotFoundException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		
		[Serializable()]
		public class ForeignKeyNotFoundException: ApplicationException
		{
			public ForeignKeyNotFoundException()
			{
			}
			
			public ForeignKeyNotFoundException(string message): base(message)
			{
			}
			
			public ForeignKeyNotFoundException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected ForeignKeyNotFoundException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		
		[Serializable()]
		public class OrdersDataLockedException: ApplicationException
		{
			public OrdersDataLockedException()
			{
			}
			
			public OrdersDataLockedException(string message): base(message)
			{
			}
			
			public OrdersDataLockedException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected OrdersDataLockedException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}

		[Serializable()]
		public class OrdersDataChangedException: ApplicationException
		{
			public OrdersDataChangedException()
			{
			}
			
			public OrdersDataChangedException(string message): base(message)
			{
			}
			
			public OrdersDataChangedException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected OrdersDataChangedException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		
		[Serializable()]
		public class OrdersDuplicateKeyException: ApplicationException
		{
			public OrdersDuplicateKeyException()
			{
			}
			
			public OrdersDuplicateKeyException(string message): base(message)
			{
			}
			
			public OrdersDuplicateKeyException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected OrdersDuplicateKeyException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		
		[Serializable()]
		public class OrdersDataDeletedException: ApplicationException
		{
			public OrdersDataDeletedException()
			{
			}
			
			public OrdersDataDeletedException(string message) : base(message)
			{
			}
			
			public OrdersDataDeletedException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected OrdersDataDeletedException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		#endregion
	}
	#endregion
}
