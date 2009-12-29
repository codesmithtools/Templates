Imports System.Collections.Generic
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByEnumTests
        Inherits TestBase
        <Test()> _
        Public Sub ByTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByStatus(Status.InProgress).ToList()
                Dim b As List(Of Task) = db.Task.ByStatus(Status.NotStarted).ToList()

                Dim d As List(Of Task) = db.Task.ByStatus(Status.InProgress, Status.NotStarted).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)
            End Using
        End Sub

        <Test()> _
        Public Sub ByNullableTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByPriority(CType(Nothing, System.Nullable(Of Priority))).ToList()
                Dim b As List(Of Task) = db.Task.ByPriority(Priority.Normal).ToList()
                Dim c As List(Of Task) = db.Task.ByPriority(Priority.High).ToList()

                Dim d As List(Of Task) = db.Task.ByPriority(Priority.Normal, CType(Nothing, System.Nullable(Of Priority))).ToList()
                Assert.AreEqual(a.Count + b.Count, d.Count)

                Dim e As List(Of Task) = db.Task.ByPriority(Priority.Normal, CType(Nothing, System.Nullable(Of Priority)), Priority.High).ToList()
                Assert.AreEqual(a.Count + b.Count + c.Count, e.Count)
            End Using
        End Sub
    End Class
End Namespace
