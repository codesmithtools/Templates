'------------------------------------------------------------------------------
'
' Copyright (c) 2002-2009 CodeSmith Tools, LLC.  All rights reserved.
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
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports CodeSmith.CustomProperties

<TypeConverter(GetType(XmlSerializedTypeConverter))> _
<Editor(GetType(XmlSerializedFilePicker), GetType(System.Drawing.Design.UITypeEditor))> _
<XmlRootAttribute("PurchaseOrder", Namespace:="http:'www.codesmithtools.com/po", IsNullable:=False)> _
   Public Class PurchaseOrder
    Public ShipTo As Address
    Public OrderDate As String
    <XmlArrayAttribute("Items")> _
    Public OrderedItems As Array
    Public SubTotal As Decimal
    Public ShipCost As Decimal
    Public TotalCost As Decimal
    Public Overrides Function ToString() As String
        Return "{PO: $" + TotalCost.ToString() + "}"
    End Function
End Class

Public Class Address
    <XmlAttribute()> _
    Public Name As String
    Public Line1 As String
    <XmlElementAttribute(IsNullable:=False)> _
    Public City As String
    Public State As String
    Public Zip As String
End Class

Public Class OrderedItem
    Public ItemName As String
    Public Description As String
    Public UnitPrice As Decimal
    Public Quantity As Integer
    Public LineTotal As Decimal

    Public Sub Calculate()
        LineTotal = UnitPrice * Quantity
    End Sub
End Class
