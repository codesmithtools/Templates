Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByEnumTests
        Inherits TaskTests
        <Test()> _
        Public Sub ByTest()
            Try
                Using db = New TrackerDataContext()
                    Dim a = db.Task.ByStatus(Status.InProgress).ToList()
                    Dim b = db.Task.ByStatus(Status.NotStarted).ToList()

                    Dim c = db.Task.ByStatus(Status.InProgress, Nothing).ToList()
                    Assert.AreEqual(a.Count, c.Count)

                    Dim d = db.Task.ByStatus(Status.InProgress, Status.NotStarted).ToList()
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
                    Dim a = db.Task.ByPriority(Nothing).ToList()
                    Dim b = db.Task.ByPriority(Priority.Normal).ToList()
                    Dim c = db.Task.ByPriority(Priority.High).ToList()

                    Dim d = db.Task.ByPriority(Priority.Normal, Nothing).ToList()
                    Assert.AreEqual(a.Count + b.Count, d.Count)

                    Dim e = db.Task.ByPriority(Priority.Normal, Nothing, Priority.High).ToList()
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
