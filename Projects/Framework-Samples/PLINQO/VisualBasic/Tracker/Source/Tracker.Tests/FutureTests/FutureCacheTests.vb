Imports System.Collections.Generic
Imports System.Data.Common
Imports System.Data.Linq
Imports System.Linq
Imports CodeSmith.Data.Caching
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.FutureTests
    <TestFixture()> _
    Public Class FutureCacheTests
        Inherits TestBase
        <Test()> _
        Public Sub ExecuteBatch()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            ' build up queries

            Dim q1 As List(Of Task) = db.Task.Take(10).ToList()


            Dim q2 As IQueryable(Of Task) = db.Task.Take(10)

            Dim dbCommand As DbCommand = db.GetCommand(q2, True)


            Dim result As IEnumerable(Of Task) = db.ExecuteQuery(Of Task)(dbCommand.CommandText)
            Dim list As List(Of Task) = result.ToList()
        End Sub

        <Test()> _
        Public Sub FutureTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 As FutureQuery(Of User) = db.User.ByEmailAddress("one@test.com").FutureCache(cache)

            Dim q2 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' this triggers the loading of all the future queries
            Dim users As List(Of User) = q1.ToList()
            Assert.IsNotNull(users)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim tasks As List(Of Task) = q2.ToList()
            Assert.IsNotNull(tasks)

            ' queries are loaded and cached, run same queries again...
            Dim c1 As FutureQuery(Of User) = db.User.ByEmailAddress("one@test.com").FutureCache(cache)

            Dim c2 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' should be loaded because it came from cache
            Assert.IsTrue(DirectCast(c1, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c2, IFutureQuery).IsLoaded)

            users = c1.ToList()
            Assert.IsNotNull(users)

            tasks = c2.ToList()
            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub FutureCountTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 As FutureQuery(Of User) = db.User.ByEmailAddress("two@test.com").FutureCache(cache)

            Dim q2 As FutureCount = db.Task.Where(Function(t) t.Details = "Hello world!").FutureCacheCount(cache)

            ' this triggers the loading of all the future queries
            Dim users As List(Of User) = q1.ToList()
            Assert.IsNotNull(users)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim count As Integer = q2.Value
            Assert.Greater(count, 0)

            ' queries are loaded and cached, run same queries again...
            Dim c1 As FutureQuery(Of User) = db.User.ByEmailAddress("two@test.com").FutureCache(cache)

            Dim c2 As FutureCount = db.Task.Where(Function(t) t.Details = "Hello world!").FutureCacheCount(cache)

            ' should be loaded because it came from cache
            Assert.IsTrue(DirectCast(c1, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c2, IFutureQuery).IsLoaded)

            users = c1.ToList()
            Assert.IsNotNull(users)

            count = c2.Value
            Assert.Greater(count, 0)
        End Sub

        <Test()> _
        Public Sub FutureCountReverseTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 As FutureQuery(Of User) = db.User.ByEmailAddress("one@test.com").FutureCache(cache)

            Dim q2 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' access q2 first to trigger loading, testing loading from FutureCount
            ' this triggers the loading of all the future queries
            Dim count As Integer = q2.Value
            Assert.Greater(count, 0)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q1, IFutureQuery).IsLoaded)

            Dim users As List(Of User) = q1.ToList()
            Assert.IsNotNull(users)

            ' queries are loaded and cached, run same queries again...
            Dim c1 As FutureQuery(Of User) = db.User.ByEmailAddress("one@test.com").FutureCache(cache)

            Dim c2 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' should be loaded because it came from cache
            Assert.IsTrue(DirectCast(c1, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c2, IFutureQuery).IsLoaded)

            count = c2.Value
            Assert.Greater(count, 0)

            users = c1.ToList()
            Assert.IsNotNull(users)
        End Sub

        <Test()> _
        Public Sub FutureValueTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 As FutureValue(Of User) = db.User.ByEmailAddress("one@test.com").FutureCacheFirstOrDefault(cache)

            Dim q2 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' duplicate query except count
            Dim q3 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' this triggers the loading of all the future queries
            Dim user As User = q1.Value
            Assert.IsNotNull(user)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim count As Integer = q2.Value
            Assert.Greater(count, 0)

            Dim tasks As List(Of Task) = q3.ToList()
            Assert.IsNotNull(tasks)

            ' queries are loaded and cached, run same queries again...
            Dim c1 As FutureValue(Of User) = db.User.ByEmailAddress("one@test.com").FutureCacheFirstOrDefault(cache)

            Dim c2 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            Dim c3 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' should be loaded because it came from cache
            Assert.IsTrue(DirectCast(c1, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c2, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c3, IFutureQuery).IsLoaded)

            user = c1.Value
            Assert.IsNotNull(user)

            count = c2.Value
            Assert.Greater(count, 0)

            tasks = c3.ToList()
            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub FutureValueReverseTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 As FutureValue(Of User) = db.User.Where(Function(u) u.EmailAddress = "one@test.com").FutureCacheFirstOrDefault(cache)

            Dim q2 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' duplicate query except count
            Dim q3 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' access q2 first to trigger loading, testing loading from FutureCount
            ' this triggers the loading of all the future queries
            Dim count As Integer = q2.Value
            Assert.Greater(count, 0)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q1, IFutureQuery).IsLoaded)

            Dim user As User = q1.Value
            Assert.IsNotNull(user)

            Dim tasks As List(Of Task) = q3.ToList()
            Assert.IsNotNull(tasks)

            ' queries are loaded and cached, run same queries again...
            Dim c1 As FutureValue(Of User) = db.User.ByEmailAddress("one@test.com").FutureCacheFirstOrDefault(cache)

            Dim c2 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            Dim c3 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' should be loaded because it came from cache
            Assert.IsTrue(DirectCast(c1, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c2, IFutureQuery).IsLoaded)
            Assert.IsTrue(DirectCast(c3, IFutureQuery).IsLoaded)

            user = c1.Value
            Assert.IsNotNull(user)

            count = c2.Value
            Assert.Greater(count, 0)

            tasks = c3.ToList()
            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub FromCacheFutre()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 As List(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FromCache(cache).ToList()

            ' duplicate query except count
            Dim q2 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(120)

            Dim q3 As FutureCount = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            Dim count As Integer = q3.Value
            Assert.Greater(count, 0)

            Dim tasks As List(Of Task) = q2.ToList()
            Assert.IsNotNull(tasks)
            Assert.AreEqual(q1.Count, tasks.Count)
        End Sub
    End Class
End Namespace
