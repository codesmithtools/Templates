using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class OrderStatus : BusinessBase<string>
    {
        #region Declarations

		private int _orderId = default(Int32);
		private int _lineNum = default(Int32);
		private System.DateTime _timestamp = new DateTime();
		private string _status = String.Empty;
		
		
		
		#endregion

        #region Constructors

        public OrderStatus() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_orderId);
			sb.Append(_lineNum);
			sb.Append(_timestamp);
			sb.Append(_status);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public override string Id
		{
			get
			{
				System.Text.StringBuilder uniqueId = new System.Text.StringBuilder();
				uniqueId.Append(_orderId.ToString());
				uniqueId.Append("^");
				uniqueId.Append(_lineNum.ToString());
				return uniqueId.ToString();
			}
		}
		
		public virtual int OrderId
        {
            get { return _orderId; }
			set
			{
				OnOrderIdChanging();
				_orderId = value;
				OnOrderIdChanged();
			}
        }
		partial void OnOrderIdChanging();
		partial void OnOrderIdChanged();
		
		public virtual int LineNum
        {
            get { return _lineNum; }
			set
			{
				OnLineNumChanging();
				_lineNum = value;
				OnLineNumChanged();
			}
        }
		partial void OnLineNumChanging();
		partial void OnLineNumChanged();
		
		public virtual System.DateTime Timestamp
        {
            get { return _timestamp; }
			set
			{
				OnTimestampChanging();
				_timestamp = value;
				OnTimestampChanged();
			}
        }
		partial void OnTimestampChanging();
		partial void OnTimestampChanged();
		
		public virtual string Status
        {
            get { return _status; }
			set
			{
				OnStatusChanging();
				_status = value;
				OnStatusChanged();
			}
        }
		partial void OnStatusChanging();
		partial void OnStatusChanged();
		
        #endregion
    }
}
