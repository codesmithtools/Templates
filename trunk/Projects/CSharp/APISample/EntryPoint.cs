using System;
using CodeSmith.Engine;
using SchemaExplorer;

namespace APISample {
    public class EntryPoint {
        [STAThread]
        public static void Main() {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\StoredProcedures.cst");
            var engine = new TemplateEngine(new DefaultEngineHost(System.IO.Path.GetDirectoryName(path)));

            CompileTemplateResult result = engine.Compile(path);
            if (result.Errors.Count == 0) {
                var database = new DatabaseSchema(new SqlSchemaProvider(), @"Server=.;Database=PetShop;Integrated Security=True;");
                TableSchema table = database.Tables["Inventory"];

                CodeTemplate template = result.CreateTemplateInstance();
                template.SetProperty("SourceTable", table);
                template.SetProperty("IncludeDrop", false);
                template.SetProperty("InsertPrefix", "Insert");

                template.Render(Console.Out);
            } else {
                foreach (var error in result.Errors)
                    Console.Error.WriteLine(error.ToString());
            }

            Console.WriteLine("\r\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}