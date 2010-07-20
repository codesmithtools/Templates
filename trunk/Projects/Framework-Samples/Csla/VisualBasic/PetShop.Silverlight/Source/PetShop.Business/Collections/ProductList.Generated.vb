﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated using CodeSmith: v5.2.2, CSLA Templates: v3.0.0.1845, CSLA Framework: v4.0.0.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'Product.vb.
'
'     Template: DynamicRootList.Generated.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports System

Imports Csla
#If SILVERLIGHT Then
Imports Csla.Serialization
#Else
Imports Csla.Data
#End If

Namespace PetShop.Business
    <Serializable()> _
    Public Partial Class ProductList
        Inherits EditableRootListBase(Of Product)
    
#Region "Constructor(s)"
        
    #If Not SILVERLIGHT Then
        Private Sub New()
            AllowNew = True
        End Sub
    #Else
        public Sub New()
            AllowNew = True
        End Sub
    #End If
    
#End Region
    
#If Not SILVERLIGHT Then
#Region "Method Overrides"
        
        Protected Overrides Function AddNewCore() As Product
            Dim item As Product = PetShop.Business.Product.NewProduct()
    
            Dim cancel As Boolean = False
            OnAddNewCore(item, cancel)
            If Not (cancel) Then
                ' Check to see if someone set the item to null in the OnAddNewCore.
                If(item Is Nothing) Then
                    item = PetShop.Business.Product.NewProduct()
                End If
                
                Add(item)
            End If
    
            Return item
        End Function
        
#End Region
    
#Region "Synchronous Factory Methods"
        
        Public Shared Function NewList() As ProductList
            Return DataPortal.Create(Of ProductList)()
        End Function

        Public Shared Function GetAll() As ProductList
            Return DataPortal.Fetch(Of ProductList)(New ProductCriteria())
        End Function

    
        Public Shared Function GetByProductId(ByVal productId As System.String) As ProductList 
            Dim criteria As New ProductCriteria()
            criteria.ProductId = productId

            Return DataPortal.Fetch(Of ProductList)(criteria)
        End Function
    
        Public Shared Function GetByName(ByVal name As System.String) As ProductList 
            Dim criteria As New ProductCriteria()
            criteria.Name = name

            Return DataPortal.Fetch(Of ProductList)(criteria)
        End Function
    
        Public Shared Function GetByCategoryId(ByVal categoryId As System.String) As ProductList 
            Dim criteria As New ProductCriteria()
            criteria.CategoryId = categoryId

            Return DataPortal.Fetch(Of ProductList)(criteria)
        End Function
    
        Public Shared Function GetByCategoryIdName(ByVal categoryId As System.String, ByVal name As System.String) As ProductList 
            Dim criteria As New ProductCriteria()
            criteria.CategoryId = categoryId
			criteria.Name = name

            Return DataPortal.Fetch(Of ProductList)(criteria)
        End Function
    
        Public Shared Function GetByCategoryIdProductIdName(ByVal categoryId As System.String, ByVal productId As System.String, ByVal name As System.String) As ProductList 
            Dim criteria As New ProductCriteria()
            criteria.CategoryId = categoryId
			criteria.ProductId = productId
			criteria.Name = name

            Return DataPortal.Fetch(Of ProductList)(criteria)
        End Function
    
#End Region

#Else

#Region "Method Overrides"

        Protected Overrides Sub AddNewCore() 
            Dim item As Product = PetShop.Business.Product.NewProduct()
    
            Dim cancel As Boolean = False
            OnAddNewCore(item, cancel)
            If Not (cancel) Then
                ' Check to see if someone set the item to null in the OnAddNewCore.
                If(item Is Nothing) Then
                    item = PetShop.Business.Product.NewProduct()
                End If
                
                Add(item)
            End If
        End Sub

#End Region
#End If 

#Region "Asynchronous Factory Methods"
            
        Public Shared Sub NewListAsync(ByVal handler As EventHandler(Of DataPortalResult(Of ProductList)))
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.CreateCompleted, handler
            dp.BeginCreate()
        End Sub
    
        Public Shared Sub GetByProductIdAsync(ByVal productId As System.String, ByVal handler As EventHandler(Of DataPortalResult(Of ProductList)))
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.FetchCompleted, handler
        
            Dim criteria As New ProductCriteria()
            criteria.ProductId = productId
    
            dp.BeginFetch(criteria)
        End Sub
    
        Public Shared Sub GetByNameAsync(ByVal name As System.String, ByVal handler As EventHandler(Of DataPortalResult(Of ProductList)))
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.FetchCompleted, handler
        
            Dim criteria As New ProductCriteria()
            criteria.Name = name
    
            dp.BeginFetch(criteria)
        End Sub
    
        Public Shared Sub GetByCategoryIdAsync(ByVal categoryId As System.String, ByVal handler As EventHandler(Of DataPortalResult(Of ProductList)))
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.FetchCompleted, handler
        
            Dim criteria As New ProductCriteria()
            criteria.CategoryId = categoryId
    
            dp.BeginFetch(criteria)
        End Sub
    
        Public Shared Sub GetByCategoryIdNameAsync(ByVal categoryId As System.String, ByVal name As System.String, ByVal handler As EventHandler(Of DataPortalResult(Of ProductList)))
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.FetchCompleted, handler
        
            Dim criteria As New ProductCriteria()
            criteria.CategoryId = categoryId
            criteria.Name = name
    
            dp.BeginFetch(criteria)
        End Sub
    
        Public Shared Sub GetByCategoryIdProductIdNameAsync(ByVal categoryId As System.String, ByVal productId As System.String, ByVal name As System.String, ByVal handler As EventHandler(Of DataPortalResult(Of ProductList)))
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.FetchCompleted, handler
        
            Dim criteria As New ProductCriteria()
            criteria.CategoryId = categoryId
            criteria.ProductId = productId
            criteria.Name = name
    
            dp.BeginFetch(criteria)
        End Sub

        Public Shared Function GetAllAsync(ByVal handler As EventHandler(Of DataPortalResult(Of ProductList))) As ProductList
            Dim dp As New DataPortal(Of ProductList)()
            AddHandler dp.FetchCompleted, handler
            dp.BeginFetch(New ProductCriteria())
        End Function

#End Region
    
#Region "DataPortal partial methods"
    
    #If Not SILVERLIGHT Then
        Partial Private Sub OnCreating(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub OnFetching(ByVal criteria As ProductCriteria, ByRef cancel As Boolean)
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
    #End If
        Partial Private Sub OnAddNewCore(ByVal item As Product, ByRef cancel As Boolean)
        End Sub
    
#End Region

#Region "Exists Command"
    
    #If Not SILVERLIGHT Then
        Public Shared Function Exists(ByVal criteria As ProductCriteria) As Boolean
            Return PetShop.Business.Product.Exists(criteria)
        End Function
    #End If
    
#End Region

    End Class
End Namespace