Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    <TestFixture()> _
    Public Class ClearCacheTests
        Inherits RoleTests
        <Test()> _
        Public Sub SimpleTest()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim query As IQueryable(Of Role) = db.Role.Where(Function(r) r.Name = "Test Role")
                Dim key As String = query.GetHashKey()
                Dim roles As IEnumerable(Of Role) = query.FromCache()

                Dim cache1 As Object = HttpRuntime.Cache.[Get](key)
                Assert.IsNotNull(cache1)

                query.ClearCache()

                Dim cache2 As Object = HttpRuntime.Cache.[Get](key)
                Assert.IsNull(cache2)
            End Using
        End Sub
    End Class
End Namespace
