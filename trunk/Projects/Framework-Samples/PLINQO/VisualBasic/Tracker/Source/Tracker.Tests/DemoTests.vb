Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports CodeSmith.Data.Linq
Imports CodeSmith.Data.Rules
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests


    <TestFixture()> _
    Public Class DemoTests
#Region "Setup"

        Private Const STATUS_NOT_STARTED As Integer = 1
        Private Const STATUS_DONE As Integer = 6
        Private Const ROLE_MANAGER As Integer = 2
        Private Property SpockId() As Integer
            Get
                Return m_SpockId
            End Get
            Set(ByVal value As Integer)
                m_SpockId = value
            End Set
        End Property
        Private m_SpockId As Integer
        Private Property JamesId() As Integer
            Get
                Return m_JamesId
            End Get
            Set(ByVal value As Integer)
                m_JamesId = value
            End Set
        End Property
        Private m_JamesId As Integer

        Private Property TaskId() As Integer
            Get
                Return m_TaskId
            End Get
            Set(ByVal value As Integer)
                m_TaskId = value
            End Set
        End Property
        Private m_TaskId As Integer

        <TestFixtureSetUp()> _
        Public Sub Setup()
            TearDown()
            CreateUsers()
            CreateTask(SpockId)
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TearDown()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Audit.Delete(Function(a) a.User.EmailAddress.EndsWith("startrek.com"))
                db.UserRole.Delete(Function(ur) ur.User.EmailAddress.EndsWith("startrek.com"))
                db.Task.Delete(Function(t) t.AssignedUser.EmailAddress.EndsWith("startrek.com") OrElse t.CreatedUser.EmailAddress.EndsWith("startrek.com"))
                db.User.Delete(Function(u) u.EmailAddress.EndsWith("startrek.com"))
            End Using
        End Sub

        Private Sub CreateTask(ByVal userId As Integer)
            Dim task As New Task() With { _
             .AssignedId = userId, _
             .Status = Status.InProgress, _
             .Summary = "Explain the visions that inspire you", _
             .Priority = Priority.High, _
             .CreatedId = userId _
            }
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Task.InsertOnSubmit(task)
                db.SubmitChanges()
                TaskId = task.Id
            End Using
        End Sub

        Private Sub CreateUsers()
            Dim user As New User() With { _
             .FirstName = "Spock", _
             .LastName = "Sarekson", _
             .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
             .PasswordSalt = "=Unc%", _
             .EmailAddress = "spock@startrek.com", _
             .IsApproved = True _
            }

            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.User.InsertOnSubmit(user)
                db.SubmitChanges()
                SpockId = user.Id
            End Using

            Dim users As New List(Of User)()

            users.Add(New User() With { _
             .FirstName = "James", _
             .LastName = "Kirk", _
             .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
             .PasswordSalt = "=Unc%", _
             .EmailAddress = "james.tiberius.kirk@startrek.com", _
             .IsApproved = True _
            })

            users.Add(New User() With { _
             .FirstName = "Bones", _
             .LastName = "McCoy", _
             .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
             .PasswordSalt = "=Unc%", _
             .EmailAddress = "bones.mccory@startrek.com", _
             .IsApproved = False _
            })

            users.Add(New User() With { _
             .FirstName = "Nyota", _
             .LastName = "Uhura", _
             .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
             .PasswordSalt = "=Unc%", _
             .EmailAddress = "uhura@startrek.com", _
             .IsApproved = False _
            })

            users.Add(New User() With { _
             .FirstName = "Ellen", _
             .LastName = "Tigh", _
             .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
             .PasswordSalt = "=Unc%", _
             .EmailAddress = "ellen.tigh@startrek.com", _
             .IsApproved = False _
            })

            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.User.InsertAllOnSubmit(users)
                db.SubmitChanges()
                JamesId = db.User.ByFirstName("James").ByLastName("Kirk").First().Id
            End Using
        End Sub

