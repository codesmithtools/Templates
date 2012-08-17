using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: VerifyDbml

        private class VerifyDbml : DbmlVisitor
        {
            private readonly Dictionary<string, Type> dbTypes = new Dictionary<string, Type>();
            private readonly string message;

            public VerifyDbml(string message)
            {
                this.message = message;
            }

            public override Type VisitType(Type type)
            {
                Type t;
                if (dbTypes.TryGetValue(type.Name, out t))
                {
                    if (t != type)
                        throw Error.Bug(" (" + message + "): Type with name " + type.Name +
                                        " has multiple Type instances in DBML.");
                }
                else
                    dbTypes.Add(type.Name, type);

                return base.VisitType(type);
            }
        }

        #endregion
    }
}