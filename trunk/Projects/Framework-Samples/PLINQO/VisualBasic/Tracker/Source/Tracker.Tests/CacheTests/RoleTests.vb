Imports System.Collections
Imports System.Collections.Generic
Imports System.Web
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.CacheTests
    Public MustInherit Class RoleTests
        Private ReadOnly _roleIds As New List(Of Integer)()

        <TestFixtureSetUp()> _
        Public Sub TestFixtureSetUp()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim role1 As New Role() With { _
                 .Name = "Test Role" _
                }
                Dim role2 As New Role() With { _
                 .Name = "Duck Roll" _
                }

                db.Role.InsertOnSubmit(role1)
                db.Role.InsertOnSubmit(role2)
                db.SubmitChanges()

                _roleIds.Add(role1.Id)
                _roleIds.Add(role2.Id)
            End Using
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TestFixtureTearDown()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Role.Delete(Function(r) _roleIds.Contains(r.Id))
            End Using
        End Sub

        <SetUp()> _
        Public Sub SetUp()
            Dim keys As New List(Of String)()

            Dim enumerator As IDictionaryEnumerator = HttpRuntime.Cache.GetEnumerator()
            While enumerator.MoveNext()
                keys.Add(TryCast(enumerator.Key, String))
            End While

            keys.ForEach(Function(k) HttpRuntime.Cache.Remove(k))
        End Sub

        <TearDown()> _
        Public Sub TearDown()
        End Sub
    End Class
End Namespace
