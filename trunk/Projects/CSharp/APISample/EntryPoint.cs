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
            // Disables CodeSmith's Error Tracking for SDK Customers.
            // It is recommended that you leave this enabled so we can continue to improve the quality and experience of CodeSmith.
            //CodeSmith.Engine.Insight.Disable();

            CodeTemplateCompiler compiler = new CodeTemplateCompiler("..\\..\\StoredProcedures.cst");
            compiler.Compile();

            if (compiler.Errors.Count == 0)
            {
                CodeTemplate template = compiler.CreateInstance();

                DatabaseSchema database = new DatabaseSchema(new SqlSchemaProvider(), @"Data Source=.\SQLEXPRESS;AttachDbFilename=PetShop.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True;");
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
        }
    }
}