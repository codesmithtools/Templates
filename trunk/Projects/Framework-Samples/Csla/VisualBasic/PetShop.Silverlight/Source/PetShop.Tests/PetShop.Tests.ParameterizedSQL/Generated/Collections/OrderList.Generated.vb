﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated using CodeSmith: v5.2.2, CSLA Templates: v2.0.1.1766, CSLA Framework: v3.8.2.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'OrderList.vb.
'
'     Template: EditableRootList.Generated.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports System

Imports Csla
Imports Csla.Data

Namespace PetShop.Tests.ParameterizedSQL
    <Serializable()> _
    Public Partial Class OrderList 
        Inherits BusinessListBase(Of OrderList, Order)
    
        #Region "Contructor(s)"
        
        Private Sub New()
            AllowNew = true
        End Sub
    
        #End Region
    
        #Region "Method Overrides"
    
        Protected Overrides Function AddNewCore() As Object
            Dim item As Order = PetShop.Tests.ParameterizedSQL.Order.NewOrder()
    
            Dim cancel As Boolean = False
            OnAddNewCore(item, cancel)
            If Not (cancel) Then
                ' Check to see if someone set the item to null in the OnAddNewCore.
                If(item Is Nothing) Then
                    item = PetShop.Tests.ParameterizedSQL.Order.NewOrder()
                End If
                Add(item)
            End If
    
            Return item
        End Function
    
        #End Region
    
        #Region "Synchronous Factory Methods"
    
        Public Shared Function NewList() As OrderList
            Return DataPortal.Create(Of OrderList)()
        End Function
    
        Public Shared Function GetByOrderId(ByVal orderId As System.Int32) As OrderList 
            Dim criteria As New OrderCriteria()
            criteria.OrderId = orderId
    
            Return DataPortal.Fetch(Of OrderList)(criteria)
        End Function
    
        Public Shared Function GetAll() As OrderList
            Return DataPortal.Fetch(Of OrderList)(New OrderCriteria())
        End Function
    
        #End Region
    
    
        #Region "Exists Command"
    
        Public Shared Function Exists(ByVal criteria As OrderCriteria) As Boolean
            Return PetShop.Tests.ParameterizedSQL.Order.Exists(criteria)
        End Function
    
        #End Region
    
    
        #Region "DataPortal partial methods"
    
        Partial Private Sub OnCreating(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub OnFetching(ByVal criteria As OrderCriteria, ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnFetched()
        End Sub
        Partial Private Sub OnMapping(ByVal reader As SafeDataReader, ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnMapped()
        End Sub
        Partial Private Sub OnUpdating(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnUpdated()
        End Sub
        Partial Private Sub OnAddNewCore(ByVal item As Order, ByRef cancel As Boolean)
        End Sub
    
        #End Region
    End Class
End Namespace