Imports System
Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports System.Text
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.FutureTests
    <TestFixture()> _
    Public Class FutureTests
        Inherits TestBase

        <Test()> _
        Public Sub PageTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            ' base query
            Dim q = db.Task.ByPriority(Priority.Normal).OrderByDescending(Function(t) t.CreatedDate)

            ' get total count
            Dim q1 = q.FutureCount()
            ' get first page
            Dim q2 = q.Skip(0).Take(10).Future()
            ' triggers sql execute as a batch
            Dim tasks = q2.ToList()
            Dim total As Integer = q1.Value

            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub SimpleTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            ' build up queries

            Dim q1 = db.User.ByEmailAddress("one@test.com").Future()

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").Future()

            ' should be 2 queries 
            Assert.AreEqual(2, db.FutureQueries.Count)

            ' this triggers the loading of all the future queries
            Dim users = q1.ToList()
            Assert.IsNotNull(users)

            ' should be cleared at this point
            Assert.AreEqual(0, db.FutureQueries.Count)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim tasks = q2.ToList()
            Assert.IsNotNull(tasks)

        End Sub

        <Test()> _
        Public Sub FutureCountTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            ' build up queries

            Dim q1 = db.User.ByEmailAddress("one@test.com").Future()

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCount()

            ' should be 2 queries 
            Assert.AreEqual(2, db.FutureQueries.Count)

            ' this triggers the loading of all the future queries
            Dim users = q1.ToList()
            Assert.IsNotNull(users)

            ' should be cleared at this point
            Assert.AreEqual(0, db.FutureQueries.Count)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim count As Integer = q2
            Assert.Greater(count, 0)
        End Sub

        <Test()> _
        Public Sub FutureCountReverseTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            ' build up queries

            Dim q1 = db.User.ByEmailAddress("one@test.com").Future()

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCount()

            ' should be 2 queries 
            Assert.AreEqual(2, db.FutureQueries.Count)

            ' access q2 first to trigger loading, testing loading from FutureCount
            ' this triggers the loading of all the future queries
            Dim count = q2.Value
            Assert.Greater(count, 0)

            ' should be cleared at this point
            Assert.AreEqual(0, db.FutureQueries.Count)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q1, IFutureQuery).IsLoaded)

            Dim users = q1.ToList()
            Assert.IsNotNull(users)
        End Sub

        <Test()> _
        Public Sub FutureValueTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            ' build up queries
            Dim q1 = db.User.ByEmailAddress("one@test.com").FutureFirstOrDefault()

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCount()

            ' duplicate query except count
            Dim q3 = db.Task.Where(Function(t) t.Summary = "Test").Future()

            ' should be 3 queries 
            Assert.AreEqual(3, db.FutureQueries.Count)

            ' this triggers the loading of all the future queries
            Dim user As User = q1
            Assert.IsNotNull(user)

            ' should be cleared at this point
            Assert.AreEqual(0, db.FutureQueries.Count)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim count = q2.Value
            Assert.Greater(count, 0)

            Dim tasks = q3.ToList()
            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub FutureValueReverseTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }
            ' build up queries

            Dim q1 = db.User.Where(Function(u) u.EmailAddress = "one@test.com").FutureFirstOrDefault()

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCount()

            ' duplicate query except count
            Dim q3 = db.Task.Where(Function(t) t.Summary = "Test").Future()

            ' should be 3 queries 
            Assert.AreEqual(3, db.FutureQueries.Count)

            ' access q2 first to trigger loading, testing loading from FutureCount
            ' this triggers the loading of all the future queries
            Dim count = q2.Value
            Assert.Greater(count, 0)

            ' should be cleared at this point
            Assert.AreEqual(0, db.FutureQueries.Count)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q1, IFutureQuery).IsLoaded)

            Dim users = q1.Value
            Assert.IsNotNull(users)

            Dim tasks = q3.ToList()
            Assert.IsNotNull(tasks)

        End Sub

        <Test()> _
        Public Sub Test_ToPagedList()
            Try
                Using db = New TrackerDataContext()
                    db.User.[Select](Function(t) New With { _
                     Key .Id = t.Id, _
                     Key .FullName = t.FirstName _
                    }).ToPagedList(1, 3)
                End Using
            Catch e As Exception
                Console.WriteLine(e.Message)
            End Try
        End Sub
    End Class
End Namespace
