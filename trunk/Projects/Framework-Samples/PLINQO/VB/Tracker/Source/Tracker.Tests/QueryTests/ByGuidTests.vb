Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data
Imports Guid = System.Guid

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByGuidTests
        Private _guid1Id As Guid = Guid.NewGuid()
        Private _guid2Id As Guid = Guid.NewGuid()
        Private _guid2Alt As Guid = Guid.NewGuid()
        Private _guid3Id As Guid = Guid.NewGuid()
        Private _guid3Alt As Guid = Guid.NewGuid()

        <TestFixtureSetUp()> _
        Public Sub TestFixtureSetUp()
            Using db = New TrackerDataContext()
                Dim guid1 = New Core.Data.Guid()
                guid1.Id = _guid1Id
                guid1.AlternateId = Nothing
                db.Guid.InsertOnSubmit(guid1)

                Dim guid2 = New Core.Data.Guid()
                guid2.Id = _guid2Id
                guid2.AlternateId = _guid2Alt
                db.Guid.InsertOnSubmit(guid2)

                Dim guid3 = New Core.Data.Guid()
                guid3.Id = _guid3Id
                guid3.AlternateId = _guid3Alt
                db.Guid.InsertOnSubmit(guid3)

                db.SubmitChanges()
            End Using
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TestFixtureTearDown()
            Using db = New TrackerDataContext()
                db.Guid.Delete(_guid1Id)
                db.Guid.Delete(_guid2Id)
                db.Guid.Delete(_guid3Id)
            End Using
        End Sub

        <Test()> _
        Public Sub ByTest()
            Try
                Using db = New TrackerDataContext()
                    Dim a = db.Guid.ById(_guid1Id).ToList()
                    Dim b = db.Guid.ById(_guid2Id).ToList()

                    Dim c = db.Guid.ById(_guid1Id, Nothing).ToList()
                    Assert.AreEqual(a.Count, c.Count)

                    Dim d = db.Guid.ById(_guid1Id, _guid2Id).ToList()
                    Assert.AreEqual(a.Count + b.Count, d.Count)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub

        <Test()> _
        Public Sub ByNullableTest()
            Try
                Using db = New TrackerDataContext()
                    Dim a = db.Guid.ByAlternateId(Nothing).ToList()
                    Dim b = db.Guid.ByAlternateId(_guid2Alt).ToList()
                    Dim c = db.Guid.ByAlternateId(_guid3Alt).ToList()

                    Dim d = db.Guid.ByAlternateId(_guid2Alt, Nothing).ToList()
                    Assert.AreEqual(a.Count + b.Count, d.Count)

                    Dim e = db.Guid.ByAlternateId(_guid2Alt, Nothing, _guid3Alt).ToList()
                    Assert.AreEqual(a.Count + b.Count + c.Count, e.Count)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub
    End Class
End Namespace
