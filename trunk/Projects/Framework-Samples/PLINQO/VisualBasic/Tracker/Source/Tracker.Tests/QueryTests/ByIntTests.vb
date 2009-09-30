Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByIntTests
        Inherits TaskTests
        <Test()> _
        Public Sub ByTest()
            Try
                Using db = New TrackerDataContext()
                    Dim a = db.Task.ByCreatedId(userIds(1)).ToList()
                    Dim b = db.Task.ByCreatedId(userIds(2)).ToList()

                    Dim c = db.Task.ByCreatedId(userIds(1), Nothing).ToList()
                    Assert.AreEqual(a.Count, c.Count)

                    Dim d = db.Task.ByCreatedId(userIds(1), userIds(2)).ToList()
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
                    Dim a = db.Task.ByAssignedId(Nothing).ToList()
                    Dim b = db.Task.ByAssignedId(userIds(1)).ToList()
                    Dim c = db.Task.ByAssignedId(userIds(2)).ToList()

                    Dim d = db.Task.ByAssignedId(userIds(1), Nothing).ToList()
                    Assert.AreEqual(a.Count + b.Count, d.Count)

                    Dim e = db.Task.ByAssignedId(userIds(1), Nothing, userIds(2)).ToList()
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
