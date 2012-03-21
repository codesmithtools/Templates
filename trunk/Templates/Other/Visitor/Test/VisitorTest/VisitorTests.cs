using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace VisitorTest
{
    [TestClass]
    public class VisitorTests
    {
        [TestMethod]
        public void CreateVisitorSample()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorSample";

            var visitor = Visitor.Create(assemblyFile, rootClass);
            Assert.IsNotNull(visitor);

            IEnumerable<Visitor> visitors = visitor.Traverse(v => v.Children).DistinctBy(v => v.VisitType);
            foreach (var v in visitors)
            {
                Console.WriteLine("Type: {0}, Collection: {1}", v.VisitType.Name, v.IsCollection);
            }
        }

        [TestMethod]
        public void IsNullable()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorChild";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            foreach (var propertyDefinition in typeDefintion.Properties)
            {
                var b = propertyDefinition.PropertyType.GetUnderlyingType();
                Console.WriteLine(b.FullName);
            }
        }

        [TestMethod]
        public void IsNotCollection()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorChild";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            var propertyDefinition = typeDefintion.Properties.First(p => p.Name == "Integer");
            TypeReference propertyType = propertyDefinition.PropertyType;

            TypeReference elementType;
            bool isCollection = propertyType.IsCollection(out elementType);

            Assert.IsFalse(isCollection);

        }

        [TestMethod]
        public void IsCollection()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorSample";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            var propertyDefinition = typeDefintion.Properties.First(p => p.Name == "ChildList");
            TypeReference propertyType = propertyDefinition.PropertyType;

            TypeReference elementType;
            bool isCollection = propertyType.IsCollection(out elementType);

            Assert.IsTrue(isCollection);

        }

        [TestMethod]
        public void IsCollectionNested()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorSample";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            var propertyDefinition = typeDefintion.Properties.First(p => p.Name == "VisitorChildCollection");
            TypeReference propertyType = propertyDefinition.PropertyType;

            TypeReference elementType;
            bool isCollection = propertyType.IsCollection(out elementType);

            Assert.IsTrue(isCollection);
        }


        [TestMethod]
        public void IsNotDictionary()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorChild";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            var propertyDefinition = typeDefintion.Properties.First(p => p.Name == "Integer");
            TypeReference propertyType = propertyDefinition.PropertyType;

            TypeReference keyType;
            TypeReference elementType;
            bool isDictionary = propertyType.IsDictionary(out keyType, out elementType);

            Assert.IsFalse(isDictionary);

        }

        [TestMethod]
        public void IsDictionary()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorSample";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            var propertyDefinition = typeDefintion.Properties.First(p => p.Name == "Dictionary");
            TypeReference propertyType = propertyDefinition.PropertyType;

            TypeReference keyType;
            TypeReference elementType;
            bool isDictionary = propertyType.IsDictionary(out keyType, out elementType);

            Assert.IsTrue(isDictionary);

        }

        [TestMethod]
        public void IsDictionaryNested()
        {
            string assemblyFile = @"..\..\..\Sample\bin\Debug\Sample.dll";
            string rootClass = "Sample.VisitorSample";

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
            var typeDefintion = moduleDefinition.GetType(rootClass);

            var propertyDefinition = typeDefintion.Properties.First(p => p.Name == "VisitorChildDictionary");
            TypeReference propertyType = propertyDefinition.PropertyType;

            TypeReference keyType;
            TypeReference elementType;
            bool isDictionary = propertyType.IsDictionary(out keyType, out elementType);

            Assert.IsTrue(isDictionary);
        }



        [TestMethod]
        public void TestRecursivePopulationThreeLevel()
        {
            var r1 = new Recursive(1,
                   new Recursive(2, new Recursive(4, new Recursive(5))),
                   new Recursive(3));

            List<Recursive> list = r1.Traverse(r => r.Children).ToList();

            Assert.AreEqual(5, list.Count);
        }
    }

    public class Recursive
    {
        public Recursive(int i, params Recursive[] recursive)
        {
            Id = i;
            Children = new List<Recursive>(recursive);
        }

        public int Id { get; set; }
        public List<Recursive> Children { get; set; }
    }
}
