Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports CodeSmith.Data.Caching
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class FromCacheFirstOrDefaultTests
        Inherits RoleTests
        <Test()> _
        Public Sub SimpleTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.Take(1).GetHashKey()
                Dim role = query.FromCacheFirstOrDefault()

                Dim cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(role.Id, cache.FirstOrDefault().Id)
            End Using
        End Sub

        <Test()> _
        Public Sub DurationTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.Take(1).GetHashKey()
                Dim role = query.FromCacheFirstOrDefault(CacheSettings.FromDuration(2))

                Dim cache1 = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache1)
                Assert.AreEqual(role.Id, cache1.FirstOrDefault().Id)

                System.Threading.Thread.Sleep(3000)

                Dim cache2 = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNull(cache2)
            End Using
        End Sub

        <Test()> _
        Public Sub AbsoluteExpirationTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.Take(1).GetHashKey()
                Dim role = query.FromCacheFirstOrDefault(CacheSettings.FromAbsolute(DateTime.Now.AddSeconds(2)))

                Dim cache1 = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache1)
                Assert.AreEqual(role.Id, cache1.FirstOrDefault().Id)

                System.Threading.Thread.Sleep(2500)

                Dim cache2 = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNull(cache2)
            End Using
        End Sub

        <Test()> _
        Public Sub SlidingExpirationTest()
            Using db = New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.Take(1).GetHashKey()
                Dim role = query.FromCacheFirstOrDefault(New CacheSettings(TimeSpan.FromSeconds(2)))

                Dim cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(role.Id, cache.FirstOrDefault().Id)

                System.Threading.Thread.Sleep(1500)

                cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(role.Id, cache.FirstOrDefault().Id)

                System.Threading.Thread.Sleep(1500)

                cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(role.Id, cache.FirstOrDefault().Id)

                System.Threading.Thread.Sleep(2500)

                cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub NoCacheEmptyResultTest()
            Using db = New TrackerDataContext()
                Dim guid = System.Guid.NewGuid().ToString()
                Dim query = db.Role.Where(Function(r) r.Name = guid)
                Dim key = query.Take(1).GetHashKey()
                Dim role = query.FromCacheFirstOrDefault(New CacheSettings(2) With { _
                 .CacheEmptyResult = False _
                })

                Assert.IsNull(role)

                Dim cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub CacheEmptyResultTest()
            Using db = New TrackerDataContext()
                Dim guid = System.Guid.NewGuid().ToString()
                Dim query = db.Role.Where(Function(r) r.Name = guid)
                Dim key = query.Take(1).GetHashKey()
                Dim role = query.FromCacheFirstOrDefault(New CacheSettings(2))

                Assert.IsNull(role)

                Dim cache = CacheManager.[Get](Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(0, cache.Count())
            End Using
        End Sub
    End Class
End Namespace
