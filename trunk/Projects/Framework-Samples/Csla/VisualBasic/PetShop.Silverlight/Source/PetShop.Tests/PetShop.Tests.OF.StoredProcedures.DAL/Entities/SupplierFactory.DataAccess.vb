﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated using CodeSmith: v5.2.2, CSLA Templates: v2.0.1.1766, CSLA Framework: v3.8.2.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'Supplier.cs'.
'
'     Template: ObjectFactory.DataAccess.StoredProcedures.cst
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
    Public Partial Class SupplierFactory
        Inherits ObjectFactory
    
        #Region "Create"
    
        ''' <summary>
        ''' Creates New Supplier with default values.
        ''' </summary>
        ''' <Returns>New Supplier.</Returns>
        <RunLocal()> _
        Public Function Create() As Supplier
            Dim item As Supplier = CType(Activator.CreateInstance(GetType(Supplier), True), Supplier)
    
            Dim cancel As Boolean = False
            OnCreating(cancel)
            If (cancel) Then
                Return item
            End If
    
            Using BypassPropertyChecks(item)
                ' Default values.
    
    
                CheckRules(item)
                MarkNew(item)
            End Using
    
            OnCreated()
    
            Return item
        End Function
    
        ''' <summary>
        ''' Creates New Supplier with default values.
        ''' </summary>
        ''' <Returns>New Supplier.</Returns>
        <RunLocal()> _
        Private Function Create(ByVal criteria As SupplierCriteria) As  Supplier
            Dim item As Supplier = CType(Activator.CreateInstance(GetType(Supplier), True), Supplier)
    
            Dim cancel As Boolean = False
            OnCreating(cancel)
            If (cancel) Then
                Return item
            End If
    
            Dim resource As Supplier = Fetch(criteria)
    
            Using BypassPropertyChecks(item)
                item.Name = resource.Name
                item.Status = resource.Status
                item.Addr1 = resource.Addr1
                item.Addr2 = resource.Addr2
                item.City = resource.City
                item.State = resource.State
                item.Zip = resource.Zip
                item.Phone = resource.Phone
            End Using
    
            CheckRules(item)
            MarkNew(item)
    
            OnCreated()
    
            Return item
        End Function
    
        #End Region
    
        #Region "Fetch"
    
        ''' <summary>
        ''' Fetch Supplier.
        ''' </summary>
        ''' <param name="criteria">The criteria.</param>
        ''' <Returns></Returns>
        Public Function Fetch(ByVal criteria As SupplierCriteria) As Supplier
            Dim item As Supplier = Nothing
            
            Dim cancel As Boolean = False
            OnFetching(criteria, cancel)
            If (cancel) Then
                Return item
            End If
    
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
                            item = Map(reader)
                        Else
                            Throw New Exception(String.Format("The record was not found in 'Supplier' using the following criteria: {0}.", criteria))
                        End If
                    End Using
                End Using
            End Using
    
            MarkOld(item)
    
            OnFetched()
    
            Return item
        End Function
    
        #End Region
    
        #Region "Insert"
    
        Private Sub DoInsert(ByRef item As Supplier, ByVal stopProccessingChildren As Boolean)
            ' Don't update If the item isn't dirty.
            If Not (item.IsDirty) Then
                Return
            End If
    
            Dim cancel As Boolean = False
            OnInserting(cancel)
            If (cancel) Then
                Return
            End If
    
            Using connection As New SqlConnection(ADOHelper.ConnectionString)
                connection.Open()
                Using command As New SqlCommand("[dbo].[CSLA_Supplier_Insert]", connection)
                    command.CommandType = CommandType.StoredProcedure
                    command.Parameters.AddWithValue("@p_SuppId", item.SuppId)
			command.Parameters.AddWithValue("@p_Name", ADOHelper.NullCheck(item.Name))
			command.Parameters.AddWithValue("@p_Status", item.Status)
			command.Parameters.AddWithValue("@p_Addr1", ADOHelper.NullCheck(item.Addr1))
			command.Parameters.AddWithValue("@p_Addr2", ADOHelper.NullCheck(item.Addr2))
			command.Parameters.AddWithValue("@p_City", ADOHelper.NullCheck(item.City))
			command.Parameters.AddWithValue("@p_State", ADOHelper.NullCheck(item.State))
			command.Parameters.AddWithValue("@p_Zip", ADOHelper.NullCheck(item.Zip))
			command.Parameters.AddWithValue("@p_Phone", ADOHelper.NullCheck(item.Phone))
    
                    command.ExecuteNonQuery()
    
                End Using
            End Using
    
            item.OriginalSuppId = item.SuppId
    
            MarkOld(item)
            CheckRules(item)
            
            If Not (stopProccessingChildren) Then
                ' Update Child Items.
                Update_Item_Items_Supplier(item)
            End If
    
            OnInserted()
        End Sub
    
        #End Region
    
        #Region "Update"
    
        <Transactional(TransactionalTypes.TransactionScope)> _
        Public Function Update(ByVal item As Supplier) As Supplier
            Return Update(item, false)
        End Function
    
        Public Function Update(ByVal item As Supplier, ByVal stopProccessingChildren as Boolean) As Supplier
            If(item.IsDeleted) Then
                DoDelete(item)
                MarkNew(item)
            Else If(item.IsNew) Then
                DoInsert(item, stopProccessingChildren)
            Else
            DoUpdate(item, stopProccessingChildren)
            End If
    
            Return item
        End Function
    
        Private Sub DoUpdate(ByRef item As Supplier, ByVal stopProccessingChildren As Boolean)
            Dim cancel As Boolean = False
            OnUpdating(cancel)
            If (cancel) Then
                Return
            End If
    
            ' Don't update If the item isn't dirty.
            If (item.IsDirty) Then
    
                If Not item.OriginalSuppId = item.SuppId Then
                    ' Insert new child.
                    Dim temp As Supplier = CType(Activator.CreateInstance(GetType(Supplier), True), Supplier)
                    temp.SuppId = item.SuppId
			temp.Name = item.Name
			temp.Status = item.Status
			temp.Addr1 = item.Addr1
			temp.Addr2 = item.Addr2
			temp.City = item.City
			temp.State = item.State
			temp.Zip = item.Zip
			temp.Phone = item.Phone
                    temp = temp.Save()
    
                    ' Mark child lists as dirty. This code may need to be updated to one-to-one relationships.
                    For Each itemToUpdate As Item In item.Items
    		itemToUpdate.Supplier = item.SuppId
                    Next
    
                    ' Update Child Items.
                    Update_Item_Items_Supplier(item)
        
                    ' Delete the old.
                    Dim criteria As New SupplierCriteria()
                    criteria.SuppId = item.OriginalSuppId
                    Delete(criteria)
        
                    ' Mark the original as the new one.
                    item.OriginalSuppId = item.SuppId
                    MarkOld(item)
                    CheckRules(item)
    
                    OnUpdated()
    
                    Return
                End If
    
                Using connection As New SqlConnection(ADOHelper.ConnectionString)
                    connection.Open()
                    Using command As New SqlCommand("[dbo].[CSLA_Supplier_Update]", connection)
                        command.CommandType = CommandType.StoredProcedure
                        command.Parameters.AddWithValue("@p_OriginalSuppId", item.OriginalSuppId)
			command.Parameters.AddWithValue("@p_SuppId", item.SuppId)
			command.Parameters.AddWithValue("@p_Name", ADOHelper.NullCheck(item.Name))
			command.Parameters.AddWithValue("@p_Status", item.Status)
			command.Parameters.AddWithValue("@p_Addr1", ADOHelper.NullCheck(item.Addr1))
			command.Parameters.AddWithValue("@p_Addr2", ADOHelper.NullCheck(item.Addr2))
			command.Parameters.AddWithValue("@p_City", ADOHelper.NullCheck(item.City))
			command.Parameters.AddWithValue("@p_State", ADOHelper.NullCheck(item.State))
			command.Parameters.AddWithValue("@p_Zip", ADOHelper.NullCheck(item.Zip))
			command.Parameters.AddWithValue("@p_Phone", ADOHelper.NullCheck(item.Phone))
    
                        'result: The number of rows changed, inserted, or deleted. -1 for select statements; 0 if no rows were affected, or the statement failed. 
                        Dim result As Integer = command.ExecuteNonQuery()
                        If (result = 0) Then
                            throw new DBConcurrencyException("The entity is out of date on the client. Please update the entity and try again. This could also be thrown if the sql statement failed to execute.")
                        End If
                        
                    End Using
                End Using
            End If
    
            item.OriginalSuppId = item.SuppId
    
            MarkOld(item)
            CheckRules(item)
    
            If Not (stopProccessingChildren) Then
                ' Update Child Items.
                Update_Item_Items_Supplier(item)
            End If
    
            OnUpdated()
        End Sub
    
        #End Region
    
        #Region "Delete"
    
        <Transactional(TransactionalTypes.TransactionScope)> _
        Public Sub Delete(ByVal criteria As SupplierCriteria)
            ' Note: this call to delete is for immediate deletion and doesn't keep track of any entity state.
            DoDelete(criteria)
        End Sub
    
        Protected Sub DoDelete(ByRef item As Supplier)
            ' If we're not dirty then don't update the database.
            If Not (item.IsDirty) Then
                Return
            End If
    
            ' If we're New then don't call delete.
            If (item.IsNew) Then
                Return
            End If
    
            Dim criteria As New SupplierCriteria()
    criteria.SuppId = item.SuppId
            DoDelete(criteria)
    
            MarkNew(item)
        End Sub
    
        Private Sub DoDelete(ByVal criteria As SupplierCriteria)
            Dim cancel As Boolean = False
            OnDeleting(criteria, cancel)
            If (cancel) Then
                Return
            End If
    
            Using connection As New SqlConnection(ADOHelper.ConnectionString)
                connection.Open()
                Using command As New SqlCommand("[dbo].[CSLA_Supplier_Delete]", connection)
                    command.CommandType = CommandType.StoredProcedure
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag))
    
                    'result: The number of rows changed, inserted, or deleted. -1 for select statements; 0 if no rows were affected, or the statement failed. 
                    Dim result As Integer = command.ExecuteNonQuery()
                    If (result = 0) Then
                        throw new DBConcurrencyException("The entity is out of date on the client. Please update the entity and try again. This could also be thrown if the sql statement failed to execute.")
                    End If
                End Using
            End Using
    
            OnDeleted()
        End Sub
    
        #End Region
    
        #Region "Helper Methods"
    
        Public Function Map(ByVal reader As SafeDataReader) As Supplier
            Dim item As Supplier = CType(Activator.CreateInstance(GetType(Supplier), True), Supplier)
            Using BypassPropertyChecks(item)
                item.SuppId = reader.GetInt32("SuppId")
                item.OriginalSuppId = reader.GetInt32("SuppId")
                item.Name = reader.GetString("Name")
                item.Status = reader.GetString("Status")
                item.Addr1 = reader.GetString("Addr1")
                item.Addr2 = reader.GetString("Addr2")
                item.City = reader.GetString("City")
                item.State = reader.GetString("State")
                item.Zip = reader.GetString("Zip")
                item.Phone = reader.GetString("Phone")
            End Using
    
            MarkOld(item)
            Return item
        End Function
    
        'AssociatedOneToMany
        Private Shared Sub Update_Item_Items_Supplier(ByRef item As Supplier)
            For Each itemToUpdate As Item In item.Items
    		itemToUpdate.Supplier = item.SuppId
    
                Dim factory As New ItemFactory
                factory.Update(itemToUpdate, True)
            Next
        End Sub
    
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
        Partial Private Sub OnInserting(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnInserted()
        End Sub
        Partial Private Sub OnUpdating(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnUpdated()
        End Sub
        Partial Private Sub OnSelfDeleting(ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnSelfDeleted()
        End Sub
        Partial Private Sub OnDeleting(ByVal criteria As SupplierCriteria, ByRef cancel As Boolean)
        End Sub
        Partial Private Sub OnDeleted()
        End Sub
    
        #End Region
    End Class
End Namespace