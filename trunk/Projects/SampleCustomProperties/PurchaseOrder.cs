//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2008 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;
using System.ComponentModel;
using CodeSmith.CustomProperties;

namespace CodeSmith.SampleCustomProperties
{
	[TypeConverter(typeof(XmlSerializedTypeConverter))]
	[Editor(typeof(CodeSmith.CustomProperties.XmlSerializedFilePicker), typeof(System.Drawing.Design.UITypeEditor))]
	[XmlRootAttribute("PurchaseOrder", Namespace="http://www.codesmithtools.com/po", IsNullable = false)]
	public class PurchaseOrder
	{
		public Address ShipTo;
		public string OrderDate; 
		[XmlArrayAttribute("Items")]
		public OrderedItem[] OrderedItems;
		public decimal SubTotal;
		public decimal ShipCost;
		public decimal TotalCost;
		public override string ToString()
		{
			return "{PO: $" + TotalCost.ToString() + "}";
		}
	}
	
	public class Address
	{
		[XmlAttribute]
		public string Name;
		public string Line1;
		[XmlElementAttribute(IsNullable = false)]
		public string City;
		public string State;
		public string Zip;
	}
	
	public class OrderedItem
	{
		public string ItemName;
		public string Description;
		public decimal UnitPrice;
		public int Quantity;
		public decimal LineTotal;
		
		public void Calculate()
		{
			LineTotal = UnitPrice * Quantity;
		}
	}
}
