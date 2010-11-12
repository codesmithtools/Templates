Imports System.Collections.Generic
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data
Imports Guid = System.Guid

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByGuidTests
        Private ReadOnly _guid1Id As Guid = Guid.NewGuid()
        Private ReadOnly _guid2Id As Guid = Guid.NewGuid()
        Private ReadOnly _guid2Alt As Guid = Guid.NewGuid()
        Private ReadOnly _guid3Id As Guid = Guid.NewGuid()
        Private ReadOnly _guid3Alt As Guid = Guid.NewGuid()

        <TestFixtureSetUp()> _
        Public Sub TestFixtureSetUp()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Guid.InsertOnSubmit(New Tracker.Core.Data.Guid() With { _
                 .Id = _guid1Id, _
                 .AlternateId = Nothing _
                })
                db.Guid.InsertOnSubmit(New Tracker.Core.Data.Guid() With { _
                 .Id = _guid2Id, _
                 .AlternateId = _guid2Alt _
                })
                db.Guid.InsertOnSubmit(New Tracker.Core.Data.Guid() With { _
                 .Id = _guid3Id, _
                 .AlternateId = _guid3Alt _
                })

                db.SubmitChanges()
            End Using
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TestFixtureTearDown()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Guid.Delete(_guid1Id)
                db.Guid.Delete(_guid2Id)
                db.Guid.Delete(_guid3Id)
            End Using
        End Sub

        <Test()> _
        Public Sub ByTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Tracker.Core.Data.Guid) = db.Guid.ById(_guid1Id).ToList()
                Dim b As List(Of Tracker.Core.Data.Guid) = db.Guid.ById(_guid2Id).ToList()

                'Dim c As List(Of Tracker.Core.Data.Guid) = db.Guid.ById(_guid1Id, CType(Nothing, System.Nullable(Of Guid))).ToList()
                'Assert.AreEqual(a.Count, c.Count)

                Dim d As List(Of Tracker.Core.Data.Guid) = db.Guid.ById(_guid1Id, _guid2Id).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)
            End Using
        End Sub

        <Test()> _
        Public Sub ByNullableTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Tracker.Core.Data.Guid) = db.Guid.ByAlternateId(CType(Nothing, System.Nullable(Of Guid))).ToList()
                Dim b As List(Of Tracker.Core.Data.Guid) = db.Guid.ByAlternateId(_guid2Alt).ToList()
                Dim c As List(Of Tracker.Core.Data.Guid) = db.Guid.ByAlternateId(_guid3Alt).ToList()

                Dim d As List(Of Tracker.Core.Data.Guid) = db.Guid.ByAlternateId(_guid2Alt, CType(Nothing, System.Nullable(Of Guid))).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)

                Dim e As List(Of Tracker.Core.Data.Guid) = db.Guid.ByAlternateId(_guid2Alt, CType(Nothing, System.Nullable(Of Guid)), _guid3Alt).ToList()
                Assert.AreEqual(a.Count + b.Count + c.Count, e.Count)
            End Using
        End Sub
    End Class
End Namespace
