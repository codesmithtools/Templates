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

namespace TypedDataSetTester
{
	#region ProductDataSet
	[Serializable()]
	[DesignerCategoryAttribute("code")]
	[System.Diagnostics.DebuggerStepThrough()]
	[ToolboxItem(true)]
	public class ProductDataSet: DataSet
	{
		private ProductDataTable _tableProduct;
		
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		public ProductDataTable Product
		{
			get
			{
				return this._tableProduct;
			}
		}
		
		public ProductDataSet()
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
			if ((ds.Tables["Product"] != null))
			{
				this.Tables.Add(new ProductDataTable(ds.Tables["Product"]));
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
		
		protected ProductDataSet(SerializationInfo info, StreamingContext context)
		{
			string strSchema = ((string)(info.GetValue("XmlSchema", typeof(string))));
			if ((strSchema != null))
			{
				DataSet ds = new DataSet();
				ds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(strSchema)));
				if ((ds.Tables["Product"] != null))
				{
					this.Tables.Add(new ProductDataTable(ds.Tables["Product"]));
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
			this.DataSetName = "ProductDataSet";
			_tableProduct = new ProductDataTable();
			this.Tables.Add(_tableProduct);
			this.ExtendedProperties.Add("DataAdapterName", "ProductDataAdapter");
			this.ExtendedProperties.Add("ObjectName", "Product");
			this.ExtendedProperties.Add("ObjectDescription", "Product");
			this.ExtendedProperties.Add("NameSpace", "");
		}
		
		public override DataSet Clone()
		{
			ProductDataSet cln = ((ProductDataSet)(base.Clone()));
			cln.InitVars();
			return cln;
		}
		
		internal void InitVars()
		{
			_tableProduct = ((ProductDataTable)(this.Tables["Product"]));
			if (_tableProduct != null)
			{
				_tableProduct.InitVars();
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
		
		private bool ShouldSerializeProduct()
		{
			return false;
		}
		
		public delegate void ProductRowChangeEventHandler(object sender, ProductRowChangeEventArgs e);
		
		[Serializable()]
		public class ProductDataTable: DataTable, System.Collections.IEnumerable
		{
			private DataColumn _columnProductId;
			private DataColumn _columnCategoryId;
			private DataColumn _columnName;
			private DataColumn _columnDescn;
			private DataColumn _columnImage;
			
			internal ProductDataTable(): base("Product")
			{
				this.InitClass();
			}
			
			protected ProductDataTable(SerializationInfo info, StreamingContext context): base(info, context)
			{
				this.InitVars();
			}
			
			internal ProductDataTable(DataTable table): base(table.TableName)
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
			
			public DataColumn ProductIdColumn
			{
				get
				{
					return _columnProductId;
				}
			}
			
			public DataColumn CategoryIdColumn
			{
				get
				{
					return _columnCategoryId;
				}
			}
			
			public DataColumn NameColumn
			{
				get
				{
					return _columnName;
				}
			}
			
			public DataColumn DescnColumn
			{
				get
				{
					return _columnDescn;
				}
			}
			
			public DataColumn ImageColumn
			{
				get
				{
					return _columnImage;
				}
			}
			
			public ProductRow this[int index]
			{
				get
				{
					return ((ProductRow)(this.Rows[index]));
				}
			}
			
			public event ProductRowChangeEventHandler ProductRowChanged;
			public event ProductRowChangeEventHandler ProductRowChanging;
			public event ProductRowChangeEventHandler ProductRowDeleted;
			public event ProductRowChangeEventHandler ProductRowDeleting;
			
			public void AddProductRow(ProductRow row)
			{
				this.Rows.Add(row);
			}
			
			public ProductRow AddProductRow(
					string productId,
					string categoryId,
					string name,
					string descn,
					string image
				)
			{
				ProductRow rowProductRow = ((ProductRow)(this.NewRow()));
				rowProductRow["ProductId"] = productId;
				rowProductRow["CategoryId"] = categoryId;
				rowProductRow["Name"] = name;
				rowProductRow["Descn"] = descn;
				rowProductRow["Image"] = image;
				this.Rows.Add(rowProductRow);
				return rowProductRow;
			}
			
			public ProductRow FindByProductId(string productId)
			{
				return ((ProductRow)(this.Rows.Find(new object[] {productId})));
			}
			
			public IEnumerator GetEnumerator()
			{
				return this.Rows.GetEnumerator();
			}
			
			public override DataTable Clone()
			{
				ProductDataTable cln = ((ProductDataTable)(base.Clone()));
				cln.InitVars();
				return cln;
			}
			
			internal void InitVars()
			{
				_columnProductId = this.Columns["ProductId"];
				_columnCategoryId = this.Columns["CategoryId"];
				_columnName = this.Columns["Name"];
				_columnDescn = this.Columns["Descn"];
				_columnImage = this.Columns["Image"];
			}
			
			public void InitClass()
			{
				_columnProductId = new DataColumn("ProductId", typeof(string), "", MappingType.Element);
				_columnProductId.AllowDBNull = false;
				_columnProductId.Caption = "Product Id";
				_columnProductId.MaxLength = 10;
				_columnProductId.Unique = true;
				_columnProductId.DefaultValue = Convert.DBNull;
				_columnProductId.ExtendedProperties.Add("IsKey", "true");
				_columnProductId.ExtendedProperties.Add("ReadOnly", "false");
				_columnProductId.ExtendedProperties.Add("Description", "Product Id");
				_columnProductId.ExtendedProperties.Add("Length", "10");
				_columnProductId.ExtendedProperties.Add("Decimals", "0");
				_columnProductId.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnProductId);
				
				_columnCategoryId = new DataColumn("CategoryId", typeof(string), "", MappingType.Element);
				_columnCategoryId.AllowDBNull = false;
				_columnCategoryId.Caption = "Category Id";
				_columnCategoryId.MaxLength = 10;
				_columnCategoryId.Unique = false;
				_columnCategoryId.DefaultValue = Convert.DBNull;
				_columnCategoryId.ExtendedProperties.Add("IsKey", "false");
				_columnCategoryId.ExtendedProperties.Add("ReadOnly", "false");
				_columnCategoryId.ExtendedProperties.Add("Description", "Category Id");
				_columnCategoryId.ExtendedProperties.Add("Length", "10");
				_columnCategoryId.ExtendedProperties.Add("Decimals", "0");
				_columnCategoryId.ExtendedProperties.Add("AllowDBNulls", "false");
				this.Columns.Add(_columnCategoryId);
				
				_columnName = new DataColumn("Name", typeof(string), "", MappingType.Element);
				_columnName.AllowDBNull = true;
				_columnName.Caption = "Name";
				_columnName.MaxLength = 80;
				_columnName.Unique = false;
				_columnName.DefaultValue = Convert.DBNull;
				_columnName.ExtendedProperties.Add("IsKey", "false");
				_columnName.ExtendedProperties.Add("ReadOnly", "false");
				_columnName.ExtendedProperties.Add("Description", "Name");
				_columnName.ExtendedProperties.Add("Length", "80");
				_columnName.ExtendedProperties.Add("Decimals", "0");
				_columnName.ExtendedProperties.Add("AllowDBNulls", "true");
				this.Columns.Add(_columnName);
				
				_columnDescn = new DataColumn("Descn", typeof(string), "", MappingType.Element);
				_columnDescn.AllowDBNull = true;
				_columnDescn.Caption = "Descn";
				_columnDescn.MaxLength = 255;
				_columnDescn.Unique = false;
				_columnDescn.DefaultValue = Convert.DBNull;
				_columnDescn.ExtendedProperties.Add("IsKey", "false");
				_columnDescn.ExtendedProperties.Add("ReadOnly", "false");
				_columnDescn.ExtendedProperties.Add("Description", "Descn");
				_columnDescn.ExtendedProperties.Add("Length", "255");
				_columnDescn.ExtendedProperties.Add("Decimals", "0");
				_columnDescn.ExtendedProperties.Add("AllowDBNulls", "true");
				this.Columns.Add(_columnDescn);
				
				_columnImage = new DataColumn("Image", typeof(string), "", MappingType.Element);
				_columnImage.AllowDBNull = true;
				_columnImage.Caption = "Image";
				_columnImage.MaxLength = 80;
				_columnImage.Unique = false;
				_columnImage.DefaultValue = Convert.DBNull;
				_columnImage.ExtendedProperties.Add("IsKey", "false");
				_columnImage.ExtendedProperties.Add("ReadOnly", "false");
				_columnImage.ExtendedProperties.Add("Description", "Image");
				_columnImage.ExtendedProperties.Add("Length", "80");
				_columnImage.ExtendedProperties.Add("Decimals", "0");
				_columnImage.ExtendedProperties.Add("AllowDBNulls", "true");
				this.Columns.Add(_columnImage);
				
				this.PrimaryKey = new DataColumn[] {_columnProductId};
			}
			
			public ProductRow NewProductRow()
			{
				ProductRow rowProductRow = ((ProductRow)(this.NewRow()));
				return rowProductRow;
			}
			
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new ProductRow(builder);
			}
			
			protected override Type GetRowType()
			{
				return typeof(ProductRow);
			}
			
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.ProductRowChanged != null)
				{
					this.ProductRowChanged(this, new ProductRowChangeEventArgs(((ProductRow)(e.Row)), e.Action));
				}
			}
			
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.ProductRowChanging != null)
				{
					this.ProductRowChanging(this, new ProductRowChangeEventArgs(((ProductRow)(e.Row)), e.Action));
				}
			}
			
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.ProductRowDeleted != null)
				{
					this.ProductRowDeleted(this, new ProductRowChangeEventArgs(((ProductRow)(e.Row)), e.Action));
				}
			}
			
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.ProductRowDeleting != null)
				{
					this.ProductRowDeleting(this, new ProductRowChangeEventArgs(((ProductRow)(e.Row)), e.Action));
				}
			}
			
			public void RemoveProductRow(ProductRow row)
			{
				this.Rows.Remove(row);
			}
		}
		
		public class ProductRow: DataRow
		{
			private ProductDataTable _tableProduct;
			
			internal ProductRow(DataRowBuilder rb): base(rb)
			{
				_tableProduct = ((ProductDataTable)(this.Table));
			}
			
			/// <summary>
			/// Gets or sets the value of ProductId property
			/// </summary>
			public string ProductId
			{
				get
				{
					try
					{
						return ((string)(this[_tableProduct.ProductIdColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value ProductId because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableProduct.ProductIdColumn] = value;
				}
			}
			
			public bool IsProductIdNull()
			{
				return this.IsNull(_tableProduct.ProductIdColumn);
			}
			
			public void SetProductIdNull()
			{
				this[_tableProduct.ProductIdColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of CategoryId property
			/// </summary>
			public string CategoryId
			{
				get
				{
					try
					{
						return ((string)(this[_tableProduct.CategoryIdColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value CategoryId because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableProduct.CategoryIdColumn] = value;
				}
			}
			
			public bool IsCategoryIdNull()
			{
				return this.IsNull(_tableProduct.CategoryIdColumn);
			}
			
			public void SetCategoryIdNull()
			{
				this[_tableProduct.CategoryIdColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of Name property
			/// </summary>
			public string Name
			{
				get
				{
					try
					{
						return ((string)(this[_tableProduct.NameColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value Name because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableProduct.NameColumn] = value;
				}
			}
			
			public bool IsNameNull()
			{
				return this.IsNull(_tableProduct.NameColumn);
			}
			
			public void SetNameNull()
			{
				this[_tableProduct.NameColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of Descn property
			/// </summary>
			public string Descn
			{
				get
				{
					try
					{
						return ((string)(this[_tableProduct.DescnColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value Descn because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableProduct.DescnColumn] = value;
				}
			}
			
			public bool IsDescnNull()
			{
				return this.IsNull(_tableProduct.DescnColumn);
			}
			
			public void SetDescnNull()
			{
				this[_tableProduct.DescnColumn] = Convert.DBNull;
			}
			
			/// <summary>
			/// Gets or sets the value of Image property
			/// </summary>
			public string Image
			{
				get
				{
					try
					{
						return ((string)(this[_tableProduct.ImageColumn]));
					}
					catch (InvalidCastException exception)
					{
						throw new StrongTypingException("Cannot get value Image because it is DBNull.", exception);
					}
				}
				set
				{
					this[_tableProduct.ImageColumn] = value;
				}
			}
			
			public bool IsImageNull()
			{
				return this.IsNull(_tableProduct.ImageColumn);
			}
			
			public void SetImageNull()
			{
				this[_tableProduct.ImageColumn] = Convert.DBNull;
			}
			
		}
		
		public class ProductRowChangeEventArgs: EventArgs
		{
			private ProductRow _eventRow;
			private System.Data.DataRowAction _eventAction;
			
			public ProductRowChangeEventArgs(ProductRow row, DataRowAction action)
			{
				_eventRow = row;
				_eventAction = action;
			}
			
			public ProductRow Row
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
	
	#region ProductDataAdapter
	public class ProductDataAdapter: MarshalByRefObject, IDataAdapter
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
		public ProductDataAdapter()
		{
			_connectionStringKey = "ConnectionString";
		}
		
		public ProductDataAdapter(string connectionStringKey)
		{
			_connectionStringKey = connectionStringKey + "_ConnectionString";
		}
		
		public ProductDataAdapter(IDbConnection connection)
		{
			this.Connection = connection;
		}
		
		public ProductDataAdapter(IDbTransaction transaction)
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
		
		public int Fill(ProductDataSet dataSet, IDataRecord dataRecord)
		{
			return Fill(dataSet, ((string)(dataRecord["ProductId"])));
		}
		
		public int Fill(ProductDataSet dataSet, DataRow dataRecord)
		{
			return Fill(dataSet, ((string)(dataRecord["ProductId"])));
		}
		
		public int Fill(ProductDataSet dataSet, string productId)
		{
			try
			{
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
					WHERE
						[ProductId] = @ProductId
					";
				_command.Parameters.Add(this.CreateParameter("@ProductId", DbType.AnsiString, productId));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
				if (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					dataSet.AcceptChanges();
					
					return 1;
				}
				else
				{
					throw new ProductNotFoundException();
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
		
		private void PopulateProductDataRow(IDataReader reader, ProductDataSet.ProductRow row)
		{
			if (!reader.IsDBNull(0)) row.ProductId = reader.GetString(0);
			if (!reader.IsDBNull(1)) row.CategoryId = reader.GetString(1);
			if (!reader.IsDBNull(2)) row.Name = reader.GetString(2);
			if (!reader.IsDBNull(3)) row.Descn = reader.GetString(3);
			if (!reader.IsDBNull(4)) row.Image = reader.GetString(4);
		}
		
		public int Fill(DataSet dataSet)
		{
			ProductDataSet pageDataSet = dataSet as ProductDataSet;
			if (pageDataSet != null)
			{
				return this.Fill(pageDataSet);
			}
			else
			{
				throw new ApplicationException();
			}
		}
		
		public int Fill(ProductDataSet dataSet, string[] columns, string[] values, DbType[] types)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
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
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
		
		public int Fill(ProductDataSet dataSet)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]";
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
		public int FillByProductId(ProductDataSet dataSet, string productId)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
					WHERE
						[ProductId] = @ProductId
						";
				
				_command.Parameters.Add(this.CreateParameter("@ProductId", DbType.AnsiString, productId));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
		public int FillByName(ProductDataSet dataSet, string name)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
					WHERE
						[Name] = @Name
						";
				
				_command.Parameters.Add(this.CreateParameter("@Name", DbType.AnsiString, name));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
		public int FillByCategoryId(ProductDataSet dataSet, string categoryId)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
					WHERE
						[CategoryId] = @CategoryId
						";
				
				_command.Parameters.Add(this.CreateParameter("@CategoryId", DbType.AnsiString, categoryId));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
		public int FillByCategoryIdAndName(ProductDataSet dataSet, string categoryId, string name)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
					WHERE
						[CategoryId] = @CategoryId
						AND [Name] = @Name
						";
				
				_command.Parameters.Add(this.CreateParameter("@CategoryId", DbType.AnsiString, categoryId));
				_command.Parameters.Add(this.CreateParameter("@Name", DbType.AnsiString, name));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
		public int FillByCategoryIdAndProductIdAndName(ProductDataSet dataSet, string categoryId, string productId, string name)
		{
			try
			{
				int recordcount = 0;
				_command = this.GetCommand();
				_command.CommandText = @"
					SELECT
						[ProductId],
						[CategoryId],
						[Name],
						[Descn],
						[Image]
					FROM
						[Product]
					WHERE
						[CategoryId] = @CategoryId
						AND [ProductId] = @ProductId
						AND [Name] = @Name
						";
				
				_command.Parameters.Add(this.CreateParameter("@CategoryId", DbType.AnsiString, categoryId));
				_command.Parameters.Add(this.CreateParameter("@ProductId", DbType.AnsiString, productId));
				_command.Parameters.Add(this.CreateParameter("@Name", DbType.AnsiString, name));
				this.OpenConnection();
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
				while (_reader.Read())
				{
					ProductDataSet.ProductRow row = dataSet.Product.NewProductRow();
					this.PopulateProductDataRow(_reader, row);
					dataSet.Product.AddProductRow(row);
					
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
			// not sure if I should create a ProductId parameter here or not.
			return null; //_fillDataParameters;
		}
		#endregion
		
		#region Update Methods
		public int Update(DataSet dataSet)
		{
			ProductDataSet pageDataSet = dataSet as ProductDataSet;
			if (pageDataSet != null)
			{
				return this.Update(pageDataSet);
			}
			else
			{
				throw new ApplicationException();
			}
		}
		
		public int Update(ProductDataSet dataSet)
		{
			if (dataSet != null)
			{
				try
				{
					int updatedRowCount = 0;
					
					foreach(ProductDataSet.ProductRow row in dataSet.Product)
					{
						switch (row.RowState)
						{
							case DataRowState.Added:
							{
								OnProductUpdating(new ProductEventArgs(row, StatementType.Insert)) ;
								_command = this.GetCommand();
								_command.CommandText = @"
									INSERT INTO [Product] (
										[ProductId],
										[CategoryId],
										[Name],
										[Descn],
										[Image]
									) VALUES (
										@ProductId,
										@CategoryId,
										@Name,
										@Descn,
										@Image
									)";
								_command.Parameters.Add(this.CreateParameter("@ProductId", DbType.AnsiString, row.IsProductIdNull() ? (object)DBNull.Value : (object)row.ProductId));
								_command.Parameters.Add(this.CreateParameter("@CategoryId", DbType.AnsiString, row.IsCategoryIdNull() ? (object)DBNull.Value : (object)row.CategoryId));
								_command.Parameters.Add(this.CreateParameter("@Name", DbType.AnsiString, row.IsNameNull() ? (object)DBNull.Value : (object)row.Name));
								_command.Parameters.Add(this.CreateParameter("@Descn", DbType.AnsiString, row.IsDescnNull() ? (object)DBNull.Value : (object)row.Descn));
								_command.Parameters.Add(this.CreateParameter("@Image", DbType.AnsiString, row.IsImageNull() ? (object)DBNull.Value : (object)row.Image));
								this.OpenConnection();
								_command.ExecuteNonQuery();
								OnProductUpdated(new ProductEventArgs(row, StatementType.Insert)) ;
								
								updatedRowCount++;
								break;
							}
							case DataRowState.Modified:
							{
								OnProductUpdating(new ProductEventArgs(row, StatementType.Update)) ;
								_command = this.GetCommand();
								_command.CommandText = @"
									UPDATE [Product] SET
										[CategoryId] = @CategoryId,
										[Name] = @Name,
										[Descn] = @Descn,
										[Image] = @Image
									WHERE
										[ProductId] = @ProductId
									";
								_command.Parameters.Add(this.CreateParameter("@ProductId", DbType.AnsiString, row.IsProductIdNull() ? (object)DBNull.Value : (object)row.ProductId));
								_command.Parameters.Add(this.CreateParameter("@CategoryId", DbType.AnsiString, row.IsCategoryIdNull() ? (object)DBNull.Value : (object)row.CategoryId));
								_command.Parameters.Add(this.CreateParameter("@Name", DbType.AnsiString, row.IsNameNull() ? (object)DBNull.Value : (object)row.Name));
								_command.Parameters.Add(this.CreateParameter("@Descn", DbType.AnsiString, row.IsDescnNull() ? (object)DBNull.Value : (object)row.Descn));
								_command.Parameters.Add(this.CreateParameter("@Image", DbType.AnsiString, row.IsImageNull() ? (object)DBNull.Value : (object)row.Image));
								this.OpenConnection();
								_command.ExecuteNonQuery();
								OnProductUpdated(new ProductEventArgs(row, StatementType.Update)) ;
								
								updatedRowCount++;
								break;
							}
							case DataRowState.Deleted:
							{
								OnProductUpdating(new ProductEventArgs(row, StatementType.Delete)) ;
								_command = this.GetCommand();
								_command.CommandText = @"
									DELETE FROM [Product]
									WHERE
										[ProductId] = @ProductId
									";
								_command.Parameters.Add(this.CreateParameter("@ProductId", DbType.AnsiString, row[dataSet.Product.ProductIdColumn, DataRowVersion.Original]));
								this.OpenConnection();
								_command.ExecuteNonQuery();
								OnProductUpdated(new ProductEventArgs(row, StatementType.Delete)) ;
								
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
		public delegate void ProductUpdateEventHandler(object sender, ProductEventArgs e);
		
		public event ProductUpdateEventHandler ProductUpdated;
		private void OnProductUpdated(ProductEventArgs e)
		{
			if ((this.ProductUpdated != null))
			{
				this.ProductUpdated(this, e);
			}
		}
		
		public event ProductUpdateEventHandler ProductUpdating;
		private void OnProductUpdating(ProductEventArgs e)
		{
			if ((this.ProductUpdating != null))
			{
				this.ProductUpdating(this, e);
			}
		}
		
		public class ProductEventArgs : EventArgs
		{
			private StatementType _statementType;
			private ProductDataSet.ProductRow _dataRow;
			
			public ProductEventArgs(ProductDataSet.ProductRow row, StatementType statementType)
			{
				_dataRow = row;
				_statementType = statementType;
			}
			
			public StatementType StatementType
			{
				get {return _statementType;}

			}
			
			public ProductDataSet.ProductRow Row
			{
				get {return _dataRow;}
				set	{_dataRow = value;}
			}
		}
		#endregion
		
		#region Custom Exceptions
		[Serializable()]
		public class ProductNotFoundException: ApplicationException
		{
			public ProductNotFoundException()
			{
			}
			
			public ProductNotFoundException(string message) : base(message)
			{
			}
			
			public ProductNotFoundException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected ProductNotFoundException(SerializationInfo info, StreamingContext context): base(info, context)
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
		public class ProductDataLockedException: ApplicationException
		{
			public ProductDataLockedException()
			{
			}
			
			public ProductDataLockedException(string message): base(message)
			{
			}
			
			public ProductDataLockedException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected ProductDataLockedException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}

		[Serializable()]
		public class ProductDataChangedException: ApplicationException
		{
			public ProductDataChangedException()
			{
			}
			
			public ProductDataChangedException(string message): base(message)
			{
			}
			
			public ProductDataChangedException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected ProductDataChangedException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		
		[Serializable()]
		public class ProductDuplicateKeyException: ApplicationException
		{
			public ProductDuplicateKeyException()
			{
			}
			
			public ProductDuplicateKeyException(string message): base(message)
			{
			}
			
			public ProductDuplicateKeyException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected ProductDuplicateKeyException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		
		[Serializable()]
		public class ProductDataDeletedException: ApplicationException
		{
			public ProductDataDeletedException()
			{
			}
			
			public ProductDataDeletedException(string message) : base(message)
			{
			}
			
			public ProductDataDeletedException(string message, Exception inner): base(message, inner)
			{
			}
			
			protected ProductDataDeletedException(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}
		}
		#endregion
	}
	#endregion
}
