Imports System.Collections.Generic
Imports System.Linq
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests

    Public MustInherit Class TestBase
        Protected DueDates As New List(Of DateTime)()
        Protected CreateDates As New List(Of DateTime)()

        Protected UserIds As New List(Of Integer)()
        Protected TaskIds As New List(Of Integer)()

        <TestFixtureSetUp()> _
        Public Sub TestFixtureSetUp()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                Dim users As New List(Of User)()
                users.Add(New User() With { _
                 .FirstName = "Testie McTester", _
                 .LastName = "The First", _
                 .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                 .PasswordSalt = "=Unc%", _
                 .EmailAddress = "one@test.com", _
                 .IsApproved = False _
                })
                users.Add(New User() With { _
                 .FirstName = "Testie McTester", _
                 .LastName = "The Second", _
                 .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                 .PasswordSalt = "=Unc%", _
                 .EmailAddress = "two@test.com", _
                 .IsApproved = False _
                })
                users.Add(New User() With { _
                 .FirstName = "Testie McTester", _
                 .LastName = "The Third", _
                 .PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA", _
                 .PasswordSalt = "=Unc%", _
                 .EmailAddress = "three@test.com", _
                 .IsApproved = False _
                })

                Dim emails As String() = users.[Select](Function(u) u.EmailAddress).ToArray()

                Dim existing As IQueryable(Of User) = From u In db.User _
                 Where emails.Contains(u.EmailAddress) _
                 Select u

                Dim existingUsers As List(Of Integer) = existing.[Select](Function(u) u.Id).ToList()

                Dim testTasks As IQueryable(Of Task) = From t In db.Task _
                 Where existingUsers.Contains(t.CreatedId) OrElse existingUsers.Contains(t.AssignedId.Value) _
                 Select t

                db.Task.Delete(testTasks)
                db.User.Delete(existing)

                db.User.InsertAllOnSubmit(users)
                db.SubmitChanges()

                UserIds = users.[Select](Function(u) u.Id).ToList()
                DueDates.Add(DateTime.Today.AddDays(1))
                DueDates.Add(DateTime.Today.AddDays(2))

                Dim tasks As New List(Of Task)()
                tasks.Add(New Task() With { _
                 .CreatedId = UserIds(0), _
                 .Priority = Priority.Normal, _
                 .Status = Status.NotStarted, _
                 .Summary = "Test" _
                 })
                tasks.Add(New Task() With { _
                 .CreatedId = UserIds(1), _
                 .Priority = Priority.Normal, _
                 .Status = Status.InProgress, _
                 .Summary = "Test", _
                 .AssignedId = UserIds(0), _
                 .DueDate = DueDates(0), _
                 .Details = String.Empty _
                })
                tasks.Add(New Task() With { _
                 .CreatedId = UserIds(2), _
                 .Priority = Priority.High, _
                 .Status = Status.Completed, _
                 .Summary = "Test", _
                 .AssignedId = UserIds(1), _
                 .DueDate = DueDates(1), _
                 .Details = "Hello world!" _
                })
                tasks.Add(New Task() With { _
                 .CreatedId = UserIds(2), _
                 .Priority = Priority.High, _
                 .Status = Status.Completed, _
                 .Summary = "Test", _
                 .AssignedId = UserIds(1), _
                 .Details = "Goodnight moon!" _
                })

                db.Task.InsertAllOnSubmit(tasks)
                db.SubmitChanges()

                TaskIds = tasks.Select(Function(t) t.Id).ToList()
                CreateDates = tasks.Select(Function(t) t.CreatedDate).ToList()
            End Using
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TestFixtureTearDown()
            Using db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
                db.Task.Delete(Function(t) TaskIds.Contains(t.Id))
                db.User.Delete(Function(u) UserIds.Contains(u.Id))
            End Using
        End Sub
    End Class
End Namespace
