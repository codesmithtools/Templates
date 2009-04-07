using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace LinqToSqlShared.Generator.DbmlEnum
{
    [GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [SerializableAttribute()]
    [DebuggerStepThroughAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://tempuri.org/DbmlEnum.xsd")]
    [XmlRootAttribute("Database", Namespace = "http://tempuri.org/DbmlEnum.xsd", IsNullable = false)]
    public class Database
    {
        public static Database DeserializeFromFile(string fileName)
        {
            Database db = null;

            if (File.Exists(fileName))
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Database));
                    db = (Database)serializer.Deserialize(fileStream);
                }
            }

            return db;
        }
        public void SerializeToFile(string fileName)
        {
            this.Sort();

            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Database));
                serializer.Serialize(fileStream, this);
            }
        }

        public void Sort()
        {
            Enums = Enums.OrderBy(e => e.Name).ToList();

            foreach (Enum enumerator in Enums)
                enumerator.Sort();
        }

        [XmlElementAttribute("Enum")]
        public List<Enum> Enums
        {
            get
            {
                if (_enums == null)
                    _enums = new List<Enum>();
                return _enums;
            }
            set
            {
                _enums = value;
            }
        }
        private List<Enum> _enums = null;

        [XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private string _name;
    }
    
    [GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [SerializableAttribute()]
    [DebuggerStepThroughAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://tempuri.org/DbmlEnum.xsd")]
    public class Enum
    {
        public void Sort()
        {
            Items = Items.OrderBy(v => v.Value).ToList();
        }

        [XmlElementAttribute("Item")]
        public List<Item> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<Item>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
        private List<Item> _items = null;

        [XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private string _name;

        [XmlAttributeAttribute()]
        public string Table
        {
            get
            {
                return _table;
            }
            set
            {
                _table = value;
            }
        }
        private string _table;

        [XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        private string _type;

        [XmlAttributeAttribute()]
        public AccessModifier AccessModifier
        {
            get
            {
                return _accessModifier;
            }
            set
            {
                _accessModifier = value;
            }
        }
        private AccessModifier _accessModifier = AccessModifier.Public;

        [XmlAttributeAttribute()]
        public bool Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;
            }
        }
        private bool _flags = false;

        [XmlAttributeAttribute()]
        public bool IncludeDataContract
        {
            get
            {
                return _includeDataContract;
            }
            set
            {
                _includeDataContract = value;
            }
        }
        private bool _includeDataContract = true;
    }

    [GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [SerializableAttribute()]
    [DebuggerStepThroughAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://tempuri.org/DbmlEnum.xsd")]
    public class Item
    {
        [XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private string _name;

        [XmlAttributeAttribute()]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        private int _value;

        [XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        private string _description;

        [XmlAttributeAttribute()]
        public bool DataContractMember
        {
            get
            {
                return _dataContractMember;
            }
            set
            {
                _dataContractMember = value;
            }
        }
        private bool _dataContractMember = true;
    }

    [GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [SerializableAttribute()]
    [XmlTypeAttribute(Namespace = "http://tempuri.org/DbmlEnum.xsd")]
    public enum AccessModifier
    {
        Public,
        Internal
    }
}