using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Order : BusinessBase<int>
    {
        #region Declarations

		private string _userId = String.Empty;
		private System.DateTime _orderDate = new DateTime();
		private string _shipAddr1 = String.Empty;
		private string _shipAddr2 = null;
		private string _shipCity = String.Empty;
		private string _shipState = String.Empty;
		private string _shipZip = String.Empty;
		private string _shipCountry = String.Empty;
		private string _billAddr1 = String.Empty;
		private string _billAddr2 = null;
		private string _billCity = String.Empty;
		private string _billState = String.Empty;
		private string _billZip = String.Empty;
		private string _billCountry = String.Empty;
		private string _courier = String.Empty;
		private decimal _totalPrice = default(Decimal);
		private string _billToFirstName = String.Empty;
		private string _billToLastName = String.Empty;
		private string _shipToFirstName = String.Empty;
		private string _shipToLastName = String.Empty;
		private int _authorizationNumber = default(Int32);
		private string _locale = String.Empty;
		
		
		
		#endregion

        #region Constructors

        public Order() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_userId);
			sb.Append(_orderDate);
			sb.Append(_shipAddr1);
			sb.Append(_shipAddr2);
			sb.Append(_shipCity);
			sb.Append(_shipState);
			sb.Append(_shipZip);
			sb.Append(_shipCountry);
			sb.Append(_billAddr1);
			sb.Append(_billAddr2);
			sb.Append(_billCity);
			sb.Append(_billState);
			sb.Append(_billZip);
			sb.Append(_billCountry);
			sb.Append(_courier);
			sb.Append(_totalPrice);
			sb.Append(_billToFirstName);
			sb.Append(_billToLastName);
			sb.Append(_shipToFirstName);
			sb.Append(_shipToLastName);
			sb.Append(_authorizationNumber);
			sb.Append(_locale);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public virtual string UserId
        {
            get { return _userId; }
			set
			{
				OnUserIdChanging();
				_userId = value;
				OnUserIdChanged();
			}
        }
		partial void OnUserIdChanging();
		partial void OnUserIdChanged();
		
		public virtual System.DateTime OrderDate
        {
            get { return _orderDate; }
			set
			{
				OnOrderDateChanging();
				_orderDate = value;
				OnOrderDateChanged();
			}
        }
		partial void OnOrderDateChanging();
		partial void OnOrderDateChanged();
		
		public virtual string ShipAddr1
        {
            get { return _shipAddr1; }
			set
			{
				OnShipAddr1Changing();
				_shipAddr1 = value;
				OnShipAddr1Changed();
			}
        }
		partial void OnShipAddr1Changing();
		partial void OnShipAddr1Changed();
		
		public virtual string ShipAddr2
        {
            get { return _shipAddr2; }
			set
			{
				OnShipAddr2Changing();
				_shipAddr2 = value;
				OnShipAddr2Changed();
			}
        }
		partial void OnShipAddr2Changing();
		partial void OnShipAddr2Changed();
		
		public virtual string ShipCity
        {
            get { return _shipCity; }
			set
			{
				OnShipCityChanging();
				_shipCity = value;
				OnShipCityChanged();
			}
        }
		partial void OnShipCityChanging();
		partial void OnShipCityChanged();
		
		public virtual string ShipState
        {
            get { return _shipState; }
			set
			{
				OnShipStateChanging();
				_shipState = value;
				OnShipStateChanged();
			}
        }
		partial void OnShipStateChanging();
		partial void OnShipStateChanged();
		
		public virtual string ShipZip
        {
            get { return _shipZip; }
			set
			{
				OnShipZipChanging();
				_shipZip = value;
				OnShipZipChanged();
			}
        }
		partial void OnShipZipChanging();
		partial void OnShipZipChanged();
		
		public virtual string ShipCountry
        {
            get { return _shipCountry; }
			set
			{
				OnShipCountryChanging();
				_shipCountry = value;
				OnShipCountryChanged();
			}
        }
		partial void OnShipCountryChanging();
		partial void OnShipCountryChanged();
		
		public virtual string BillAddr1
        {
            get { return _billAddr1; }
			set
			{
				OnBillAddr1Changing();
				_billAddr1 = value;
				OnBillAddr1Changed();
			}
        }
		partial void OnBillAddr1Changing();
		partial void OnBillAddr1Changed();
		
		public virtual string BillAddr2
        {
            get { return _billAddr2; }
			set
			{
				OnBillAddr2Changing();
				_billAddr2 = value;
				OnBillAddr2Changed();
			}
        }
		partial void OnBillAddr2Changing();
		partial void OnBillAddr2Changed();
		
		public virtual string BillCity
        {
            get { return _billCity; }
			set
			{
				OnBillCityChanging();
				_billCity = value;
				OnBillCityChanged();
			}
        }
		partial void OnBillCityChanging();
		partial void OnBillCityChanged();
		
		public virtual string BillState
        {
            get { return _billState; }
			set
			{
				OnBillStateChanging();
				_billState = value;
				OnBillStateChanged();
			}
        }
		partial void OnBillStateChanging();
		partial void OnBillStateChanged();
		
		public virtual string BillZip
        {
            get { return _billZip; }
			set
			{
				OnBillZipChanging();
				_billZip = value;
				OnBillZipChanged();
			}
        }
		partial void OnBillZipChanging();
		partial void OnBillZipChanged();
		
		public virtual string BillCountry
        {
            get { return _billCountry; }
			set
			{
				OnBillCountryChanging();
				_billCountry = value;
				OnBillCountryChanged();
			}
        }
		partial void OnBillCountryChanging();
		partial void OnBillCountryChanged();
		
		public virtual string Courier
        {
            get { return _courier; }
			set
			{
				OnCourierChanging();
				_courier = value;
				OnCourierChanged();
			}
        }
		partial void OnCourierChanging();
		partial void OnCourierChanged();
		
		public virtual decimal TotalPrice
        {
            get { return _totalPrice; }
			set
			{
				OnTotalPriceChanging();
				_totalPrice = value;
				OnTotalPriceChanged();
			}
        }
		partial void OnTotalPriceChanging();
		partial void OnTotalPriceChanged();
		
		public virtual string BillToFirstName
        {
            get { return _billToFirstName; }
			set
			{
				OnBillToFirstNameChanging();
				_billToFirstName = value;
				OnBillToFirstNameChanged();
			}
        }
		partial void OnBillToFirstNameChanging();
		partial void OnBillToFirstNameChanged();
		
		public virtual string BillToLastName
        {
            get { return _billToLastName; }
			set
			{
				OnBillToLastNameChanging();
				_billToLastName = value;
				OnBillToLastNameChanged();
			}
        }
		partial void OnBillToLastNameChanging();
		partial void OnBillToLastNameChanged();
		
		public virtual string ShipToFirstName
        {
            get { return _shipToFirstName; }
			set
			{
				OnShipToFirstNameChanging();
				_shipToFirstName = value;
				OnShipToFirstNameChanged();
			}
        }
		partial void OnShipToFirstNameChanging();
		partial void OnShipToFirstNameChanged();
		
		public virtual string ShipToLastName
        {
            get { return _shipToLastName; }
			set
			{
				OnShipToLastNameChanging();
				_shipToLastName = value;
				OnShipToLastNameChanged();
			}
        }
		partial void OnShipToLastNameChanging();
		partial void OnShipToLastNameChanged();
		
		public virtual int AuthorizationNumber
        {
            get { return _authorizationNumber; }
			set
			{
				OnAuthorizationNumberChanging();
				_authorizationNumber = value;
				OnAuthorizationNumberChanged();
			}
        }
		partial void OnAuthorizationNumberChanging();
		partial void OnAuthorizationNumberChanged();
		
		public virtual string Locale
        {
            get { return _locale; }
			set
			{
				OnLocaleChanging();
				_locale = value;
				OnLocaleChanged();
			}
        }
		partial void OnLocaleChanging();
		partial void OnLocaleChanged();
		
        #endregion
    }
}
