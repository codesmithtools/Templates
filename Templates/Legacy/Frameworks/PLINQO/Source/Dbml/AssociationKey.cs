using System;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class AssociationKey : IEquatable<AssociationKey>
    {
        public AssociationKey(string name, bool isForeignKey)
        {
            this.isForeignKey = isForeignKey;
            this.name = name;
        }

        private bool isForeignKey;

        public bool IsForeignKey
        {
            get { return isForeignKey; }
        }

        private string name;

        public string Name
        {
            get { return name; }
        }

        public override bool Equals(object obj)
        {
            if (obj is AssociationKey)
                return Equals((AssociationKey)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            string hash = name + isForeignKey.ToString();
            return hash.GetHashCode();
        }

        public bool Equals(AssociationKey other)
        {
            if (other == null)
                return false;

            return (name == other.Name && isForeignKey == other.IsForeignKey);
        }

        public static AssociationKey CreateForeignKey(string name)
        {
            return new AssociationKey(name, true);
        }

        public static AssociationKey CreatePrimaryKey(string name)
        {
            return new AssociationKey(name, false);
        }
    }
}