using System;
using System.Collections.Generic;
using CodeSmith.Data.Linq;
using System.Text;

namespace CodeSmith.Data
{
    public interface ILinqEntity
    {
        void Detach();
        bool IsAttached();
        string ToEntityString(int indentLevel, string indentValue);
    }
}
