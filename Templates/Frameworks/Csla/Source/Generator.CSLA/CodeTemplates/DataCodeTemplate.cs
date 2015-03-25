//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2012 CodeSmith Tools, LLC.  All rights reserved.
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
using System.ComponentModel;
using CodeSmith.SchemaHelper;

namespace Generator.CSLA
{
    public class DataCodeTemplate : EntityCodeTemplate
    {
        #region Constructor(s)

        public DataCodeTemplate()
        {
        }

        #endregion

        #region Public Properties

        [Category("3. Business Project")]
        [Description("If set to true then the Object Factory Child logic will be implemented.")]
        public bool IsChildBusinessObject { get; set; }

        [Category("3. Business Project")]
        [Description("If set to true then the Object Factory Read-Only logic will be implemented.")]
        public bool IsReadOnlyBusinessObject { get; set; }

        public bool IsReadOnly(IAssociation association, string suffix) {
            if (association.Properties.Count <= 0)
                return false;

            string key = String.Format("{0}{1}", association.ForeignEntity.EntityKeyName, suffix);
            if (ContextData.Get(key) == null)
                return false;

            var value = ContextData[key];
            switch (value) {
                case Constants.ReadOnlyChild:
                case Constants.ReadOnlyRoot:
                case Constants.ReadOnlyChildList:
                case Constants.ReadOnlyList:
                    return true;
            }

            return false;
        }

        [Category("4. Data Project")]
        [Description("The Name Space for the Data Project.")]
        public string DataProjectName { get; set; }

        #endregion
    }
}
