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
Imports System.ComponentModel

Namespace CodeSmith.Samples

    <PropertySerializer(GetType(ModalEditorPropertySerializer))> _
    <Editor(GetType(ModalEditorPropertyEditor), GetType(System.Drawing.Design.UITypeEditor))> _
    Public Class ModalEditorProperty
        Dim _sampleString As String
        Private _sampleBoolean As Boolean

        Public Sub New()

        End Sub

        Public Sub New(ByVal sampleString As String, ByVal sampleBoolean As Boolean)
            _sampleString = sampleString
            _sampleBoolean = sampleBoolean
        End Sub

        Public Property SampleString() As String
            Get
                Return _sampleString
            End Get
            Set(ByVal value As String)
                _sampleString = value
            End Set
        End Property

        Public Property SampleBoolean() As Boolean
            Get
                Return _sampleBoolean
            End Get
            Set(ByVal value As Boolean)
                _sampleBoolean = value
            End Set
        End Property

        ''' <summary>
        ''' The value that we return here will be shown in the property grid.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return SampleString + ": " + SampleBoolean
        End Function

    End Class

End Namespace