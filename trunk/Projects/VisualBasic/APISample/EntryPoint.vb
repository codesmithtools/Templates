Imports System
Imports CodeSmith.Engine
Imports SchemaExplorer

Module EntryPoint
    Sub Main()

        ' Disables CodeSmith's Error Tracking for SDK Customers.
        ' It is recommended that you leave this enabled so we can continue to improve the quality and experience of CodeSmith.
        'CodeSmith.Engine.Insight.Disable()

        Dim compiler As CodeTemplateCompiler
        compiler = New CodeTemplateCompiler ("..\\..\\StoredProcedures.cst")
        compiler.Compile()
        If compiler.Errors.Count = 0 Then
            Dim template As CodeTemplate
            template = compiler.CreateInstance()

            Dim database As DatabaseSchema
            database = _
                New DatabaseSchema (New SqlSchemaProvider(), "Data Source=.\SQLEXPRESS;AttachDbFilename=PetShop.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True")
            Dim table As TableSchema
            table = database.Tables ("Inventory")

            template.SetProperty ("SourceTable", table)
            template.SetProperty ("IncludeDrop", False)
            template.SetProperty ("InsertPrefix", "Insert")

            template.Render (Console.Out)
        Else
            Dim i As Integer
            For i = 0 To compiler.Errors.Count
                Console.Error.WriteLine (compiler.Errors (i).ToString())
                Console.Read()
            Next
        End If
    End Sub
End Module