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
using System.Reflection;
using CodeSmith.Engine;
using System.Xml.XPath;
using System.Xml;
using CodeSmith.Engine.Schema;

namespace CodeSmith.Samples
{
	/// <summary>
	/// This class provides CodeSmith serialization support for ModalEditorProperty.
	/// </summary>
	public class ModalEditorPropertySerializer : IPropertySerializer
	{
		public ModalEditorPropertySerializer()
		{
		}
		
		/// <summary>
		/// This method will be used to save the property value when a template is being compiled.
		/// </summary>
		/// <param name="propertyInfo">Information about the target property.</param>
		/// <param name="propertyValue">The property to be saved.</param>
		/// <returns>An object that will be stored in a Hashtable during template compilation.</returns>
        public object SaveProperty(PropertySerializerContext context, object propertyValue)
		{
			// Nothing special needs to be done to save this property so we just return the unmodified property value.
			return propertyValue;
		}
		
		/// <summary>
		/// This method will be used to restore the property value after a template has been compiled.
		/// </summary>
		/// <param name="propertyInfo">Information about the target property.</param>
		/// <param name="propertyValue">The property to be loaded.</param>
		/// <returns>The value to be assigned to the template property after it has been compiled.</returns>
        public object LoadProperty(PropertySerializerContext context, object propertyValue)
		{
			// Nothing special needs to be done to load this property so we just return the unmodified property value.
			return propertyValue;
		}
		
		/// <summary>
		/// This method will be used when serializing the property value to an XML property set.
		/// </summary>
		/// <param name="propertyInfo">Information about the target property.</param>
		/// <param name="writer">The XML writer that the property value will be written to.</param>
		/// <param name="propertyValue">The property to be serialized.</param>
        public void WritePropertyXml(PropertySerializerContext context, System.Xml.XmlWriter writer, object propertyValue)
		{
			if (propertyValue == null) return;
			
			ModalEditorProperty modalEditorPropertyValue = propertyValue as ModalEditorProperty;
			if (modalEditorPropertyValue != null)
			{
				writer.WriteElementString("SampleBoolean", modalEditorPropertyValue.SampleBoolean.ToString());
				writer.WriteElementString("SampleString", modalEditorPropertyValue.SampleString);
			}
		}
		
		/// <summary>
		/// This method will be used when deserializing the property from an XML property set.
		/// </summary>
		/// <param name="propertyInfo">Information about the target property.</param>
		/// <param name="propertyValue">The XML node to read the property value from.</param>
		/// <param name="basePath">The path to use for resolving file references.</param>
		/// <returns>The value to be assigned to the template property.</returns>
        public object ReadPropertyXml(PropertySerializerContext context, System.Xml.XmlNode propertyValue)
		{
            if (context.PropertyInfo.PropertyType != typeof(ModalEditorProperty))
				return null;

            // use XPath to select out values
            XPathNavigator navigator = propertyValue.CreateNavigator();
            // we need to import the CodeSmith Namespace
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("cs", CodeSmithProject.DefaultNamespace);

            // expresion to select SampleBoolean value
            XPathExpression sampleBooleanExpression = XPathExpression.Compile("string(cs:SampleBoolean/text())", manager);
            string boolString = navigator.Evaluate(sampleBooleanExpression) as string;
            bool sampleBoolean;
            bool.TryParse(boolString, out sampleBoolean);

            // expresion to select SampleString value
            XPathExpression sampleTextExpression = XPathExpression.Compile("string(cs:SampleString/text())", manager);
            string sampleString = navigator.Evaluate(sampleTextExpression) as string;

            ModalEditorProperty modalEditorPropertyValue = new ModalEditorProperty();
            modalEditorPropertyValue.SampleBoolean = sampleBoolean;
            modalEditorPropertyValue.SampleString = sampleString;
            return modalEditorPropertyValue;
		}
		
		/// <summary>
		/// This method will be used to parse a default value for a property when a template is being instantiated.
		/// </summary>
		/// <param name="propertyInfo">Information about the target property.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="basePath">The path to use for resolving file references.</param>
		/// <returns>An object that will be assigned to the template property.</returns>
        public object ParseDefaultValue(PropertySerializerContext context, string defaultValue)
		{
            if (context.PropertyInfo.PropertyType == typeof(ModalEditorProperty))
			{
				ModalEditorProperty modalEditorPropertyValue = new ModalEditorProperty();
				string[] values = defaultValue.Split(',');
				if (values.Length == 2)
				{
					modalEditorPropertyValue.SampleString = values[0];
					modalEditorPropertyValue.SampleBoolean = Boolean.Parse(values[1]);
					return modalEditorPropertyValue;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
	}
}
