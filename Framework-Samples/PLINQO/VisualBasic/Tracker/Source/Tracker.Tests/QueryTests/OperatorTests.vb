Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests.QueryTests
    <TestFixture()> _
    Public Class OperatorTests
        Inherits TestBase
        <Test()> _
        Public Sub Contains()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim assignedId As New List(Of System.Nullable(Of Integer))()
            assignedId.Add(Nothing)
            assignedId.Add(1)

            ' null and int
            Dim q1 As IQueryable(Of Task) = db.Task.ByAssignedId(assignedId)

            Dim assignedList As List(Of Task) = q1.ToList()
            Assert.IsNotNull(assignedList)


            Dim details As New List(Of String)()
            details.Add(Nothing)
            details.Add("test")

            ' null and string
            Dim q2 As IQueryable(Of Task) = db.Task.ByDetails(details)
            Dim detailList As List(Of Task) = q2.ToList()
            Assert.IsNotNull(detailList)

            ' null and int direct
            Dim q3 As IQueryable(Of Task) = db.Task.ByAssignedId(Nothing, 1)

            Dim assignedList2 As List(Of Task) = q3.ToList()
            Assert.IsNotNull(assignedList2)

            ' string, null direct
            Dim q4 As IQueryable(Of Task) = db.Task.ByDetails("test", Nothing, "other")
            Dim detailList2 As List(Of Task) = q4.ToList()
            Assert.IsNotNull(detailList2)

            ' should use IN operator
            Dim q5 As IQueryable(Of Task) = db.Task.ByCreatedId(1, 2)
            Dim createdList As List(Of Task) = q5.ToList()
            Assert.IsNotNull(createdList)
        End Sub

        <Test()> _
        Public Sub ContainmentTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q1 As IQueryable(Of Task) = db.Task.BySummary(ContainmentOperator.Contains, "Earth")
            Dim list As List(Of Task) = q1.ToList()
            Assert.IsNotNull(list)

            Dim q2 As IQueryable(Of Task) = db.Task.BySummary(ContainmentOperator.StartsWith, "Make")
            list = q2.ToList()
            Assert.IsNotNull(list)

            Dim q3 As IQueryable(Of Task) = db.Task.BySummary(ContainmentOperator.EndsWith, "Earth")
            list = q3.ToList()
            Assert.IsNotNull(list)

            Dim q4 As IQueryable(Of Task) = db.Task.BySummary(ContainmentOperator.NotContains, "test")
            list = q4.ToList()
            Assert.IsNotNull(list)

            Dim q5 As IQueryable(Of Task) = db.Task.ByDetails(ContainmentOperator.Equals, Nothing)
            list = q5.ToList()
            Assert.IsNotNull(list)

            Dim q6 As IQueryable(Of Task) = db.Task.ByDetails(ContainmentOperator.NotEquals, Nothing)
            list = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ContainmentStartsWith()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q5 As IQueryable(Of Task) = db.Task.ByDetails(ContainmentOperator.StartsWith, Nothing)
            Dim list As List(Of Task) = q5.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ContainmentEndsWith()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByDetails(ContainmentOperator.EndsWith, Nothing)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ContainmentContains()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q8 As IQueryable(Of Task) = db.Task.ByDetails(ContainmentOperator.Contains, Nothing)
            Dim list As List(Of Task) = q8.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        Public Sub ComparisonTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q1 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.GreaterThan, DateTime.Now)
            Dim list As List(Of Task) = q1.ToList()
            Assert.IsNotNull(list)

            Dim q2 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.GreaterThanOrEquals, DateTime.Now)
            list = q2.ToList()
            Assert.IsNotNull(list)

            Dim q3 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.LessThan, DateTime.Now)
            list = q3.ToList()
            Assert.IsNotNull(list)

            Dim q4 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.LessThanOrEquals, DateTime.Now)
            list = q4.ToList()
            Assert.IsNotNull(list)

            Dim q5 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.NotEquals, DateTime.Now)
            list = q5.ToList()
            Assert.IsNotNull(list)

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.Equals, Nothing)
            list = q6.ToList()
            Assert.IsNotNull(list)

            Dim q7 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.NotEquals, Nothing)
            list = q7.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonGreaterThan()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.GreaterThan, Nothing)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonGreaterThanOrEquals()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.GreaterThanOrEquals, Nothing)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonLessThan()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.LessThan, Nothing)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonLessThanOrEquals()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(ComparisonOperator.LessThanOrEquals, Nothing)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub
    End Class
End Namespace
