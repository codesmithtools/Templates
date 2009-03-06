using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickStartUtils
{
    public enum LanguageEnum
    {
        CSharp = 1,
        VB = 2
    }

    public enum ProjectTypeEnum
    {
        None = 0,
        DynamicDataWebApp = 1,
        DynamicDataWebSite = 2
    }

    public enum SkinEnum
    {
        Default
    }

    public enum QueryPatternEnum
    {
        ManagerClasses,
        QueryExtensions
    }
}
