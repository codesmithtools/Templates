'------------------------------------------------------------------------------ 
' 
' Copyright (c) 2002-2008 CodeSmith Tools, LLC. All rights reserved. 
' 
' The terms of use for this software are contained in the file 
' named sourcelicense.txt, which can be found in the root of this distribution. 
' By using this software in any fashion, you are agreeing to be bound by the 
' terms of this license. 
' 
' You must not remove this notice, or any other, from this software. 
' 
'------------------------------------------------------------------------------ 

Imports System
Imports System.Data
Imports System.Data.Common
Imports System.Collections

Namespace TypedDataSetTester
    Public Class EntryPoint
        <STAThread()> _
        Public Shared Sub Main(ByVal args As String())
            ' PetShop 
            Dim cds As New OrdersDataSet()
            Dim cda As New OrdersDataAdapter()
            Dim pds As New ProductDataSet()
            Dim pda As New ProductDataAdapter()

            pda.FillByCategoryId(pds, "BIRDS")
            Console.Out.WriteLine(cds.Orders.Rows.Count)
            Dim en As IEnumerator = pds.Product.GetEnumerator()
            While en.MoveNext()
                Dim row As ProductDataSet.ProductRow = DirectCast(en.Current, ProductDataSet.ProductRow)
                Console.Out.WriteLine(row.ProductId + " " + row.CategoryId + " " + row.Name + " " + row.Descn)
            End While

            Console.In.ReadLine()

            'fill all the records from the permission table. 
            Dim columns As String() = {"ShipCity", "Courier"}
            Dim values As String() = {"Nowhere", "Me"}
            Dim types As DbType() = {DbType.AnsiString, DbType.AnsiString}
            cda.Fill(cds, columns, values, types)

            Console.Out.WriteLine(cds.Orders.Rows.Count)
            Console.Out.WriteLine(If(cds.Orders.FindByOrderId(1) IsNot Nothing, cds.Orders.FindByOrderId(1).OrderId.ToString(), "Nope"))
            If cds.Orders.FindByOrderId(4) Is Nothing Then
                Console.Out.WriteLine("Nope")
            Else
                Console.Out.WriteLine(cds.Orders.FindByOrderId(4).OrderId.ToString())
            End If
            Console.In.ReadLine()
            cds.Clear()
            cda.Fill(cds)

            Dim newRow As OrdersDataSet.OrdersRow = cds.Orders.NewOrdersRow()
            newRow.OrderDate = DateTime.Now
            newRow.ShipAddr1 = "1002 Nowhere"
            newRow.ShipCity = "Nowhere"
            newRow.ShipState = "Tx"
            newRow.ShipZip = "12345"
            newRow.UserId = "joe"
            newRow.ShipCountry = "USA"
            newRow.BillAddr1 = "2001 UHUH"
            newRow.BillCity = "Nowhere"
            newRow.BillState = "Tx"
            newRow.BillZip = "12345"
            newRow.UserId = "yoyDu"
            newRow.BillCountry = "USA"
            newRow.Courier = "Me"
            newRow.TotalPrice = 12.12D
            newRow.BillToFirstName = "Yaba"
            newRow.BillToLastName = "Daba"
            newRow.ShipToFirstName = "Yoko"
            newRow.ShipToLastName = "Ono"
            newRow.AuthorizationNumber = 123
            newRow.Locale = "Here"
            cds.Orders.AddOrdersRow(newRow)

            'Console.In.Read()
            '' make some changes and save 
            'Dim editRow As OrdersDataSet.OrdersRow = cds.Orders.FindByOrderId(19)
            'editRow.BillZip = "11111"
            'editRow.Courier = "Fedex"
            cda.Update(cds)

            Console.In.ReadLine()
            ' Delete row and update 
            'Dim deleteRow As OrdersDataSet.OrdersRow = cds.Orders.FindByOrderId(22)
            'deleteRow.Delete()
            'cda.Update(cds)
            'Console.In.ReadLine()

        End Sub
    End Class
End Namespace
