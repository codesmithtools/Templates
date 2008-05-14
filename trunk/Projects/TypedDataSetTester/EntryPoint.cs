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

namespace TypedDataSetTester
{
	public class EntryPoint
	{
		[STAThread]
		static void Main(string[] args)
		{
			// Northwind
            OrdersDataSet cds = new OrdersDataSet();
            OrdersDataAdapter cda = new OrdersDataAdapter("ConnectionString");
			
			// fill all the records from the permission table.
			cda.Fill(cds);
			
			// make some changes and save
			
			// reset changes and save
			
		}
	}
}
