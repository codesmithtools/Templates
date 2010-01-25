'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated Imports CSLA 3.8.X CodeSmith Templates.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'Inventory.vb.
'
'     Template path: EditableRoot.Generated.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports System

Imports Csla
Imports Csla.Data
Imports Csla.Validation

<Serializable()> _
Public Partial Class Inventory 
    Inherits BusinessBase(Of Inventory)

    #Region "Contructor(s)"

    Private Sub New()
        ' require use of factory method 
    End Sub

    Friend Sub New(Byval reader As SafeDataReader)
        Map(reader)
    End Sub

    #End Region
    
    #Region "Validation Rules"
    
    Protected Overrides Sub AddBusinessRules()
    
        If AddBusinessValidationRules() Then Exit Sub
       
    End Sub
    
    #End Region
    
    #Region "Business Methods"


    
    Private Shared ReadOnly _itemIdProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Inventory) p.ItemId)
		<System.ComponentModel.DataObjectField(true, false)> _
    Public Property ItemId() As String
        Get 
            Return GetProperty(_itemIdProperty)
        End Get
        Set (ByVal value As String)
            SetProperty(_itemIdProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _qtyProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As Inventory) p.Qty)
    Public Property Qty() As Integer
        Get 
            Return GetProperty(_qtyProperty)
        End Get
        Set (ByVal value As Integer)
            SetProperty(_qtyProperty, value)
        End Set
    End Property
    
    #End Region
    
    #Region "Factory Methods"
    
    Public Shared Function NewInventory() As Inventory 
        Return DataPortal.Create(Of Inventory)()
    End Function
    
    Public Shared Function GetInventory(ByVal itemId As String) As Inventory         
        Return DataPortal.Fetch(Of Inventory)(New InventoryCriteria(itemId))
    End Function

    Public Shared Sub DeleteInventory(ByVal itemId As String)
        DataPortal.Delete(New InventoryCriteria(itemId))
    End Sub

    #End Region
    
End Class
