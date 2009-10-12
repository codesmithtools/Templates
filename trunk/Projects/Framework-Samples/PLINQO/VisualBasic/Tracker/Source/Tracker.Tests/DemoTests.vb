Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports CodeSmith.Data.Rules
Imports NUnit.Framework
Imports Tracker.Core.Data
Imports System.Data.Linq
Imports CodeSmith.Data.Linq

Namespace Tracker.Tests
    <TestFixture()> _
    Public Class DemoTests

#Region "Setup"
        Private Const STATUS_NOT_STARTED As Integer = 1
        Private Const STATUS_DONE As Integer = 6
        Private Const ROLE_MANAGER As Integer = 2
        Private _SpockId As Integer
        Private Property SpockId() As Integer
            Get
                Return _SpockId
            End Get
            Set(ByVal value As Integer)
                _SpockId = value
            End Set
        End Property
        Private _JamesId As Integer
        Private Property JamesId() As Integer
            Get
                Return _JamesId
            End Get
            Set(ByVal value As Integer)
                _JamesId = value
            End Set
        End Property

        Private _TaskId As Integer
        Private Property TaskId() As Integer
            Get
                Return _TaskId
            End Get
            Set(ByVal value As Integer)
                _TaskId = value
            End Set
        End Property

        <SetUp()> _
        Public Sub Setup()
            TearDown()
            CreateUsers()
            CreateTask(SpockId)
        End Sub

        <TearDown()> _
        Public Sub TearDown()
            Using context = New TrackerDataContext()
                context.Audit.Delete(Function(a) a.User.EmailAddress.EndsWith("startrek.com"))
                context.UserRole.Delete(Function(ur) ur.User.EmailAddress.EndsWith("startrek.com"))
                context.Task.Delete(Function(t) _
                    t.AssignedUser.EmailAddress.EndsWith("startrek.com") OrElse _
                    t.CreatedUser.EmailAddress.EndsWith("startrek.com"))
                context.User.Delete(Function(u) u.EmailAddress.EndsWith("startrek.com"))
            End Using
        End Sub

        Private Sub CreateTask(ByVal userId As Integer)
            Dim task = New Task With { _
                .AssignedId = userId, _
                .Status = Status.InProgress, _
                .Summary = "Explain the visions that inspire you", _
                .Priority = Priority.High, _
                .CreatedId = userId}
            Using context = New TrackerDataContext()
                context.Task.InsertOnSubmit(task)
                context.SubmitChanges()
                TaskId = task.Id
            End Using
        End Sub

        Private Sub CreateUsers()
            Dim user = New User With { _
                    .FirstName = "Spock", _
                    .LastName = "Sarekson", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "spock@startrek.com", _
                    .IsApproved = True, _
                    .LastActivityDate = DateTime.Now}

            Using context = New TrackerDataContext()
                context.User.InsertOnSubmit(user)
                context.SubmitChanges()
                SpockId = user.Id
            End Using

            Dim users = New List(Of User)()

            users.Add(New User With { _
                    .FirstName = "James", _
                    .LastName = "Kirk", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "james.tiberius.kirk@startrek.com", _
                    .IsApproved = True, _
                    .LastActivityDate = DateTime.Now})

            users.Add(New User With { _
                    .FirstName = "Bones", _
                    .LastName = "McCoy", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "bones.mccory@startrek.com", _
                    .IsApproved = False, _
                    .LastActivityDate = DateTime.Now})

            users.Add(New User With { _
                    .FirstName = "Nyota", _
                    .LastName = "Uhura", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "uhura@startrek.com", _
                    .IsApproved = False, _
                    .LastActivityDate = DateTime.Now})

            users.Add(New User With { _
                    .FirstName = "Ellen", _
                    .LastName = "Tigh", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "ellen.tigh@startrek.com", _
                    .IsApproved = False, _
                    .LastActivityDate = DateTime.Now})

            Using context = New TrackerDataContext()
                context.User.InsertAllOnSubmit(users)
                context.SubmitChanges()
                JamesId = context.User.ByFirstName("James").ByLastName("Kirk").First().Id
            End Using
        End Sub

