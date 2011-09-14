Imports System
Imports CodeSmith.Engine
Imports SchemaExplorer

Module EntryPoint
    Sub Main()
        Dim compiler As CodeTemplateCompiler
        compiler = New CodeTemplateCompiler ("..\\..\\StoredProcedures.cst")
        compiler.Compile()
        If compiler.Errors.Count = 0 Then
            Dim template As CodeTemplate
            template = compiler.CreateInstance()

            Dim database As DatabaseSchema
            database = _
                New DatabaseSchema(New SqlSchemaProvider(), "Server=(local);Database=PetShop;Integrated Security=True;")
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

        Console.WriteLine(Environment.NewLine + "Please press any key to continue.")
        Console.ReadKey()
    End Sub
End Module