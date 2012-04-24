using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateProvider : IEntityProvider
    {
        private bool _intialized;
        private string[] _entityPaths;
        private string[] _viewPaths;
        private string[] _functionPaths;

        public const string EntitiesSubFolder = "Entities";
        public const string ViewsSubFolder = "Views";
        public const string FunctionsSubFolder = "Functions";

        public NHibernateProvider(string mapsDirectory)
        {
            EntitiesDirectory = Path.Combine(mapsDirectory, EntitiesSubFolder);
            ViewsDirectory = Path.Combine(mapsDirectory, ViewsSubFolder);
            FunctionsDirectory = Path.Combine(mapsDirectory, FunctionsSubFolder);
        }

        public string EntitiesDirectory { get; private set; }
        public string ViewsDirectory { get; private set; }
        public string FunctionsDirectory { get; private set; }

        #region IEntityProvider Implementation

        public string Name
        {
            get { return "NHibernateManager"; }
        }

        public string Description
        {
            get { return "Entity Provider for PLINQO for NHibernate"; }
        }

        public bool Validate()
        {
            Initialize();

            return _entityPaths != null && _entityPaths.Length > 0;
        }

        public void Load()
        {
            Initialize();

            if (_entityPaths != null)
            {
                foreach (var file in _entityPaths)
                {
                    var doc = ReadHbmDocument(file);
                    if(doc == null)
                        continue;

                    var entity = new NHibernateEntity(doc, Path.GetFileName(file));
                    EntityStore.Instance.EntityCollection.Add(entity.Name, entity);
                }
            }
            if (_viewPaths != null)
            {
                foreach (var file in _viewPaths)
                {
                    var doc = ReadHbmDocument(file);
                    if (doc == null)
                        continue;

                    var entity = new NHibernateEntity(doc, Path.GetFileName(file), true);
                    EntityStore.Instance.EntityCollection.Add(entity.Name, entity);
                }
            }

            if (_functionPaths != null)
            {
                foreach (var file in _functionPaths)
                {
                    var doc = ReadHbmDocument(file);
                    if (doc == null)
                        continue;

                    var entity = new NHibernateCommandEntity(doc, Path.GetFileName(file));
                    EntityStore.Instance.EntityCollection.Add(entity.Name, entity);
                }
            }

            foreach (var entity in EntityStore.Instance.EntityCollection.Values)
                entity.Initialize();

            foreach (var entity in EntityStore.Instance.EntityCollection.Values)
                entity.ValidateAllMembers();
        }

        public void Save()
        {
        }

        #endregion

        private void Initialize()
        {
            if (_intialized)
                return;

            if (Directory.Exists(EntitiesDirectory))
                _entityPaths = Directory.GetFiles(EntitiesDirectory, String.Format("*{0}", NHibernateUtilities.MapExtension));

            if (Directory.Exists(ViewsDirectory))
                _viewPaths = Directory.GetFiles(ViewsDirectory, String.Format("*{0}", NHibernateUtilities.MapExtension));

            if (Directory.Exists(FunctionsDirectory))
                _functionPaths = Directory.GetFiles(FunctionsDirectory, String.Format("*{0}", NHibernateUtilities.MapExtension));

            _intialized = true;
        }

        private XDocument ReadHbmDocument(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = XmlReader.Create(stream))
                {
                    try
                    {
                        return XDocument.Load(reader);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Unable to load file: " + file);
                        Trace.WriteLine(ex.ToString());

                        Debug.WriteLine("Unable to load file " + file);
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }

            return null;
        }
    }
}
