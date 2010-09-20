Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports CodeSmith.Data.Caching
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class ClearCacheTests
        Inherits RoleTests

        <Test()> _
        Public Sub SimpleTest()
            Using db = New TrackerDataContext()
                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.GetHashKey()
                Dim roles = query.FromCache().ToList()

                Dim runtimeCache = HttpRuntime.Cache.Get(CacheManager.DefaultProvider.GetGroupKey(key))
                Assert.IsNotNull(runtimeCache)

                Dim cache1 = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNotNull(cache1)
                Assert.AreEqual(roles.Count, cache1.Count)

                Dim success As Boolean = query.ClearCache()
                Assert.IsTrue(success)
                success = query.ClearCache()
                Assert.IsFalse(success)

                Dim cache2 = CacheManager.Get(Of ICollection(Of Role))(key)
                Assert.IsNull(cache2)
            End Using
        End Sub

        <Test()> _
        Public Sub SimpleTestWithGroup()
            Using db = New TrackerDataContext()
                CacheManager.InvalidateGroup("group1")

                Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key = query.GetHashKey()
                Dim roles = query.FromCache(CacheManager.GetProfile().WithGroup("group1")).ToList()

                Dim cache1 = CacheManager.Get(Of ICollection(Of Role))(key, "group1")
                Assert.IsNotNull(cache1)
                Assert.AreEqual(roles.Count, cache1.Count)

                query.ClearCache("group1")

                Dim cache2 = CacheManager.Get(Of ICollection(Of Role))(key, "group1")
                Assert.IsNull(cache2)
            End Using
        End Sub
    End Class
End Namespace
