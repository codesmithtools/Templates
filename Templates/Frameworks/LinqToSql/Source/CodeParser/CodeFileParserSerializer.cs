using System;
using System.IO;

namespace CodeSmith.Engine
{
    public class CodeFileParserSerializer : IPropertySerializer
    {
        #region IPropertySerializer Members
        public object LoadProperty(PropertySerializerContext context, object propertyValue)
        {
            if (propertyValue is string)
            {
                return CreateCodeFileParser(context, propertyValue.ToString());
            }

            return propertyValue;
        }

        public object SaveProperty(PropertySerializerContext context, object propertyValue)
        {
            return propertyValue;
        }

        public void WritePropertyXml(PropertySerializerContext context, System.Xml.XmlWriter writer, object propertyValue)
        {
            if (propertyValue == null)
                return;

            CodeFileParser codeFileParser = propertyValue as CodeFileParser;
            
            if (codeFileParser == null)
                return;

            string relativePath = Utility.PathUtil.RelativePathTo(context.WorkingDirectory, codeFileParser.FileName);

            writer.WriteString(File.Exists(Path.Combine(context.WorkingDirectory, relativePath))
                ? relativePath
                : codeFileParser.FileName);
        }

        public object ReadPropertyXml(PropertySerializerContext context, System.Xml.XmlNode propertyValue)
        {
            return CreateCodeFileParser(context, propertyValue.InnerText);
        }

        public object ParseDefaultValue(PropertySerializerContext context, string defaultValue)
        {
            return CreateCodeFileParser(context, defaultValue);
        }
        #endregion

        protected CodeFileParser CreateCodeFileParser(PropertySerializerContext context, string fileName)
        {
            CodeFileParser codeFileParser;
            bool parseMethodBodies = false;
            CodeTemplate template = context.Instance;

            if (template != null)
            {
                string loadString = template.GetPropertyAttribute(context.PropertyInfo.Name, "ParseMethodBodies");
                Boolean.TryParse(loadString, out parseMethodBodies);
            }
            
            try
            {
                codeFileParser = new CodeFileParser(fileName, context.WorkingDirectory, parseMethodBodies);
            }
            catch (FileNotFoundException)
            {
                codeFileParser = null;
            }

            return codeFileParser;
        }
    }
}
