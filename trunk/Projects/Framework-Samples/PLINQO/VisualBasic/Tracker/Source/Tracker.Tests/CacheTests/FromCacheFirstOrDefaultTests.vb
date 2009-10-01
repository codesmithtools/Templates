Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class FromCacheFirstOrDefaultTests
        Inherits RoleTests
        <Test()> _
        Public Sub SimpleTest()
            Try
                Using db = New TrackerDataContext()
                    Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                    Dim key = query.Take(1).GetKey()
                    Dim role = query.FromCacheFirstOrDefault()

                    Dim cache = TryCast(HttpRuntime.Cache.[Get](key), List(Of Role))
                    Assert.IsNotNull(cache)
                    Assert.AreSame(role, cache.FirstOrDefault())
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub

        <Test()> _
        Public Sub DurationTest()
            Try
                Using db = New TrackerDataContext()
                    Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                    Dim key = query.Take(1).GetKey()
                    Dim role = query.FromCacheFirstOrDefault(2)

                    Dim cache1 = TryCast(HttpRuntime.Cache.[Get](key), List(Of Role))
                    Assert.IsNotNull(cache1)
                    Assert.AreSame(role, cache1.FirstOrDefault())

                    System.Threading.Thread.Sleep(2500)

                    Dim cache2 = HttpRuntime.Cache.[Get](key)
                    Assert.IsNull(cache2)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub

        <Test()> _
        Public Sub AbsoluteExpirationTest()
            Try
                Using db = New TrackerDataContext()
                    Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                    Dim key = query.Take(1).GetKey()
                    Dim role = query.FromCacheFirstOrDefault(DateTime.UtcNow.AddSeconds(2))

                    Dim cache1 = TryCast(HttpRuntime.Cache.[Get](key), List(Of Role))
                    Assert.IsNotNull(cache1)
                    Assert.AreSame(role, cache1.FirstOrDefault())

                    System.Threading.Thread.Sleep(2500)

                    Dim cache2 = HttpRuntime.Cache.[Get](key)
                    Assert.IsNull(cache2)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub

        <Test()> _
        Public Sub SlidingExpirationTest()
            Try
                Using db = New TrackerDataContext()
                    Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                    Dim key = query.Take(1).GetKey()
                    Dim role = query.FromCacheFirstOrDefault(New TimeSpan(0, 0, 2))

                    Dim cache1 = TryCast(HttpRuntime.Cache.[Get](key), List(Of Role))
                    Assert.IsNotNull(cache1)
                    Assert.AreSame(role, cache1.FirstOrDefault())

                    System.Threading.Thread.Sleep(1500)

                    Dim cache2 = TryCast(HttpRuntime.Cache.[Get](key), List(Of Role))
                    Assert.IsNotNull(cache2)
                    Assert.AreSame(role, cache2.FirstOrDefault())

                    System.Threading.Thread.Sleep(1500)

                    Dim cache3 = TryCast(HttpRuntime.Cache.[Get](key), List(Of Role))
                    Assert.IsNotNull(cache3)
                    Assert.AreSame(role, cache3.FirstOrDefault())

                    System.Threading.Thread.Sleep(2500)

                    Dim cache4 = HttpRuntime.Cache.[Get](key)
                    Assert.IsNull(cache4)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub

        <Test()> _
        Public Sub CacheEmptyResultTest()
            Try
                Using db = New TrackerDataContext()
                    Dim query = db.Role.Where(Function(r) r.Name = System.Guid.NewGuid().ToString())
                    Dim key = query.Take(1).GetKey()
                    Dim role = query.FromCacheFirstOrDefault(New CacheSettings())

                    Assert.IsNull(role)

                    Dim cache = HttpRuntime.Cache.[Get](key)
                    Assert.IsNull(cache)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub
    End Class
End Namespace
