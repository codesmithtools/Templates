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
            Enumerators = Enumerators.OrderBy(e => e.Name).ToList();

            foreach (Enumerator enumerator in Enumerators)
                enumerator.Sort();
        }

        [XmlElementAttribute("Enumerator")]
        public List<Enumerator> Enumerators
        {
            get
            {
                if (_enumerators == null)
                    _enumerators = new List<Enumerator>();
                return _enumerators;
            }
            set
            {
                _enumerators = value;
            }
        }
        private List<Enumerator> _enumerators = null;

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
    public class Enumerator
    {
        public void Sort()
        {
            Values = Values.OrderBy(v => v.IntegerValue).ToList();
        }

        [XmlElementAttribute("Value")]
        public List<Value> Values
        {
            get
            {
                if (_values == null)
                    _values = new List<Value>();
                return _values;
            }
            set
            {
                _values = value;
            }
        }
        private List<Value> _values = null;

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
    }

    [GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [SerializableAttribute()]
    [DebuggerStepThroughAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://tempuri.org/DbmlEnum.xsd")]
    public class Value
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
        public int IntegerValue
        {
            get
            {
                return _integerValue;
            }
            set
            {
                _integerValue = value;
            }
        }
        private int _integerValue;

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
        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                _summary = value;
            }
        }
        private string _summary;
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