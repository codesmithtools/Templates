Imports System.Collections.Generic
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class ByStringTests
        <Test()> _
        Public Sub ByNullableTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim a As List(Of Task) = db.Task.ByDetails(DirectCast(Nothing, String)).ToList()
                Dim b As List(Of Task) = db.Task.ByDetails(String.Empty).ToList()
                Dim c As List(Of Task) = db.Task.ByDetails("Hello world!").ToList()
                Dim d As List(Of Task) = db.Task.ByDetails("Goodnight moon!").ToList()

                Dim e As List(Of Task) = db.Task.ByDetails("Hello world!", DirectCast(Nothing, String)).ToList()
                Assert.AreEqual(a.Count + c.Count, e.Count)

                Dim f As List(Of Task) = db.Task.ByDetails(String.Empty, "Goodnight moon!").ToList()
                Assert.AreEqual(b.Count + d.Count, f.Count)

                Dim g As List(Of Task) = db.Task.ByDetails(Nothing, String.Empty, "Hello world!", "Goodnight moon!").ToList()
                Assert.AreEqual(a.Count + b.Count + c.Count + d.Count, g.Count)
            End Using
        End Sub
    End Class
End Namespace
