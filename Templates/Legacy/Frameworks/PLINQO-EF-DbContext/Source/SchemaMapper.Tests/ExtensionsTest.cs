using SchemaMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SchemaMapper.Tests
{
  [TestClass]
  public class ExtensionsTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void IsCSharpKeywordTest()
    {
      bool actual;

      actual = "Public".IsKeyword(CodeLanguage.CSharp);
      Assert.IsFalse(actual);

      actual = "case".IsKeyword(CodeLanguage.CSharp);
      Assert.IsTrue(actual);

      actual = "public".IsKeyword(CodeLanguage.CSharp);
      Assert.IsTrue(actual);

      actual = "void".IsKeyword(CodeLanguage.CSharp);
      Assert.IsTrue(actual);

      actual = "do".IsKeyword(CodeLanguage.CSharp);
      Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsVisualBasicKeywordTest()
    {
      bool actual;

      actual = "Public".IsKeyword(CodeLanguage.VisualBasic);
      Assert.IsTrue(actual);

      actual = "case".IsKeyword(CodeLanguage.VisualBasic);
      Assert.IsTrue(actual);

      actual = "public".IsKeyword(CodeLanguage.VisualBasic);
      Assert.IsTrue(actual);

      actual = "do".IsKeyword(CodeLanguage.VisualBasic);
      Assert.IsTrue(actual);
    }

    [TestMethod]
    public void ToNullableTypeTest()
    {
      var int64Type = typeof (Int64);
      var dateTimeType = typeof (DateTime);
      var stringType = typeof (string);

      string actual = int64Type.ToNullableType(true);
      Assert.AreEqual("long?", actual);
      
      actual = int64Type.ToNullableType(false);
      Assert.AreEqual("long", actual);
      
      actual = dateTimeType.ToNullableType(true);
      Assert.AreEqual("System.DateTime?", actual);
      
      actual = dateTimeType.ToNullableType(false);
      Assert.AreEqual("System.DateTime", actual);

      actual = stringType.ToNullableType(true);
      Assert.AreEqual("string", actual);

      actual = stringType.ToNullableType(false);
      Assert.AreEqual("string", actual);

    }

    [TestMethod]
    public void UniqueNamerTest()
    {
      var namer = new UniqueNamer();
      Assert.IsNotNull(namer);

      string result;

      result = namer.UniqueName("Tester", "Users");
      Assert.AreEqual("Users", result);

      result = namer.UniqueName("Tester", "Users");
      Assert.AreEqual("Users1", result);

      result = namer.UniqueName("Tester", "Users");
      Assert.AreEqual("Users2", result);

    }
  }
}
