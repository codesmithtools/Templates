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
    Public Class FromCacheFirstOrDefaultTests
        Inherits RoleTests
        <Test()> _
        Public Sub SimpleTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key As String = query.Take(1).GetHashKey()
                Dim role As Role = query.FromCacheFirstOrDefault()

                Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id)
            End Using
        End Sub

        <Test()> _
        Public Sub DurationTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key As String = query.Take(1).GetHashKey()
                Dim role As Role = query.FromCacheFirstOrDefault(2)

                Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id)

                Thread.Sleep(2500)

                cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub AbsoluteExpirationTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key As String = query.Take(1).GetHashKey()
                Dim role As Role = query.FromCacheFirstOrDefault(2)

                Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id)

                Thread.Sleep(2500)

                cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub SlidingExpirationTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key As String = query.Take(1).GetHashKey()
                Dim role As Role = query.FromCacheFirstOrDefault(New CacheSettings(TimeSpan.FromSeconds(2)))

                Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id)

                Thread.Sleep(1500)

                cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                list = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id)

                Thread.Sleep(1500)

                cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                list = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id)

                Thread.Sleep(2500)

                cache = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub NoCacheEmptyResultTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim guid__1 As String = Guid.NewGuid().ToString()
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = guid__1)
                Dim key As String = query.Take(1).GetHashKey()
                Dim role As Role = query.FromCacheFirstOrDefault(New CacheSettings(2) With { _
                 .CacheEmptyResult = False _
                })

                Assert.IsNull(role)

                Dim cache As Object = HttpRuntime.Cache.[Get](key)
                Assert.IsNull(cache)
            End Using
        End Sub

        <Test()> _
        Public Sub CacheEmptyResultTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim guid__1 As String = Guid.NewGuid().ToString()
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = guid__1)
                Dim key As String = query.Take(1).GetHashKey()
                Dim role As Role = query.FromCacheFirstOrDefault(New CacheSettings(2) With { _
                 .CacheEmptyResult = True _
                })

                Assert.IsNull(role)

                Dim cache As Byte() = TryCast(HttpRuntime.Cache.[Get](key), Byte())
                Assert.IsNotNull(cache)

                Dim list As ICollection(Of Role) = cache.ToCollection(Of Role)()
                Assert.IsNotNull(list)
                Assert.AreEqual(0, list.Count)
            End Using
        End Sub
    End Class
End Namespace
