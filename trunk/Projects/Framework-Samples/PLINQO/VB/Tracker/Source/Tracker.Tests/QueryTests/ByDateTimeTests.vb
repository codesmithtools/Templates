Imports System
Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByDateTimeTests
        Inherits TaskTests
        <Test()> _
        Public Sub ByTest()
            Try
                Using db = New TrackerDataContext()
                    Dim a = db.Task.ByCreatedDate(createDates(0)).ToList()
                    Dim b = db.Task.ByCreatedDate(createDates(1)).ToList()

                    Dim c = db.Task.ByCreatedDate(createDates(0), Nothing).ToList()
                    Assert.AreEqual(a.Count, c.Count)

                    Dim d = db.Task.ByCreatedDate(createDates(0), createDates(1)).ToList()
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
                    Dim a = db.Task.ByDueDate(Nothing).ToList()
                    Dim b = db.Task.ByDueDate(dueDates(0)).ToList()
                    Dim c = db.Task.ByDueDate(dueDates(1)).ToList()

                    Dim d = db.Task.ByDueDate(dueDates(0), Nothing).ToList()
                    Assert.AreEqual(a.Count + b.Count, d.Count)

                    Dim e = db.Task.ByDueDate(dueDates(0), Nothing, dueDates(1)).ToList()
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
