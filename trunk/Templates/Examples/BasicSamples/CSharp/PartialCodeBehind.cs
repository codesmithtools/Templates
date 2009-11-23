// Name: Partial Template.
// Author: Blake Niemyjski
// Description: Shows off how to use a code behind as a partial class.

using System;

namespace MyCustomNameSpace
{
    public partial class PartialTemplate
    {
        // This property uses the property that is declared in the template.
        public string PropertyInCodeBehind
        {
            get
            {
                return string.Format("{0}-Example", PropertyInTemplate);
            }
        }
    }
}