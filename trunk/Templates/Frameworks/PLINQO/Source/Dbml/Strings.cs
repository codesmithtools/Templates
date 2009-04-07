namespace LinqToSqlShared.DbmlObjectModel
{
    internal static class Strings
    {
        internal static string Bug(object p0)
        {
            return string.Format("BUG {0}", new[] {p0});
        }

        internal static string DatabaseNodeNotFound(object p0)
        {
            return string.Format("Database node not found.  Is the DBML namespace ({0}) correctly specified?",
                                 new[] {p0});
        }

        internal static string ElementMoreThanOnceViolation(object p0)
        {
            return string.Format("Element {0} must only appear at most once.", new[] {p0});
        }

        internal static string InvalidBooleanAttributeValueViolation(object p0)
        {
            return string.Format("The boolean attribute value '{0}' is invalid.", new[] {p0});
        }

        internal static string InvalidEnumAttributeValueViolation(object p0)
        {
            return string.Format("The value '{0}' is invalid.", new[] {p0});
        }

        internal static string RequiredAttributeMissingViolation(object p0)
        {
            return string.Format("Required attribute {0} is missing.", new[] {p0});
        }

        internal static string RequiredElementMissingViolation(object p0)
        {
            return string.Format("Element {0} is required.", new[] {p0});
        }

        internal static string SchemaDuplicateIdViolation(object p0, object p1)
        {
            return string.Format("Duplicate {0} seen with value '{1}'.", new[] {p0, p1});
        }

        internal static string SchemaExpectedEmptyElement(object p0, object p1, object p2)
        {
            return string.Format("Element '{0}' must be empty, but contains a node of type {1} named '{2}'.",
                                 new[] {p0, p1, p2});
        }

        internal static string SchemaInvalidIdRefToNonexistentId(object p0, object p1, object p2)
        {
            return string.Format("Unresolved reference {0} {1}: '{2}'.", new[] {p0, p1, p2});
        }

        internal static string SchemaOrRequirementViolation(object p0, object p1, object p2)
        {
            return string.Format("{0} requires {1} or {2}.", new[] {p0, p1, p2});
        }

        internal static string SchemaRecursiveTypeReference(object p0, object p1)
        {
            return
                string.Format(
                    "The type IdRef '{0}' cannot point to the type '{1}' because it is in the same inheritance hierarchy.",
                    new[] {p0, p1});
        }

        internal static string SchemaRequirementViolation(object p0, object p1)
        {
            return string.Format("{0} requires {1}.", new[] {p0, p1});
        }

        internal static string SchemaUnexpectedAdditionalAttributeViolation(object p0, object p1)
        {
            return string.Format("Attribute {0} on element {1} must appear alone.", new[] {p0, p1});
        }

        internal static string SchemaUnexpectedElementViolation(object p0, object p1)
        {
            return string.Format("Element {0} is unexpected under {1} element.", new[] {p0, p1});
        }

        internal static string SchemaUnrecognizedAttribute(object p0)
        {
            return string.Format("Unrecognized attribute '{0}' in file.", new[] {p0});
        }

        internal static string TypeNameNotUnique(object p0)
        {
            return string.Format("The Name attribute '{0}' of the Type element is already used by another type.",
                                 new[] {p0});
        }
    }
}