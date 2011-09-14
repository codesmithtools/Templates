using System;

using CodeSmith.Engine;

using SchemaExplorer;

namespace APISample
{
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            CodeTemplateCompiler compiler = new CodeTemplateCompiler("..\\..\\StoredProcedures.cst");
            compiler.Compile();

            if (compiler.Errors.Count == 0)
            {
                CodeTemplate template = compiler.CreateInstance();

                DatabaseSchema database = new DatabaseSchema(new SqlSchemaProvider(), @"Server=.;Database=PetShop;Integrated Security=True;");
                TableSchema table = database.Tables["Inventory"];

                template.SetProperty("SourceTable", table);
                template.SetProperty("IncludeDrop", false);
                template.SetProperty("InsertPrefix", "Insert");

                template.Render(Console.Out);
            }
            else
            {
                for (int i = 0; i < compiler.Errors.Count; i++)
                {
                    Console.Error.WriteLine(compiler.Errors[i].ToString());
                }
            }

            Console.WriteLine("\r\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}