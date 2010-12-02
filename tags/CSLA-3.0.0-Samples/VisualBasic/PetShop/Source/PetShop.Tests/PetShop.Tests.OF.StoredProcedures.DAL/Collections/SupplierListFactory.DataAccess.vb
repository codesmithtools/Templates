﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated using CodeSmith: v5.2.2, CSLA Templates: v3.0.0.0, CSLA Framework: v3.8.4.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'Supplier.cs'.
'
'     Template: ObjectFactoryList.DataAccess.StoredProcedures.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

#Region "Imports declarations"

Imports System
Imports System.Data
Imports System.Data.SqlClient

Imports Csla
Imports Csla.Data
Imports Csla.Server

Imports PetShop.Tests.OF.StoredProcedures

#End Region

Namespace PetShop.Tests.OF.StoredProcedures.DAL
    Public Partial Class SupplierListFactory
        Inherits ObjectFactory
    
#Region "Create"
    
        ''' <summary>
        ''' Creates New SupplierList with default values.
        ''' </summary>
        ''' <Returns>New SupplierList.</Returns>
        <RunLocal()> _
        Public Function Create() As SupplierList
            Dim item As SupplierList = CType(Activator.CreateInstance(GetType(SupplierList), True), SupplierList)
    
            Dim cancel As Boolean = False
            OnCreating(cancel)
            If (cancel) Then
                Return item
            End If
    
            CheckRules(item)
            MarkNew(item)
            MarkAsChild(item)
    
            OnCreated()
    
            Return item
        End Function
    
#End Region
    
#Region "Fetch
    
        ''' <summary>
        ''' Fetch SupplierList.
        ''' </summary>
        ''' <param name="criteria">The criteria.</param>
        ''' <Returns></Returns>
        Public Function Fetch(ByVal criteria As SupplierCriteria) As SupplierList
            Dim item As SupplierList = CType(Activator.CreateInstance(GetType(SupplierList), True), SupplierList)
    
            Dim cancel As Boolean = False
            OnFetching(criteria, cancel)
            If (cancel) Then
                Return item
            End If
    
            ' Fetch Child objects.
            Using connection As New SqlConnection(ADOHelper.ConnectionString)
                connection.Open()
                Using command As New SqlCommand("[dbo].[CSLA_Supplier_Select]", connection)
                    command.CommandType = CommandType.StoredProcedure
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag))
                    command.Parameters.AddWithValue("@p_NameHasValue", criteria.NameHasValue)
					command.Parameters.AddWithValue("@p_Addr1HasValue", criteria.Addr1HasValue)
					command.Parameters.AddWithValue("@p_Addr2HasValue", criteria.Addr2HasValue)
					command.Parameters.AddWithValue("@p_CityHasValue", criteria.CityHasValue)
					command.Parameters.AddWithValue("@p_StateHasValue", criteria.StateHasValue)
					command.Parameters.AddWithValue("@p_ZipHasValue", criteria.ZipHasValue)
					command.Parameters.AddWithValue("@p_PhoneHasValue", criteria.PhoneHasValue)
                    Using reader As SafeDataReader = New SafeDataReader(command.ExecuteReader())
                        If reader.Read() Then
                            Do
                                item.Add(new SupplierFactory().Map(reader))
                            Loop While reader.Read()
                        End If
                    End Using
                End Using
            End Using
    
            MarkOld(item)
            MarkAsChild(item)
    
            OnFetched()
    
            Return item
        End Function
    
#End Region
    
#Region "DataPortal partial methods"
    
        Partial Private Sub OnCreating(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnCreated()
        End Sub
        Partial Private Sub OnFetching(ByVal criteria As SupplierCriteria, ByRef cancel As Boolean)
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
        Partial Private Sub OnAddNewCore(ByVal item As Supplier, ByRef cancel As Boolean)
        End Sub
    
#End Region
    End Class
End Namespace