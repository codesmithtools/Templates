﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'CategoryList.vb.
'
'     Template: DynamicRootList.DataAccess.StoredProcedures.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports System
Imports System.Data
Imports System.Data.SqlClient

Imports Csla
Imports Csla.Data

Namespace PetShop.Tests.Collections.DynamicRoot
    Public Partial Class CategoryList
        Private Sub DataPortal_Create()
        End Sub
    
        Private Shadows Sub DataPortal_Fetch(ByVal criteria As CategoryCriteria)
            Dim cancel As Boolean = False
            OnFetching(criteria, cancel)
            If (cancel) Then
                Return
            End If
    
            RaiseListChangedEvents = False
    
            ' Fetch Child objects.
            Using connection As New SqlConnection(ADOHelper.ConnectionString)
                connection.Open()
                Using command As New SqlCommand("[dbo].[CSLA_Category_Select]", connection)
                    command.CommandType = CommandType.StoredProcedure
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag))
                    command.Parameters.AddWithValue("@p_NameHasValue", criteria.NameHasValue)
					command.Parameters.AddWithValue("@p_DescnHasValue", criteria.DescriptionHasValue)
                    Using reader As SafeDataReader = New SafeDataReader(command.ExecuteReader())
                        If reader.Read() Then
                            Do
                                Me.Add(New PetShop.Tests.Collections.DynamicRoot.Category(reader))
                            Loop While reader.Read()
                        End If
                    End Using
                End Using
            End Using
    
            RaiseListChangedEvents = True
    
            OnFetched()
        End Sub
    End Class
End Namespace
