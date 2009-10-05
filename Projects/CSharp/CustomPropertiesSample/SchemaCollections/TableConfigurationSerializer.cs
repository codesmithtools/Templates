//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2009 CodeSmith Tools, LLC.  All rights reserved.
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
using System.Xml;
using System.Xml.XPath;
using CodeSmith.Engine;
using CodeSmith.Engine.Schema;
using SchemaExplorer;
using SchemaExplorer.Serialization;

namespace CodeSmith.Samples
{
    public class TableConfigurationSerializer : IPropertySerializer
    {

        public object SaveProperty(PropertySerializerContext context, object propertyValue)
        {
            return propertyValue;
        }

        public object LoadProperty(PropertySerializerContext context, object propertyValue)
        {
            return propertyValue;
        }

        public void WritePropertyXml(PropertySerializerContext context, XmlWriter writer, object propertyValue)
        {
            TableConfiguration configuration = propertyValue as TableConfiguration;

            if (configuration != null)
            {
                DatabaseSchema database = null;
                if (configuration.SourceTable != null)
                {
                    database = configuration.SourceTable.Database;
                }
                else if (configuration.SourceView != null)
                {
                    database = configuration.SourceView.Database;
                }

                if (database != null)
                {
                    writer.WriteStartElement("DatabaseSchema");
                    new DatabaseSchemaSerializer().WritePropertyXml(context, writer, database);
                    writer.WriteEndElement();
                }

                if (configuration.SourceTable != null)
                {
                    writer.WriteStartElement("SourceTable");
                    writer.WriteElementString("Owner", configuration.SourceTable.Owner);
                    writer.WriteElementString("Name", configuration.SourceTable.Name);
                    writer.WriteEndElement();
                }

                if (configuration.SourceView != null)
                {
                    writer.WriteStartElement("SourceView");
                    writer.WriteElementString("Owner", configuration.SourceView.Owner);
                    writer.WriteElementString("Name", configuration.SourceView.Name);
                    writer.WriteEndElement();
                }
            }
        }

        public object ReadPropertyXml(PropertySerializerContext context, XmlNode propertyValue)
        {
            if (context.PropertyInfo.PropertyType != typeof(TableConfiguration))
            {
                return null;
            }
            return ReadPropertyXmlInner(context, propertyValue);
        }

        internal TableConfiguration ReadPropertyXmlInner(PropertySerializerContext context, XmlNode propertyValue)
        {
            XPathNavigator navigator = propertyValue.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            XPathNodeIterator iterator;

            // Add the CodeSmith namespace
            manager.AddNamespace("cs", CodeSmithProject.DefaultNamespace);

            // Get the DatabaseSchema
            DatabaseSchema databaseSchema = null;
            iterator = (XPathNodeIterator)navigator.Evaluate("cs:DatabaseSchema", manager);
            if (iterator.MoveNext())
            {
                XmlNode oNode = ((IHasXmlNode)iterator.Current).GetNode();
                databaseSchema = (DatabaseSchema)new DatabaseSchemaSerializer().ReadPropertyXml(context, oNode);
            }

            // Get the SourceTable
            TableSchema sourceTable = null;
            if (databaseSchema != null)
            {
                string strOwner = navigator.Evaluate("string(cs:SourceTable/cs:Owner/text())", manager) as string;
                string strName = navigator.Evaluate("string(cs:SourceTable/cs:Name/text())", manager) as string;

                if (!string.IsNullOrEmpty(strName))
                {
                    sourceTable = !string.IsNullOrEmpty(strOwner) ? databaseSchema.Tables[strOwner, strName] : databaseSchema.Tables[strName];
                }
            }

            // Get the SourceView
            ViewSchema sourceView = null;
            if (databaseSchema != null)
            {
                string strOwner = navigator.Evaluate("string(cs:SourceView/cs:Owner/text())", manager) as string;
                string strName = navigator.Evaluate("string(cs:SourceView/cs:Name/text())", manager) as string;

                if (!string.IsNullOrEmpty(strName))
                {
                    sourceView = !string.IsNullOrEmpty(strOwner) ? databaseSchema.Views[strOwner, strName] : databaseSchema.Views[strName];
                }
            }

            // Create and return
            return new TableConfiguration
                       {
                SourceTable = sourceTable,
                SourceView = sourceView
            };
        }

        public object ParseDefaultValue(PropertySerializerContext context, string defaultValue)
        {
            return null;
        }
    }
}
