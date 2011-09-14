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
Imports System.Collections.ObjectModel
Imports System.Text
Imports CodeSmith.Engine

Namespace CodeSmith.Samples

    <PropertySerializer(GetType(TableConfigurationCollectionSerializer))> _
    <Serializable()> _
    Public Class TableConfigurationCollection
        Inherits Collection(Of TableConfiguration)

        Public Sub New()
        End Sub

        Public Overloads Overrides Function ToString() As String
            Dim builder As New StringBuilder()

            For Each oItem As TableConfiguration In Me.Items
                If builder.Length > 0 Then
                    builder.Append(", ")
                End If
                builder.Append(oItem.SourceTable.Name)
            Next

            Return builder.ToString()
        End Function
    End Class

End Namespace