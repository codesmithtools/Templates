//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2008 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System;
using CodeSmith.Engine;
using SchemaExplorer;

namespace ConsoleSample
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
				
				DatabaseSchema database = new DatabaseSchema(new SqlSchemaProvider(), @"Server=(local);Database=Petshop;Integrated Security=true;");
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
