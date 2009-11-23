' Name: Partial Template.
' Author: Blake Niemyjski
' Description: Shows off how to use a code behind as a partial class.

Imports System

Namespace MyCustomNameSpace

    Public Partial Class PartialTemplate
        ' This property uses the property that is declared in the template.
        Public ReadOnly Property PropertyInCodeBehind As String
           Get
                Return String.Format("{0}-Example", PropertyInTemplate)
            End Get
        End Property
    End Class
    
End Namespace