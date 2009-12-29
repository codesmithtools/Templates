Imports System.Collections.Generic
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByIntTests
        Inherits TestBase
        <Test()> _
        Public Sub ByTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByCreatedId(UserIds(1)).ToList()
                Dim b As List(Of Task) = db.Task.ByCreatedId(UserIds(2)).ToList()

                Dim d As List(Of Task) = db.Task.ByCreatedId(UserIds(1), UserIds(2)).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)
            End Using
        End Sub

        <Test()> _
        Public Sub ByNullableTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByAssignedId(CType(Nothing, System.Nullable(Of Integer))).ToList()
                Dim b As List(Of Task) = db.Task.ByAssignedId(UserIds(1)).ToList()
                Dim c As List(Of Task) = db.Task.ByAssignedId(UserIds(2)).ToList()

                Dim d As List(Of Task) = db.Task.ByAssignedId(UserIds(1), CType(Nothing, System.Nullable(Of Integer))).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)

                Dim e As List(Of Task) = db.Task.ByAssignedId(UserIds(1), Nothing, UserIds(2)).ToList()
                Assert.AreEqual(a.Count + b.Count + c.Count, e.Count)
            End Using
        End Sub
    End Class
End Namespace