#End Region

        <Test()> _
        Public Sub Test_Manager_And_Query_Gets()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim task As Task = db.Manager.Task.GetByKey(SpockId)
                Dim tasks As IQueryable(Of Task) = db.Manager.Task.GetByAssignedIdStatus(SpockId, Status.InProgress)
                Dim taskList As List(Of Task) = tasks.ToList()
            End Using

            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim task As Task = db.Task.GetByKey(SpockId)
                Dim tasks As IQueryable(Of Task) = db.Task.ByAssignedId(SpockId).ByStatus(Status.InProgress)
                Dim taskList As List(Of Task) = tasks.ToList()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Query_Advanced()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim priorites As List(Of Task) = db.Task.Where(Function(t) Not t.Priority.HasValue OrElse t.Priority.Value = Priority.High).ToList()
                Dim something As List(Of Task) = db.Task.ByPriority(Nothing, Priority.High).ToList()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Many_To_Many()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim u As User = db.User.GetByKey(SpockId)
                Dim r As Role = db.Role.ByName("Manager").First()
                u.RoleList.Add(r)
                db.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Enum()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim task As Task = db.Task.GetByKey(TaskId)
                task.Priority = Priority.High
                db.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Auditing()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim user As User = db.User.GetByKey(SpockId)
                user.Comment = "I love my mom, but I hate Winona Ryder."

                Dim task As New Task() With { _
                 .AssignedId = SpockId, _
                 .CreatedId = SpockId, _
                 .Status = Status.NotStarted, _
                 .Priority = Priority.High, _
                 .Summary = "Punch Kirk in the face!" _
                }
                db.Task.InsertOnSubmit(task)
                db.SubmitChanges()
                Console.Write(db.LastAudit.ToXml())
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Rules()
            Dim brokenRules As Integer = 0
            Try
                Using db As New TrackerDataContext() With { _
                 .Log = Console.Out _
                }
                    Dim user As New User()
                    user.EmailAddress = "spock@startrek.com"
                    db.User.InsertOnSubmit(user)
                    db.SubmitChanges()
                End Using
            Catch e As BrokenRuleException
                brokenRules = e.BrokenRules.Count
                Console.Write(e.ToString())
            End Try
            Assert.AreEqual(brokenRules, 2)
        End Sub

        <Test()> _
        Public Sub Test_Entity_Detach()
            Dim task As Task = Nothing
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                task = db.Task.GetByKey(TaskId)
                task.Detach()
            End Using

            task.Status = Status.Done

            Using context2 As New TrackerDataContext()
                context2.Log = Console.Out

                context2.Task.Attach(task, True)
                context2.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Entity_Detach_Update()
            Dim task As Task = Nothing
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                task = db.Task.GetByKey(TaskId)
                task.Detach()
            End Using

            Using context2 As New TrackerDataContext()
                context2.Log = Console.Out
                ' attach, then update properties
                context2.Task.Attach(task)
                task.Status = Status.Done

                context2.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Entity_Clone()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim u As Task = db.Task.GetByKey(TaskId)
                Dim taskCopy As Task = u.Clone()
                taskCopy.Id = 0
                db.Task.InsertOnSubmit(taskCopy)
                db.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Serialization()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                ' Write to Console/Output ... or break point.

                Dim task As Task = db.Task.GetByKey(TaskId)
                Dim s As String = task.ToXml()
                Console.Write(s)
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Query_Result_Cache()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                'By default, the cached results use a one minute sliding expiration with
                'no absolute expiration.
                Dim tasks As IEnumerable(Of Task) = db.Task.ByAssignedId(SpockId).FromCache()
                Dim cachedTasks As IEnumerable(Of Task) = db.Task.ByAssignedId(SpockId).FromCache()

                'query result is now cached 300 seconds
                Dim approvedUsers As IEnumerable(Of User) = db.User.ByIsApproved(True).FromCache(300)
                Dim cachedApprovedUsers As IEnumerable(Of User) = db.User.ByIsApproved(True).FromCache(300)
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Batch_Update()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Task.Update(Function(u) u.Status = Status.NotStarted, Function(u2) New Task() With { _
        .Status = Status.Done _
       })

                Dim tasks As IQueryable(Of Task) = From t In db.Task _
                 Where t.Status = Status.Done _
                 Select t
                db.Task.Update(tasks, Function(u) New Task() With { _
        .Status = Status.NotStarted _
       })
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Batch_Delete()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.User.Delete(JamesId)

                db.User.Delete(Function(u) u.IsApproved = False AndAlso u.LastName <> "McCoy" AndAlso u.EmailAddress.EndsWith("startrek.com"))

                Dim usersToDelete As IQueryable(Of User) = From u In db.User _
                 Where u.IsApproved = False AndAlso u.LastName = "McCoy" _
                 Select u
                db.User.Delete(usersToDelete)
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Stored_Procedure_with_Multiple_Results()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                ' Create Procedure [dbo].[GetUsersWithRoles]
                ' As
                ' Select * From [User]
                ' Select * From UserRole
                ' GO

                Dim results As IMultipleResults = db.GetUsersWithRoles()
                Dim users As List(Of User) = results.GetResult(Of User)().ToList()
                Dim roles As List(Of UserRole) = results.GetResult(Of UserRole)().ToList()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Batch_Queries()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim q1 As IQueryable(Of User) = From u In db.User _
                 Select u
                Dim q2 As IQueryable(Of UserRole) = From ur In db.UserRole _
                 Select ur
                Dim results As IMultipleResults = db.ExecuteQuery(q1, q2)
                Dim users As List(Of User) = results.GetResult(Of User)().ToList()
                Dim roles As List(Of UserRole) = results.GetResult(Of UserRole)().ToList()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_ToPagedList()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim q1 As IOrderedQueryable(Of User) = From u In db.User _
                 Order By u.EmailAddress _
                 Select u

                Dim users As PagedList(Of User) = q1.ToPagedList(0, 5)

                Assert.IsNotNull(users)
                Assert.AreEqual(5, users.Count)
            End Using
        End Sub
    End Class
End Namespace