#End Region

        <Test()> _
        Public Sub Test_Manager_And_Query_Gets()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                'IQueryable<Task> tasks = context.Manager.Task.GetByAssignedIdStatusId(SpockId, Status.NotStarted);
                'List<Task> taskList = tasks.ToList();
                Dim task As Task = context.Manager.Task.GetByKey(SpockId)
            End Using

            Using context = New TrackerDataContext()
                context.Log = Console.Out

                Dim task As Task = context.Task.GetByKey(SpockId)
                Dim tasks As IQueryable(Of Task) = context.Task.ByAssignedId(SpockId).ByStatus(Status.NotStarted)
                Dim taskList As List(Of Task) = tasks.ToList()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Many_To_Many()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                Dim u As User = context.User.GetByKey(SpockId)
                Dim r As Role = context.Role.ByName("Manager").First()
                u.RoleList.Add(r)
                context.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Enum()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                Dim task = context.Task.GetByKey(TaskId)
                task.Priority = Priority.High
                context.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Auditing()
            Using context = New TrackerDataContext()
                context.Log = Console.Out
                Dim user = context.User.GetByKey(SpockId)
                user.Comment = "I love my mom, but I hate Winona Ryder."

                Dim task = New Task With { _
                    .AssignedId = SpockId, _
                    .CreatedId = SpockId, _
                    .Status = Status.NotStarted, _
                    .Priority = Priority.High, _
                    .Summary = "Punch Kirk in the face!"}
                context.Task.InsertOnSubmit(task)
                context.SubmitChanges()
                Console.Write(context.LastAudit.ToXml())
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Rules()
            Dim brokenRules As Integer = 0
            Try
                Using context = New TrackerDataContext()
                    context.Log = Console.Out

                    Dim user As New User()
                    user.EmailAddress = "spock@startrek.com"
                    context.User.InsertOnSubmit(user)
                    context.SubmitChanges()
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
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                task = context.Task.GetByKey(TaskId)
                task.Detach()
            End Using

            task.Status = Status.Done

            Using context2 = New TrackerDataContext()
                context2.Log = Console.Out

                context2.Task.Attach(task, True)
                context2.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Entity_Detach_Update()
            Dim task As Task = Nothing
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                task = context.Task.GetByKey(TaskId)
                task.Detach()
            End Using

            Using context2 = New TrackerDataContext()
                context2.Log = Console.Out
                ' attach, then update properties
                context2.Task.Attach(task)
                task.Status = Status.Done

                context2.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Entity_Clone()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                Dim u = context.Task.GetByKey(TaskId)
                Dim taskCopy As Task = u.Clone()
                taskCopy.Id = 0
                context.Task.InsertOnSubmit(taskCopy)
                context.SubmitChanges()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Serialization()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                ' Write to Console/Output ... or break point.

                Dim task = context.Task.GetByKey(TaskId)
                Dim s = task.ToXml()
                Console.Write(s)
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Query_Result_Cache()
            Using context = New TrackerDataContext()
                'By default, the cached results use a one minute sliding expiration with
                'no absolute expiration.
                Dim tasks = context.Task.ByAssignedId(SpockId).FromCache()
                Dim cachedTasks = context.Task.ByAssignedId(SpockId).FromCache()

                'query result is now cached 300 seconds
                Dim approvedUsers = context.User.ByIsApproved(True).FromCache(300)
                Dim cachedApprovedUsers = context.User.ByIsApproved(True).FromCache(300)
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Batch_Update()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                context.Task.Update( _
                    Function(u) u.Status = Status.NotStarted, _
                    Function(u2) New Task With {.Status = Status.Done})

                Dim tasks = From t In context.Task _
                    Where t.Status = Status.Done _
                    Select t
                context.Task.Update(tasks, Function(u) New Task With {.Status = Status.NotStarted})
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Batch_Delete()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                context.User.Delete(JamesId)

                context.User.Delete(Function(u) _
                                        u.IsApproved = False AndAlso _
                                        u.LastName IsNot "McCoy" AndAlso _
                                        u.EmailAddress.EndsWith("startrek.com"))

                Dim usersToDelete As IQueryable(Of User) = From u In context.User _
                    Where u.IsApproved = False AndAlso u.LastName = "McCoy" _
                    Select u
                context.User.Delete(usersToDelete)
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Stored_Procedure_with_Multiple_Results()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                ' Create Procedure [dbo].[GetUsersWithRoles]
                ' As
                ' Select * From [User]
                ' Select * From UserRole
                ' GO

                Dim results = context.GetUsersWithRoles()
                Dim users As List(Of User) = results.GetResult(Of User)().ToList()
                Dim roles As List(Of UserRole) = results.GetResult(Of UserRole)().ToList()
            End Using
        End Sub

        <Test()> _
        Public Sub Test_Batch_Queries()
            Using context = New TrackerDataContext()
                context.Log = Console.Out

                Dim q1 = From u In context.User _
                    Select u
                Dim q2 = From ur In context.UserRole _
                    Select ur
                Dim results As IMultipleResults = context.ExecuteQuery(q1, q2)
                Dim users As List(Of User) = results.GetResult(Of User)().ToList()
                Dim roles As List(Of UserRole) = results.GetResult(Of UserRole)().ToList()
            End Using
        End Sub

    End Class
End Namespace
