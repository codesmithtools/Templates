using System;
using System.ComponentModel;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    [Serializable]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SearchCriteriaProperty
    {
        #region Constructor(s)

        public SearchCriteriaProperty()
        {
            SearchCriteria = SearchCriteriaEnum.All;
            Prefix = "GetBy";
            Delimeter = String.Empty;
            Suffix = String.Empty;
        }

        #endregion

        #region Public Overridden Method(s)

        public override string ToString()
        {
            return "(Expand to edit...)";
        }

        #endregion

        #region Public Properties

        [NotifyParentProperty(true)]
        [Description("Which SearchCriteria to generate.")]
        public SearchCriteriaEnum SearchCriteria { get; set; }

        [NotifyParentProperty(true)]
        [Description("Prefix for a search method.")]
        public string Prefix { get; set; }

        [NotifyParentProperty(true)]
        [Description("Delimeter between member names for a search method..")]
        public string Delimeter { get; set; }

        [NotifyParentProperty(true)]
        [Description("Suffix for a search method.")]
        public string Suffix { get; set; }

        #endregion
    }
}