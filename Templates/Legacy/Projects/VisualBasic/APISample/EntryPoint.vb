Imports System
Imports CodeSmith.Engine
Imports SchemaExplorer

Module EntryPoint
    Sub Main()
        Dim path As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\..\StoredProcedures.cst")
        Dim engine As New TemplateEngine(New DefaultEngineHost(IO.Path.GetDirectoryName(path)))

        Dim result As CompileTemplateResult = engine.Compile(path)
        If result.Errors.Count = 0 Then
            Dim database As New DatabaseSchema(New SqlSchemaProvider(), "Server=.;Database=PetShop;Integrated Security=True;")
            Dim table As TableSchema = database.Tables("Inventory")

            Dim template As CodeTemplate = result.CreateTemplateInstance()
            template.SetProperty("SourceTable", table)
            template.SetProperty("IncludeDrop", False)
            template.SetProperty("InsertPrefix", "Insert")

            template.Render(Console.Out)
        Else
            For Each e As Object In result.Errors
                Console.Error.WriteLine(e.ToString())
            Next
        End If

        Console.WriteLine(Environment.NewLine + "Please press any key to continue.")
        Console.ReadKey()
    End Sub
End Module