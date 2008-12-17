using System;
using System.Xml;
using System.Xml.Schema;

namespace LinqToSqlShared.DbmlObjectModel
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName);
        }

        internal static Exception ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        internal static Exception Bug(object p0)
        {
            return new InvalidOperationException(Strings.Bug(p0));
        }

        internal static Exception DatabaseNodeNotFound(object p0)
        {
            return new XmlException(Strings.DatabaseNodeNotFound(p0));
        }

        internal static Exception ElementMoreThanOnceViolation(object p0)
        {
            return new XmlException(Strings.ElementMoreThanOnceViolation(p0));
        }

        internal static Exception ElementMoreThanOnceViolation(object p0, int line)
        {
            return new XmlSchemaException(Strings.ElementMoreThanOnceViolation(p0), null, line, 0);
        }

        internal static Exception InvalidBooleanAttributeValueViolation(object p0)
        {
            return new XmlException(Strings.InvalidBooleanAttributeValueViolation(p0));
        }

        internal static Exception InvalidBooleanAttributeValueViolation(object p0, int line)
        {
            return new XmlSchemaException(Strings.InvalidBooleanAttributeValueViolation(p0), null, line, 0);
        }

        internal static Exception InvalidEnumAttributeValueViolation(object p0)
        {
            return new XmlException(Strings.InvalidEnumAttributeValueViolation(p0));
        }

        internal static Exception InvalidEnumAttributeValueViolation(object p0, int line)
        {
            return new XmlSchemaException(Strings.InvalidEnumAttributeValueViolation(p0), null, line, 0);
        }

        internal static Exception NotImplemented()
        {
            return new NotImplementedException();
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception RequiredAttributeMissingViolation(object p0)
        {
            return new XmlException(Strings.RequiredAttributeMissingViolation(p0));
        }

        internal static Exception RequiredAttributeMissingViolation(object p0, int line)
        {
            return new XmlSchemaException(Strings.RequiredAttributeMissingViolation(p0), null, line, 0);
        }

        internal static Exception RequiredElementMissingViolation(object p0)
        {
            return new XmlException(Strings.RequiredElementMissingViolation(p0));
        }

        internal static Exception RequiredElementMissingViolation(object p0, int line)
        {
            return new XmlSchemaException(Strings.RequiredElementMissingViolation(p0), null, line, 0);
        }

        internal static Exception SchemaDuplicateIdViolation(object p0, object p1)
        {
            return new XmlException(Strings.SchemaDuplicateIdViolation(p0, p1));
        }

        internal static Exception SchemaDuplicateIdViolation(object p0, object p1, int line)
        {
            return new XmlSchemaException(Strings.SchemaDuplicateIdViolation(p0, p1), null, line, 0);
        }

        internal static Exception SchemaExpectedEmptyElement(object p0, object p1, object p2)
        {
            return new XmlException(Strings.SchemaExpectedEmptyElement(p0, p1, p2));
        }

        internal static Exception SchemaInvalidIdRefToNonexistentId(object p0, object p1, object p2)
        {
            return new XmlException(Strings.SchemaInvalidIdRefToNonexistentId(p0, p1, p2));
        }

        internal static Exception SchemaInvalidIdRefToNonexistentId(object p0, object p1, object p2, int line)
        {
            return new XmlSchemaException(Strings.SchemaInvalidIdRefToNonexistentId(p0, p1, p2), null, line, 0);
        }

        internal static Exception SchemaOrRequirementViolation(object p0, object p1, object p2)
        {
            return new XmlException(Strings.SchemaOrRequirementViolation(p0, p1, p2));
        }

        internal static Exception SchemaOrRequirementViolation(object p0, object p1, object p2, int line)
        {
            return new XmlSchemaException(Strings.SchemaOrRequirementViolation(p0, p1, p2), null, line, 0);
        }

        internal static Exception SchemaRecursiveTypeReference(object p0, object p1)
        {
            return new XmlException(Strings.SchemaRecursiveTypeReference(p0, p1));
        }

        internal static Exception SchemaRecursiveTypeReference(object p0, object p1, int line)
        {
            return new XmlSchemaException(Strings.SchemaRecursiveTypeReference(p0, p1), null, line, 0);
        }

        internal static Exception SchemaRequirementViolation(object p0, object p1)
        {
            return new XmlException(Strings.SchemaRequirementViolation(p0, p1));
        }

        internal static Exception SchemaUnexpectedAdditionalAttributeViolation(object p0, object p1)
        {
            return new XmlException(Strings.SchemaUnexpectedAdditionalAttributeViolation(p0, p1));
        }

        internal static Exception SchemaUnexpectedAdditionalAttributeViolation(object p0, object p1, int line)
        {
            return new XmlSchemaException(Strings.SchemaUnexpectedAdditionalAttributeViolation(p0, p1), null, line, 0);
        }

        internal static Exception SchemaUnexpectedElementViolation(object p0, object p1)
        {
            return new XmlException(Strings.SchemaUnexpectedElementViolation(p0, p1));
        }

        internal static Exception SchemaUnexpectedElementViolation(object p0, object p1, int line)
        {
            return new XmlSchemaException(Strings.SchemaUnexpectedElementViolation(p0, p1), null, line, 0);
        }

        internal static Exception SchemaUnrecognizedAttribute(object p0)
        {
            return new XmlException(Strings.SchemaUnrecognizedAttribute(p0));
        }

        internal static Exception SchemaUnrecognizedAttribute(string attribute, int line)
        {
            return new XmlSchemaException(Strings.SchemaUnrecognizedAttribute(attribute), null, line, 0);
        }

        internal static Exception TypeNameNotUnique(object p0)
        {
            return new XmlException(Strings.TypeNameNotUnique(p0));
        }

        internal static Exception TypeNameNotUnique(string attribute, int line)
        {
            return new XmlSchemaException(Strings.TypeNameNotUnique(attribute), null, line, 0);
        }
    }
}