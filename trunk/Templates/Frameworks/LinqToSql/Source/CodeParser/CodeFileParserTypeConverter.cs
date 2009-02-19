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
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace CodeSmith.Engine
{
    /// <summary>
    /// This class can be used to convert to and from an CodeFileParser.
    /// </summary>
    public class PropertySerializerTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                CodeTemplate codeTemplate = context.Instance as CodeTemplate;
                PropertyInfo propertyInfo = context.PropertyDescriptor.ComponentType.GetProperty(context.PropertyDescriptor.Name);

                if (codeTemplate != null && propertyInfo != null)
                {
                    IPropertySerializer serializer = this.GetPropertySerializer(propertyInfo);
                    var ctx = new PropertySerializerContext(propertyInfo, codeTemplate);
                    return serializer.LoadProperty(ctx, value);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        private IPropertySerializer GetPropertySerializer(PropertyInfo targetProperty)
        {
            Type serializerType = null;

            object[] attributes = targetProperty.GetCustomAttributes(typeof(PropertySerializerAttribute), true);

            if (attributes.Length > 0 && attributes[0] is PropertySerializerAttribute)
            {
                serializerType = Type.GetType(((PropertySerializerAttribute)attributes[0]).SerializerTypeName);
            }
            else
            {
                attributes = targetProperty.PropertyType.GetCustomAttributes(typeof(PropertySerializerAttribute), true);

                if (attributes.Length > 0 && attributes[0] is PropertySerializerAttribute)
                {
                    serializerType = Type.GetType(((PropertySerializerAttribute)attributes[0]).SerializerTypeName);
                }
            }

            if (serializerType != null)
            {
                IPropertySerializer serializer = Activator.CreateInstance(serializerType) as IPropertySerializer;

                if (serializer != null)
                {
                    return serializer;
                }
            }

            return null;
        }
    }
}
