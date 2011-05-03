using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateProvider : IEntityProvider
    {
        public NHibernateProvider(string mapsDirectory)
        {
            MapsDirectory = mapsDirectory;
        }

        public string MapsDirectory { get; private set; }

        public string Description
        {
            get { return "Entity Provider for PLINQO for NHibernate"; }
        }

        public void Load()
        {
            Initialize();

            foreach (var file in _filePaths)
                using (var stream = new FileStream(file, FileMode.Open))
                using (var reader = XmlReader.Create(stream))
                {
                    var doc = XDocument.Load(reader);
                    var entity = new NHibernateEntity(doc, Path.GetFileName(file));
                    EntityStore.Instance.EntityCollection.Add(entity.Name, entity);
                }
        }

        public string Name
        {
            get { return "NHibernateManager"; }
        }

        public void Save()
        {
        }

        public bool Validate()
        {
            Initialize();

            return _filePaths != null && _filePaths.Length > 0;
        }

        private bool _intialized = false;

        private string[] _filePaths;

        private void Initialize()
        {
            if (_intialized)
                return;

            if (Directory.Exists(MapsDirectory))
                _filePaths = Directory.GetFiles(MapsDirectory, "*" + NHibernateUtilities.MapExtension);

            _intialized = true;
        }
    }
}
