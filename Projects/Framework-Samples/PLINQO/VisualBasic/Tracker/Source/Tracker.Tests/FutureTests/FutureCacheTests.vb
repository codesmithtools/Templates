Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Text
Imports CodeSmith.Data.Caching
Imports NUnit.Framework
Imports Tracker.Core.Data
Imports CodeSmith.Data.Linq

Namespace Tracker.Tests.FutureTests
    <TestFixture()> _
    Public Class FutureCacheTests
        Inherits TestBase

        <Test()> _
        Public Sub ExecuteBatch()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            ' build up queries

            Dim q1 = db.Task.Take(10).ToList()
            Dim q2 = db.Task.Take(10)

            Dim dbCommand = db.GetCommand(q2, True)


            Dim result = db.ExecuteQuery(Of Task)(dbCommand.CommandText)
            Dim list = result.ToList()

        End Sub

        <Test()> _
        Public Sub FutureTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 = db.User.ByEmailAddress("one@test.com").FutureCache(cache)
            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' this triggers the loading of all the future queries
            Dim users = q1.ToList()
            Assert.IsNotNull(users)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim tasks = q2.ToList()
            Assert.IsNotNull(tasks)

            ' queries are loaded and cached, run same queries again...
            Dim c1 = db.User.ByEmailAddress("one@test.com").FutureCache(cache)
            Dim c2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

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
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 = db.User.ByEmailAddress("two@test.com").FutureCache(cache)
            Dim q2 = db.Task.Where(Function(t) t.Details = "Hello world!").FutureCacheCount(cache)

            ' this triggers the loading of all the future queries
            Dim users = q1.ToList()
            Assert.IsNotNull(users)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim count = q2.Value
            Assert.Greater(count, 0)

            ' queries are loaded and cached, run same queries again...
            Dim c1 = db.User.ByEmailAddress("two@test.com").FutureCache(cache)
            Dim c2 = db.Task.Where(Function(t) t.Details = "Hello world!").FutureCacheCount(cache)

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
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 = db.User.ByEmailAddress("one@test.com").FutureCache(cache)

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' access q2 first to trigger loading, testing loading from FutureCount
            ' this triggers the loading of all the future queries
            Dim count = q2.Value
            Assert.Greater(count, 0)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q1, IFutureQuery).IsLoaded)

            Dim users = q1.ToList()
            Assert.IsNotNull(users)

            ' queries are loaded and cached, run same queries again...
            Dim c1 = db.User.ByEmailAddress("one@test.com").FutureCache(cache)

            Dim c2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

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
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 = db.User.ByEmailAddress("one@test.com").FutureCacheFirstOrDefault(cache)

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' duplicate query except count
            Dim q3 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' this triggers the loading of all the future queries
            Dim user = q1.Value
            Assert.IsNotNull(user)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q2, IFutureQuery).IsLoaded)

            Dim count = q2.Value
            Assert.Greater(count, 0)

            Dim tasks = q3.ToList()
            Assert.IsNotNull(tasks)

            ' queries are loaded and cached, run same queries again...
            Dim c1 = db.User.ByEmailAddress("one@test.com").FutureCacheFirstOrDefault(cache)

            Dim c2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            Dim c3 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

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
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 = db.User.Where(Function(u) u.EmailAddress = "one@test.com").FutureCacheFirstOrDefault(cache)

            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            ' duplicate query except count
            Dim q3 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

            ' access q2 first to trigger loading, testing loading from FutureCount
            ' this triggers the loading of all the future queries
            Dim count = q2.Value
            Assert.Greater(count, 0)

            ' this should already be loaded
            Assert.IsTrue(DirectCast(q1, IFutureQuery).IsLoaded)

            Dim user = q1.Value
            Assert.IsNotNull(user)

            Dim tasks = q3.ToList()
            Assert.IsNotNull(tasks)

            ' queries are loaded and cached, run same queries again...
            Dim c1 = db.User.ByEmailAddress("one@test.com").FutureCacheFirstOrDefault(cache)

            Dim c2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            Dim c3 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(cache)

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
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim cache As New CacheSettings(120)

            ' build up queries
            Dim q1 = db.Task.Where(Function(t) t.Summary = "Test").FromCache(cache).ToList()

            ' duplicate query except count
            Dim q2 = db.Task.Where(Function(t) t.Summary = "Test").FutureCache(120)

            Dim q3 = db.Task.Where(Function(t) t.Summary = "Test").FutureCacheCount(cache)

            Dim count = q3.Value
            Assert.Greater(count, 0)

            Dim tasks = q2.ToList()
            Assert.IsNotNull(tasks)
            Assert.AreEqual(q1.Count, tasks.Count)

        End Sub
    End Class
End Namespace
