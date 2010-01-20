using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using SchemaExplorer;
using CodeSmith.SchemaHelper.Util;


namespace CodeSmith.SchemaHelper
{
    public class Association : Collection<AssociationMember>
    {
        //#region Constructor(s)

        //public Association(AssociationType associationType, TableSchema table, Entity entity)
        //{
        //    //ToManyTableKeyName = (associationType == SchemaHelper.AssociationType.ManyToMany)
        //    //                         ? GetToManyTableKey(columns[0].Table, table).Name
        //    //                         : String.Empty;

        //    //Columns = new Dictionary<Member, Member>();
        //    //for (int index = 0; index < columns.Count; index++)
        //    //{
        //    //    Columns.Add(new Member(columns[index], new Entity(columns[index].Table)), new Member(localColumns[index], new Entity(localColumns[index].Table)));
        //    //}

        //    //foreach (ColumnSchema col in columns)
        //    //{
        //    //    Cascade &= (associationType == SchemaHelper.AssociationType.OneToMany && !col.AllowDBNull);
        //    //}

        //    Entity = entity;
        //    //ClassName = table.ClassName();
        //    AssociationType = associationType;
        //    //Name = columns.GetName(table, associationType);
        //    Table = table;
        //}

        //#endregion

        //#region Public Read-Only Methods

        //internal TableSchema Table { get; private set; }
        //internal Entity Entity { get; private set; }
        //public List<AssociationMember> Members { get; }
        ////public string Name { get; internal set; }
        //public AssociationType AssociationType { get; private set; }
        ////public string ClassName { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="column"></param>
        ///// <param name="localColumn"></param>
        //public void AddMember(ColumnSchema column, ColumnSchema localColumn)
        //{
        //    Members.Add(new AssociationMember(this.AssociationType, Table, column, localColumn, Entity));
        //}

        //#endregion


        /// <summary>
        /// Returns the appended Key for all AssociatedMembers
        /// </summary>
        public string Key
        {
            get
            {
                string key = string.Empty;

                foreach (var item in this)
                {
                    key += item.Key + "|";  
                }

                return key;
            }
        }

        /// <summary>
        /// Returns the association type of the First associated member.
        /// 
        /// TODO: Handle different association types between members
        /// </summary>
        public AssociationType AssociationType
        {
            get
            {
                if(this.Count > 0)
                    return this[0].AssociationType;

                throw new Exception("There are no Associated members to get the AssociationType.");
            }
        }

        internal bool ContainsKey(string key)
        {
            foreach (var item in this)
            {
                if (key == item.Key)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the concatenated PropertyName of all associated members.
        /// </summary>
        public string PropertyName
        {
            get 
            {
                string propertyName = string.Empty;

                foreach (var item in this)
                {
                    propertyName += NamingConventions.PropertyName(item.Name); 
                }

                return propertyName;
            }
        }

        /// <summary>
        /// Returns the concatenated PrivateMemberVariableName of all associated members.
        /// </summary>
        public string PrivateMemberVariableName
        {
            get
            {
                string propertyName = string.Empty;

                foreach (var item in this)
                {
                    propertyName += NamingConventions.PrivateMemberVariableName(item.Name);
                }

                return propertyName;
            }
        }

        /// <summary>
        /// Returns the concatenated VariableName of all associated members.
        /// </summary>
        public string VariableName
        {
            get
            {
                string propertyName = string.Empty;

                foreach (var item in this)
                {
                    propertyName += NamingConventions.VariableName(item.Name);
                }

                return propertyName;
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description) && this.Count > 0)
                    _description = this[0].Table.Description;

                return _description;
            }
        }

        public bool HasDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(Description))
                    return Description.Trim().Length > 0;

                return false;
            }
        }

        /// <summary>
        /// Returns the ClassName for the first associated member
        /// </summary>
        public string ClassName
        {
            get
            {
                if (this.Count > 0)
                    return this[0].Table.ClassName();

                throw new Exception("There are no Associated members to get the ClassName.");
            }
        }

        public SearchCriteria SearchCriteria
        {
            get; internal set;
        }
    }
}