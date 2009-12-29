Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Web
Imports CodeSmith.Data.Caching
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data
Imports Guid = System.Guid

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class FromCacheTests
        Inherits RoleTests
        <Test()> _
        Public Sub SimpleTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
            Dim roles As List(Of Role) = query.FromCache().ToList()

            Assert.IsInstanceOf(GetType(HttpCacheProvider), CacheManager.GetProvider())

            Dim key As String = CacheManager.GetProvider().GetGroupKey(query.GetHashKey(), Nothing)

            Dim cache As Byte() = CacheManager.GetProvider().[Get](Of Byte())(key)
            Assert.IsNotNull(cache)

            Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(roles.Count, list.Count)
        End Sub

        <Test()> _
        Public Sub DurationTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
            Dim key As String = query.GetHashKey()
            Dim roles As List(Of Role) = query.FromCache(2).ToList()

            Dim cache As Byte() = CacheManager.GetProvider().[Get](Of Byte())(key)
            Assert.IsNotNull(cache)

            Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(roles.Count, list.Count)

            Thread.Sleep(3000)

            cache = CacheManager.GetProvider().[Get](Of Byte())(key)
            Assert.IsNull(cache)
        End Sub

        <Test()> _
        Public Sub AbsoluteExpirationTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
            Dim key As String = query.GetHashKey()
            Dim roles As List(Of Role) = query.FromCache(2).ToList()

            Dim cache As Byte() = CacheManager.GetProvider().[Get](Of Byte())(key)
            Assert.IsNotNull(cache)

            Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(roles.Count, list.Count)

            Thread.Sleep(3000)

            cache = CacheManager.GetProvider().[Get](Of Byte())(key)
            Assert.IsNull(cache)
        End Sub

        <Test()> _
        Public Sub SlidingExpirationTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
            Dim key As String = query.GetHashKey()
            Dim roles As List(Of Role) = query.FromCache(New CacheSettings(TimeSpan.FromSeconds(2))).ToList()

            Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
            Assert.IsNotNull(cache)

            Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(roles.Count, list.Count)

            Thread.Sleep(1500)

            cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
            Assert.IsNotNull(cache)

            list = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(roles.Count, list.Count)

            Thread.Sleep(1500)

            cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
            Assert.IsNotNull(cache)

            list = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(roles.Count, list.Count)

            Thread.Sleep(2500)

            cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
            Assert.IsNull(cache)
        End Sub

        <Test()> _
        Public Sub NoCacheEmptyResultTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim guid__1 As String = Guid.NewGuid().ToString()
            Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = guid__1)
            Dim key As String = query.GetHashKey()
            Dim roles As List(Of Role) = query.FromCache(New CacheSettings(2) With { _
             .CacheEmptyResult = False _
            }).ToList()



            Assert.IsNotNull(roles)
            Assert.AreEqual(0, roles.Count())

            Dim cache As Object = HttpRuntime.Cache.[Get](key)
            Assert.IsNull(cache)
        End Sub

        <Test()> _
        Public Sub CacheEmptyResultTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim guid__1 As String = Guid.NewGuid().ToString()
            Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = guid__1)
            Dim key As String = query.GetHashKey()
            Dim roles As List(Of Role) = query.FromCache(New CacheSettings(2) With { _
             .CacheEmptyResult = True _
            }).ToList()



            Assert.IsNotNull(roles)

            Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
            Assert.IsNotNull(cache)

            Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
            Assert.IsNotNull(list)
            Assert.AreEqual(0, list.Count)
        End Sub
    End Class
End Namespace
