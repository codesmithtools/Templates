using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        private static readonly string[] emptyKeys = new string[0];

        public static string BuildKeyField(string[] columnNames)
        {
            return columnNames == null ? string.Empty : string.Join(",", columnNames);
        }

        public static Database CopyWithFilledInDefaults(Database db)
        {
            Database database = new DbmlDuplicator().DuplicateDatabase(db);
            var assigner = new DbmlDefaultValueAssigner();
            return assigner.AssignDefaultValues(database);
        }

        public static Database CopyWithNulledOutDefaults(Database db)
        {
            Database database = new DbmlDuplicator().DuplicateDatabase(db);
            var nullifier = new DbmlDefaultValueNullifier();
            return nullifier.NullifyDefaultValues(database);
        }

        public static Database Duplicate(Database db)
        {
            Database database = new DbmlDuplicator().DuplicateDatabase(db);
            return database;
        }

        public static void FillInDefaults(Database db)
        {
            new DbmlDefaultValueAssigner().AssignDefaultValues(db);
        }

        public static Database FromFile(string dbmlFile)
        {
            var reader = new DbmlReader();
            return reader.FileToDbml(dbmlFile);
        }

        public static Database FromStream(Stream dbmlStream)
        {
            var reader = new DbmlReader();
            return reader.StreamToDbml(dbmlStream);
        }

        public static Dictionary<Association, Association> GetAssociationPairs(Database db)
        {
            return PairAssociations.Gather(db);
        }

        public static string[] GetPrimaryKeys(Type type)
        {
            var list = new List<string>();
            foreach (Column column in type.Columns)
            {
                if (column.IsPrimaryKey == true)
                {
                    list.Add(column.Name);
                }
            }
            return list.ToArray();
        }

        public static Dictionary<string, Table> GetTablesByTypeName(Database db)
        {
            return TypeTableLookup.CreateLookup(db);
        }

        public static bool HasPrimaryKey(Type type)
        {
            foreach (Column column in type.Columns)
            {
                if (column.IsPrimaryKey == true)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPrimaryKeyOfType(Type type, string[] columns)
        {
            int found = 0;
            int keyCount = 0;
            foreach (Column column in type.Columns)
            {
                if (column.IsPrimaryKey != true)
                {
                    continue;
                }
                keyCount++;
                foreach (string str in columns)
                {
                    if (column.Name == str)
                    {
                        found++;
                        break;
                    }
                }
                if (keyCount != found)
                {
                    return false;
                }
            }
            return (columns.Length == found);
        }

        public static void NullOutDefaults(Database db)
        {
            new DbmlDefaultValueNullifier().NullifyDefaultValues(db);
        }

        public static string[] ParseKeyField(string keyField)
        {
            return string.IsNullOrEmpty(keyField) ? emptyKeys : keyField.Split(new[] {','});
        }

        public static void ToFile(Database db, string dbmlFile)
        {
            ToFile(db, dbmlFile, Encoding.UTF8);
        }

        public static void ToFile(Database db, string dbmlFile, Encoding encoding)
        {
            new DbmlSerializer().DbmlToFile(db, dbmlFile, encoding);
        }

        public static string ToText(Database db)
        {
            return ToText(db, Encoding.UTF8);
        }

        public static string ToText(Database db, Encoding encoding)
        {
            var serializer = new DbmlSerializer();
            return serializer.DbmlToString(db, encoding);
        }

        public static void Verify(Database db, string message)
        {
            new VerifyDbml(message).VisitDatabase(db);
        }
    }
}