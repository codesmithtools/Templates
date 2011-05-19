//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System.IO;
using System.Reflection;
using CodeSmith.Engine;

namespace Generator.Microsoft.Frameworks.Utility
{
    internal static class SystemTypeMapper
    {
        private static MapCollection _efConceptualTypeToSystemType;

        /// <summary>
        /// Returns the EntityFramework ConceptualType to SystemType MapCollection.
        /// </summary>
        /// <returns>Returns the correct SystemTypeEscape MapCollection.</returns>
        public static MapCollection EfConceptualTypeToSystemType
        {
            get
            {
                if (_efConceptualTypeToSystemType == null)
                {
                    string path;
                    if (!Map.TryResolvePath("EF-System", string.Empty, out path))
                    {
                        string baseDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
                        Map.TryResolvePath("EF-System", baseDirectory, out path);
                    }

                    _efConceptualTypeToSystemType = Map.Load(path);
                }

                return _efConceptualTypeToSystemType;
            }
        }
    }
}
