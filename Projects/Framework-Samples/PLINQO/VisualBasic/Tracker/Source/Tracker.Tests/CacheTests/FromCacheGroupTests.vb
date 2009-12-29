Imports System.Collections.Generic
Imports System.Linq
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
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim cache As CacheSettings = CacheManager.GetProfile().WithGroup("Role")

            Dim roles As IEnumerable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role").FromCache(cache)
            Dim role As Role = db.Role.ByName("Duck Roll").FromCacheFirstOrDefault(cache)

            Assert.IsNotNull(roles)
            Assert.IsNotNull(role)

            Dim roles2 As IEnumerable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role").FromCache(cache)
            Dim role2 As Role = db.Role.ByName("Duck Roll").FromCacheFirstOrDefault(cache)

            Assert.IsNotNull(roles2)
            Assert.IsNotNull(role2)

            ' some update expire tag
            CacheManager.GetProvider().InvalidateGroup("Role")

            Dim roles3 As IEnumerable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role").FromCache(cache)
            Dim role3 As Role = db.Role.ByName("Duck Roll").FromCacheFirstOrDefault(cache)

            Assert.IsNotNull(roles3)
            Assert.IsNotNull(role3)
        End Sub
    End Class
End Namespace
