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

namespace CodeSmith.Samples
{
    public class TableConfigurationCollectionSerializer : IPropertySerializer
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
            TableConfigurationCollection collection = propertyValue as TableConfigurationCollection;

            if (collection != null)
            {
                TableConfigurationSerializer itemSerializer = new TableConfigurationSerializer();

                foreach (TableConfiguration item in collection)
                {
                    writer.WriteStartElement("TableConfiguration");
                    itemSerializer.WritePropertyXml(context, writer, item);
                    writer.WriteEndElement();
                }
            }
        }

        public object ReadPropertyXml(PropertySerializerContext context, XmlNode propertyValue)
        {
            if (context.PropertyInfo.PropertyType != typeof(TableConfigurationCollection))
            {
                return null;
            }

            XPathNavigator navigator = propertyValue.CreateNavigator();
            XmlNamespaceManager oManager = new XmlNamespaceManager(navigator.NameTable);
            TableConfigurationCollection collection = new TableConfigurationCollection();
            TableConfigurationSerializer itemSerializer = new TableConfigurationSerializer();

            // Add the CodeSmith namespace
            oManager.AddNamespace("cs", CodeSmithProject.DefaultNamespace);

            // Loop through items
            XPathNodeIterator oIterator = navigator.Select("cs:TableConfiguration", oManager);
            while (oIterator.MoveNext())
            {
                collection.Add(itemSerializer.ReadPropertyXmlInner(context, ((IHasXmlNode)oIterator.Current).GetNode()));
            }

            return collection;
        }

        public object ParseDefaultValue(PropertySerializerContext context, string defaultValue)
        {
            return new TableConfigurationCollection();
        }

    }
}
