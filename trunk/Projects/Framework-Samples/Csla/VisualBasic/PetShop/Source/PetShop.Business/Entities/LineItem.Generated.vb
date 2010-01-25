'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated Imports CSLA 3.8.X CodeSmith Templates.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'LineItem.vb.
'
'     Template: SwitchableObject.Generated.cst
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
Public Partial Class LineItem 
    Inherits BusinessBase(Of LineItem)
    
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
       
        ValidationRules.AddRule(AddressOf CommonRules.StringRequired, _itemIdProperty)
        ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs(_itemIdProperty, 10))
    End Sub
    
    #End Region
    
    #Region "Business Methods"


    
    Private Shared ReadOnly _orderIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As LineItem) p.OrderId)
		<System.ComponentModel.DataObjectField(true, false)> _
    Public Property OrderId() As Integer
        Get 
            Return GetProperty(_orderIdProperty)
        End Get
        Set (ByVal value As Integer)
            SetProperty(_orderIdProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _lineNumProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As LineItem) p.LineNum)
		<System.ComponentModel.DataObjectField(true, false)> _
    Public Property LineNum() As Integer
        Get 
            Return GetProperty(_lineNumProperty)
        End Get
        Set (ByVal value As Integer)
            SetProperty(_lineNumProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _itemIdProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As LineItem) p.ItemId)
    Public Property ItemId() As String
        Get 
            Return GetProperty(_itemIdProperty)
        End Get
        Set (ByVal value As String)
            SetProperty(_itemIdProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _quantityProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As LineItem) p.Quantity)
    Public Property Quantity() As Integer
        Get 
            Return GetProperty(_quantityProperty)
        End Get
        Set (ByVal value As Integer)
            SetProperty(_quantityProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _unitPriceProperty As PropertyInfo(Of Decimal) = RegisterProperty(Of Decimal)(Function(p As LineItem) p.UnitPrice)
    Public Property UnitPrice() As Decimal
        Get 
            Return GetProperty(_unitPriceProperty)
        End Get
        Set (ByVal value As Decimal)
            SetProperty(_unitPriceProperty, value)
        End Set
    End Property
    
    Private Shared ReadOnly _orderProperty As PropertyInfo(Of Order) = RegisterProperty(Of Order)(Function(p As LineItem) p.Order, Csla.RelationshipTypes.LazyLoad)
    Public ReadOnly Property Order() As Order
        Get
        
            If Not(FieldManager.FieldExists(_orderProperty))
                If (Me.IsNew) Then
                    LoadProperty(_orderProperty, Order.NewOrder())
                Else
                    LoadProperty(_orderProperty, Order.GetOrder(OrderId))
                End If
            End If
            
            Return GetProperty(_orderProperty) 
        End Get
    End Property
    
    #End Region
            
    #Region "Root Factory Methods"
    
    Public Shared Function NewLineItem() As LineItem 
        Return DataPortal.Create(Of LineItem)()
    End Function
    
    Public Shared Function GetLineItem(ByVal orderId As Integer, ByVal lineNum As Integer) As LineItem         
        Return DataPortal.Fetch(Of LineItem)(New LineItemCriteria(orderId, lineNum))
    End Function

    Public Shared Function GetLineItem(ByVal orderId As Integer) As LineItem 
        Dim criteria As New LineItemCriteria()
        criteria.OrderId = orderId
        
        Return DataPortal.Fetch(Of LineItem)(criteria)
    End Function

    Public Shared Sub DeleteLineItem(ByVal orderId As Integer, ByVal lineNum As Integer)
        DataPortal.Delete(New LineItemCriteria(orderId, lineNum))
    End Sub
    
    #End Region
    
    #Region "Child Factory Methods"
    
    Friend Shared Function NewLineItemChild() As LineItem
        Return DataPortal.CreateChild(Of LineItem)()
    End Function
    
    Friend Shared Function GetLineItemChild(ByVal orderId As Integer, ByVal lineNum As Integer) As LineItem         
        Return DataPortal.FetchChild(Of LineItem)(New LineItemCriteria(orderId, lineNum))
    End Function

    Friend Shared Function GetLineItemChild(ByVal orderId As Integer) As LineItem 
        Dim criteria As New LineItemCriteria()
        criteria.OrderId = orderId
        
        Return DataPortal.FetchChild(Of LineItem)(criteria)
    End Function

    #End Region
    
    #Region "Protected Overriden Method(s)"
    
    ' NOTE: This is needed for Composite Keys. 
    Private ReadOnly _guidID As Guid = Guid.NewGuid()
    Protected Overrides Function GetIdValue() As Object
        Return _guidID
    End Function
    
    #End Region

End Class