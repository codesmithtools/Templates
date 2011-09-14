'------------------------------------------------------------------------------
'
' Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
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
Imports CodeSmith.Engine
Imports SchemaExplorer

Namespace CodeSmith.Samples

    <PropertySerializer(GetType(TableConfigurationSerializer))> _
    <Serializable()> _
    Public Class TableConfiguration

        Private _sourceTable As TableSchema
        Private _sourceView As ViewSchema

        Public Sub New()
        End Sub

        Public Property SourceTable() As TableSchema
            Get
                Return _sourceTable
            End Get
            Set(ByVal value As TableSchema)
                _sourceTable = value
            End Set
        End Property

        Public Property SourceView() As ViewSchema
            Get
                Return _sourceView
            End Get
            Set(ByVal value As ViewSchema)
                _sourceView = value
            End Set
        End Property

        Public Overloads Overrides Function ToString() As String
            If Not (SourceTable Is Nothing) Then
                Return SourceTable.ToString()
            End If

            Return "The table has not been set."
        End Function
    End Class

End Namespace