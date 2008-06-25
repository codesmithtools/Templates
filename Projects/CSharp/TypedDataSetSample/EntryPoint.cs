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
using System.Data;
using System.Data.Common;
using System.Collections;

namespace TypedDataSetSample
{
	public class EntryPoint
	{
		[STAThread]
		static void Main(string[] args)
		{
			// PetShop
            OrdersDataSet cds = new OrdersDataSet();
            OrdersDataAdapter cda = new OrdersDataAdapter();
            ProductDataSet pds = new ProductDataSet();
            ProductDataAdapter pda = new ProductDataAdapter();
            
            pda.FillByCategoryId(pds, "BIRDS");
            Console.Out.WriteLine(cds.Orders.Rows.Count);
            IEnumerator en = pds.Product.GetEnumerator();
            while (en.MoveNext())
            {
                ProductDataSet.ProductRow row = (ProductDataSet.ProductRow)en.Current;
                Console.Out.WriteLine(row.ProductId + " " + row.CategoryId + " " + row.Name + " " + row.Descn);
            }

            // fill all the records from the permission table.
            string[] columns = { "ShipCity", "Courier" };
            string[] values = { "Nowhere", "Me" };
            DbType[] types = { DbType.AnsiString, DbType.AnsiString };
            cda.Fill(cds, columns, values, types);


            Console.Out.WriteLine(cds.Orders.Rows.Count);
            Console.Out.WriteLine(cds.Orders.FindByOrderId(1) != null ? cds.Orders.FindByOrderId(1).OrderId.ToString() : "Nope");
            Console.Out.WriteLine(cds.Orders.FindByOrderId(4) != null ? cds.Orders.FindByOrderId(4).OrderId.ToString() : "Nope");
            Console.In.Read();

            cds.Clear();
            cda.Fill(cds);
            
            OrdersDataSet.OrdersRow newRow = cds.Orders.NewOrdersRow();
            newRow.OrderDate = DateTime.Now;
            newRow.ShipAddr1 = "2001 Nowhere";
            newRow.ShipCity = "Nowhere";
            newRow.ShipState = "Tx";
            newRow.ShipZip = "12345";
            newRow.UserId = "joe";
            newRow.ShipCountry = "USA";
            newRow.BillAddr1 = "2001 UHUH";
            newRow.BillCity = "Nowhere";
            newRow.BillState = "Tx";
            newRow.BillZip = "12345";
            newRow.UserId = "yoyDu";
            newRow.BillCountry = "USA";
            newRow.Courier = "Me";
            newRow.TotalPrice = 12.12M;
            newRow.BillToFirstName = "Yaba";
            newRow.BillToLastName = "Daba";
            newRow.ShipToFirstName = "Yoko";
            newRow.ShipToLastName = "Ono";
            newRow.AuthorizationNumber = 123;
            newRow.Locale = "Here";
            cds.Orders.AddOrdersRow(newRow);
                
            Console.In.Read();
			// make some changes and save
            //OrdersDataSet.OrdersRow editRow = cds.Orders.FindByOrderId(19);
            //editRow.BillZip = "33333";
            //editRow.Courier = "USPS";
            cda.Update(cds);

            //Console.In.Read();
            //// reset changes and save
            

            //Console.In.Read();
            //// Delete row and update
            //OrdersDataSet.OrdersRow deleteRow = cds.Orders.FindByOrderId(20);
            //deleteRow.Delete();
            //cda.Update(cds);
            //Console.In.Read();
			
		}
	}
}
