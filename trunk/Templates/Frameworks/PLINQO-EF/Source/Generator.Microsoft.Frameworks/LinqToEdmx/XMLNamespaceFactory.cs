using System;

namespace LinqToEdmx {
    internal static class XMLNamespaceFactory {
        private static int _version = 6;
        public static int Version { get { return _version; } set { _version = value >= 4 && value <= 6 ? value : 6; } }

        public static string Edm {
            get {
                switch (_version) {
                    case 4:
                    case 5:
                        return "http://schemas.microsoft.com/ado/2008/09/edm";
                    default:
                        return "http://schemas.microsoft.com/ado/2009/11/edm";
                }
            }

        }

        public static string Edmx {
            get {
                switch (_version) {
                    case 4:
                    case 5:
                        return "http://schemas.microsoft.com/ado/2008/10/edmx";
                    default:
                        return "http://schemas.microsoft.com/ado/2009/11/edmx";
                }
            }
        }

        public static string SSDL {
            get {
                switch (_version) {
                    case 4:
                    case 5:
                        return "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
                    default:
                        return "http://schemas.microsoft.com/ado/2009/11/edm/ssdl";
                }
            }
        }

        public static string CS {
            get {
                switch (_version) {
                    case 4:
                    case 5:
                        return "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
                    default:
                        return "http://schemas.microsoft.com/ado/2009/11/mapping/cs";
                }
            }
        }

        public static string Annotation { get { return "http://schemas.microsoft.com/ado/2009/02/edm/annotation"; } }

        public static string EntityStoreSchemaGenerator { get { return "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"; } }
    }
}