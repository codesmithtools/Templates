Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Web
Imports CodeSmith.Data.Caching
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class FromCacheGroupTests
        Inherits RoleTests

        <Test()> _
        Public Sub TagTest()
            Dim db = New TrackerDataContext() With { _
                .Log = Console.Out _
            }

            Dim cache = CacheManager.GetProfile().WithGroup("Role")

            Dim roles = db.Role.Where(Function(r) r.Name = "Test Role").FromCache(cache)
            Dim role = db.Role.ByName("Duck Roll").FromCacheFirstOrDefault(cache)

            Assert.IsNotNull(roles)
            Assert.IsNotNull(role)

            Dim roles2 = db.Role.Where(Function(r) r.Name = "Test Role").FromCache(cache)
            Dim role2 = db.Role.ByName("Duck Roll").FromCacheFirstOrDefault(cache)

            Assert.IsNotNull(roles2)
            Assert.IsNotNull(role2)

            ' some update expire tag
            CacheManager.InvalidateGroup("Role")

            Dim roles3 = db.Role.Where(Function(r) r.Name = "Test Role").FromCache(cache)
            Dim role3 = db.Role.ByName("Duck Roll").FromCacheFirstOrDefault(cache)

            Assert.IsNotNull(roles3)
            Assert.IsNotNull(role3)
        End Sub
    End Class
End Namespace
