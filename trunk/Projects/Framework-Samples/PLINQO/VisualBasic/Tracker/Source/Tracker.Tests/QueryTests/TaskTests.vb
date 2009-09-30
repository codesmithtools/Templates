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
                dueDates.Add(DateTime.Today.AddDays(1))
                dueDates.Add(DateTime.Today.AddDays(2))

                userIds = db.User.[Select](Function(u) u.Id).Take(3).ToList()

                Dim task1 = New Task()
                task1.CreatedId = userIds(0)
                task1.Priority = Nothing
                task1.Status = Status.NotStarted
                task1.Summary = "Test"
                task1.AssignedId = Nothing
                task1.DueDate = Nothing
                task1.Details = Nothing

                Dim task2 = New Task()
                task2.CreatedId = userIds(1)
                task2.Priority = Priority.Normal
                task2.Status = Status.InProgress
                task2.Summary = "Test"
                task2.AssignedId = userIds(0)
                task2.DueDate = dueDates(0)
                task2.Details = String.Empty

                Dim task3 = New Task()
                task3.CreatedId = userIds(2)
                task3.Priority = Priority.High
                task3.Status = Status.Completed
                task3.Summary = "Test"
                task3.AssignedId = userIds(1)
                task3.DueDate = dueDates(1)
                task3.Details = "Hello world!"

                Dim task4 = New Task()
                task4.CreatedId = userIds(2)
                task4.Priority = Priority.High
                task4.Status = Status.Completed
                task4.Summary = "Test"
                task4.AssignedId = userIds(1)
                task4.DueDate = Nothing
                task4.Details = "Goodnight moon!"

                db.Task.InsertOnSubmit(task1)
                db.SubmitChanges()
                System.Threading.Thread.Sleep(1000)
                db.Task.InsertOnSubmit(task2)
                db.Task.InsertOnSubmit(task3)
                db.SubmitChanges()

                _taskIds.Add(task1.Id)
                _taskIds.Add(task2.Id)
                _taskIds.Add(task3.Id)
                _taskIds.Add(task4.Id)
                createDates.Add(task1.CreatedDate)
                createDates.Add(task2.CreatedDate)
                createDates.Add(task3.CreatedDate)
                createDates.Add(task4.CreatedDate)
            End Using
        End Sub

        <TestFixtureTearDown()> _
        Public Sub TestFixtureTearDown()
            Using db = New TrackerDataContext()
                db.Task.Delete(Function(t) _taskIds.Contains(t.Id))
            End Using
        End Sub

    End Class
End Namespace
