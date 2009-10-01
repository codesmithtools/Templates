Imports System
Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByStringTests
        <Test()> _
        Public Sub ByNullableTest()
            Try
                Using db = New TrackerDataContext()
                    Dim a = db.Task.ByDetails(Nothing).ToList()
                    Dim b = db.Task.ByDetails([String].Empty).ToList()
                    Dim c = db.Task.ByDetails("Hello world!").ToList()
                    Dim d = db.Task.ByDetails("Goodnight moon!").ToList()

                    Dim e = db.Task.ByDetails("Hello world!", Nothing).ToList()
                    Assert.AreEqual(a.Count + c.Count, e.Count)

                    Dim f = db.Task.ByDetails([String].Empty, "Goodnight moon!").ToList()
                    Assert.AreEqual(b.Count + d.Count, f.Count)

                    Dim g = db.Task.ByDetails(Nothing, [String].Empty, "Hello world!", "Goodnight moon!").ToList()
                    Assert.AreEqual(a.Count + b.Count + c.Count + d.Count, g.Count)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub

    End Class
End Namespace
