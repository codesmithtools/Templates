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
            Try
                Using db = New TrackerDataContext()
                    Dim query = db.Role.Where(Function(r) r.Name = "Test Role")
                    Dim key = query.GetKey()
                    Dim roles = query.FromCache()

                    Dim cache1 = HttpRuntime.Cache.[Get](key)
                    Assert.IsNotNull(cache1)
                    Assert.AreSame(roles, cache1)

                    query.ClearCache()

                    Dim cache2 = HttpRuntime.Cache.[Get](key)
                    Assert.IsNull(cache2)
                End Using
            Catch generatedExceptionName As AssertionException
                Throw
            Catch
                Assert.Fail()
            End Try
        End Sub
    End Class
End Namespace
