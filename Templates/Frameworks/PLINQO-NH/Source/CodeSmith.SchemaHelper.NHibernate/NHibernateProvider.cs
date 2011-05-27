using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateProvider : IEntityProvider
    {
        public const string EntitiesSubFolder = "Entities";
        public const string FunctionsSubFolder = "Functions";

        public NHibernateProvider(string mapsDirectory)
        {
            EntitiesDirectory = Path.Combine(mapsDirectory, EntitiesSubFolder);
            FunctionsDirectory = Path.Combine(mapsDirectory, FunctionsSubFolder);
        }

        public string EntitiesDirectory { get; private set; }

        public string FunctionsDirectory { get; private set; }

        public string Description
        {
            get { return "Entity Provider for PLINQO for NHibernate"; }
        }

        public void Load()
        {
            Initialize();

            foreach (var file in _entityPaths)
                using (var stream = new FileStream(file, FileMode.Open))
                using (var reader = XmlReader.Create(stream))
                {
                    var doc = XDocument.Load(reader);
                    var entity = new NHibernateEntity(doc, Path.GetFileName(file));
                    EntityStore.Instance.EntityCollection.Add(entity.Name, entity);
                }

            foreach (var file in _functionPaths)
                using (var stream = new FileStream(file, FileMode.Open))
                using (var reader = XmlReader.Create(stream))
                {
                    var doc = XDocument.Load(reader);
                    var entity = new NHibernateCommandEntity(doc, Path.GetFileName(file));
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

            return _entityPaths != null && _entityPaths.Length > 0;
        }

        private bool _intialized = false;

        private string[] _entityPaths;

        private string[] _functionPaths;

        private void Initialize()
        {
            if (_intialized)
                return;

            if (Directory.Exists(EntitiesDirectory))
                _entityPaths = Directory.GetFiles(EntitiesDirectory, "*" + NHibernateUtilities.MapExtension);

            if (Directory.Exists(FunctionsDirectory))
                _functionPaths = Directory.GetFiles(FunctionsDirectory, "*" + NHibernateUtilities.MapExtension);

            _intialized = true;
        }
    }
}
