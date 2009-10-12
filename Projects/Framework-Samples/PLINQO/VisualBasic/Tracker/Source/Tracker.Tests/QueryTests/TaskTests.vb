Imports System
Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    Public MustInherit Class TaskTests
        Protected dueDates As New List(Of DateTime)()
        Protected createDates As New List(Of DateTime)()
        Protected userIds As List(Of Integer)
        Private _taskIds As New List(Of Integer)()

        <TestFixtureSetUp()> _
        Public Sub TestFixtureSetUp()
            Using db = New TrackerDataContext()
                Dim users = New List(Of User)()
                users.Add(New User With { _
                    .FirstName = "Testie McTester", _
                    .LastName = "The First", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "one@test.com", _
                    .IsApproved = False, _
                    .LastActivityDate = DateTime.Now})
                users.Add(New User() With { _
                    .FirstName = "Testie McTester", _
                    .LastName = "The Second", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "two@test.com", _
                    .IsApproved = False, _
                    .LastActivityDate = DateTime.Now})
                users.Add(New User() With { _
                    .FirstName = "Testie McTester", _
                    .LastName = "The Third", _
                    .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                    .PasswordSalt = "=Unc%", _
                    .EmailAddress = "three@test.com", _
                    .IsApproved = False, _
                    .LastActivityDate = DateTime.Now})
                db.User.InsertAllOnSubmit(users)
                db.SubmitChanges()
                userIds = users.[Select](Function(u) u.Id).ToList()

                dueDates.Add(DateTime.Today.AddDays(1))
                dueDates.Add(DateTime.Today.AddDays(2))

                Dim tasks = New List(Of Task)()
                tasks.Add(New Task With { _
                    .CreatedId = userIds(0), _
                    .Priority = Nothing, _
                    .Status = Status.NotStarted, _
                    .Summary = "Test", _
                    .AssignedId = Nothing, _
                    .DueDate = Nothing, _
                    .Details = Nothing})
                tasks.Add(New Task With { _
                    .CreatedId = userIds(1), _
                    .Priority = Priority.Normal, _
                    .Status = Status.InProgress, _
                    .Summary = "Test", _
                    .AssignedId = userIds(0), _
                    .DueDate = dueDates(0), _
                    .Details = String.Empty})
                tasks.Add(New Task With { _
                    .CreatedId = userIds(2), _
                    .Priority = Priority.High, _
                    .Status = Status.Completed, _
                    .Summary = "Test", _
                    .AssignedId = userIds(1), _
                    .DueDate = dueDates(1), _
                    .Details = "Hello world!"})
                tasks.Add(New Task With { _
                    .CreatedId = userIds(2), _
                    .Priority = Priority.High, _
                    .Status = Status.Completed, _
                    .Summary = "Test", _
                    .AssignedId = userIds(1), _
                    .DueDate = Nothing, _
                    .Details = "Goodnight moon!"})
                db.Task.InsertAllOnSubmit(tasks.Take(1))
                db.SubmitChanges()
                System.Threading.Thread.Sleep(1000)
                db.Task.InsertAllOnSubmit(tasks.Skip(1))
                db.SubmitChanges()

                _taskIds = tasks.[Select](Function(t) t.Id).ToList()
                createDates = tasks.[Select](Function(t) t.CreatedDate).ToList()
            End Using
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TestFixtureTearDown()
            Using db = New TrackerDataContext()
                db.Task.Delete(Function(t) _taskIds.Contains(t.Id))
                db.User.Delete(Function(u) userIds.Contains(u.Id))
            End Using
        End Sub

    End Class
End Namespace
