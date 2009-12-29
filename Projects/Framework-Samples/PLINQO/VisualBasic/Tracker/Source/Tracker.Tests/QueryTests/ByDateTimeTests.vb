Imports System.Collections.Generic
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByDateTimeTests
        Inherits TestBase
        <Test()> _
        Public Sub ByTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByCreatedDate(CreateDates(0)).ToList()
                Dim b As List(Of Task) = db.Task.ByCreatedDate(CreateDates(1)).ToList()

                'Dim c As List(Of Task) = db.Task.ByCreatedDate(CreateDates(0), CType(Nothing, System.Nullable(Of DateTime))).ToList()
                'Assert.AreEqual(a.Count, c.Count)
            End Using
        End Sub

        <Test()> _
        Public Sub ByNullableTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByDueDate(CType(Nothing, System.Nullable(Of DateTime))).ToList()
                Dim b As List(Of Task) = db.Task.ByDueDate(DueDates(0)).ToList()
                Dim c As List(Of Task) = db.Task.ByDueDate(DueDates(1)).ToList()

                Dim d As List(Of Task) = db.Task.ByDueDate(DueDates(0), CType(Nothing, System.Nullable(Of DateTime))).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)

                Dim e As List(Of Task) = db.Task.ByDueDate(DueDates(0), CType(Nothing, System.Nullable(Of DateTime)), DueDates(1)).ToList()
                Assert.AreEqual(a.Count + b.Count + c.Count, e.Count)
            End Using
        End Sub
    End Class
End Namespace
