Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports CodeSmith.Data.Caching
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class FromCacheTests
        Inherits RoleTests
        <Test()> _
        Public Sub SimpleTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Duck Roll")
                Dim roles = query.FromCache().ToList()

                Dim key = query.GetHashKey()

                Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(roles.Count, cache.Count)
            End Using
        End Sub

        <Test()> _
        Public Sub LongProfile()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Duck Roll")
                Dim key = query.GetHashKey()
                Dim roles = query.FromCache("Long").ToList()

                Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(roles.Count, cache.Count)
            End Using
        End Sub

        <Test()> _
        Public Sub DurationTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.GetHashKey()
                Dim roles = query.FromCache(CacheSettings.FromDuration(2)).ToList()

                Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(roles.Count, cache.Count)

                System.Threading.Thread.Sleep(3000)

                cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub AbsoluteExpirationTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.GetHashKey()
                Dim roles = query.FromCache(New CacheSettings(DateTime.Now.AddSeconds(2))).ToList()

                Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache)
                Assert.AreEqual(roles.Count, cache.Count)

                System.Threading.Thread.Sleep(3000)

                cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub SlidingExpirationTest()
            Dim db = New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
            Dim key = query.GetHashKey()
            Dim roles = query.FromCache(New CacheSettings(TimeSpan.FromSeconds(2))).ToList()

            Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
            Assert.IsNotNull(cache)
            Assert.AreEqual(roles.Count, cache.Count)

            System.Threading.Thread.Sleep(1500)

            cache = CacheManager.Get(Of ICollection(Of Role))(key)
            Assert.IsNotNull(cache)
            Assert.AreEqual(roles.Count, cache.Count)

            System.Threading.Thread.Sleep(1500)

            cache = CacheManager.Get(Of ICollection(Of Role))(key)
            Assert.IsNotNull(cache)
            Assert.AreEqual(roles.Count, cache.Count)

            System.Threading.Thread.Sleep(2500)

            cache = CacheManager.Get(Of ICollection(Of Role))(key)
            Assert.IsNull(cache)

        End Sub

        <Test()> _
        Public Sub NoCacheEmptyResultTest()
            Using db = New TrackerDataContext()
                Dim guid = System.Guid.NewGuid().ToString()
                Dim query = db.Role.Where(Function(r) r.Name = guid)
                Dim key = query.GetHashKey()
                Dim roles = query.FromCache(New CacheSettings(2) With { _
                 .CacheEmptyResult = False _
                })

                Assert.IsNotNull(roles)
                Assert.AreEqual(0, roles.Count())

                Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub CacheEmptyResultTest()
            Dim db = New TrackerDataContext() With { _
              .Log = Console.Out _
            }

            Dim guid = System.Guid.NewGuid().ToString()
            Dim query = db.Role.Where(Function(r) r.Name = guid)
            Dim key = query.GetHashKey()
            Dim roles = query.FromCache(New CacheSettings(2) With { _
             .CacheEmptyResult = True _
            }).ToList()



            Assert.IsNotNull(roles)

            Dim cache = CacheManager.Get(Of ICollection(Of Role))(key)
            Assert.IsNotNull(cache)
            Assert.AreEqual(0, cache.Count)

        End Sub
    End Class
End Namespace
