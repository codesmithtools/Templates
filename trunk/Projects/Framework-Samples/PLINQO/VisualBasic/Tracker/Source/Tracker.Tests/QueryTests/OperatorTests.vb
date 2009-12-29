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

            Dim q1 As IQueryable(Of Task) = db.Task.BySummary("Earth", ContainmentOperator.Contains)
            Dim list As List(Of Task) = q1.ToList()
            Assert.IsNotNull(list)

            Dim q2 As IQueryable(Of Task) = db.Task.BySummary("Make", ContainmentOperator.StartsWith)
            list = q2.ToList()
            Assert.IsNotNull(list)

            Dim q3 As IQueryable(Of Task) = db.Task.BySummary("Earth", ContainmentOperator.EndsWith)
            list = q3.ToList()
            Assert.IsNotNull(list)

            Dim q4 As IQueryable(Of Task) = db.Task.BySummary("test", ContainmentOperator.NotContains)
            list = q4.ToList()
            Assert.IsNotNull(list)

            Dim q5 As IQueryable(Of Task) = db.Task.ByDetails(Nothing, ContainmentOperator.Equals)
            list = q5.ToList()
            Assert.IsNotNull(list)

            Dim q6 As IQueryable(Of Task) = db.Task.ByDetails(Nothing, ContainmentOperator.NotEquals)
            list = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ContainmentStartsWith()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q5 As IQueryable(Of Task) = db.Task.ByDetails(Nothing, ContainmentOperator.StartsWith)
            Dim list As List(Of Task) = q5.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ContainmentEndsWith()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByDetails(Nothing, ContainmentOperator.EndsWith)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ContainmentContains()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q8 As IQueryable(Of Task) = db.Task.ByDetails(Nothing, ContainmentOperator.Contains)
            Dim list As List(Of Task) = q8.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        Public Sub ComparisonTest()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q1 As IQueryable(Of Task) = db.Task.ByStartDate(DateTime.Now, ComparisonOperator.GreaterThan)
            Dim list As List(Of Task) = q1.ToList()
            Assert.IsNotNull(list)

            Dim q2 As IQueryable(Of Task) = db.Task.ByStartDate(DateTime.Now, ComparisonOperator.GreaterThanOrEquals)
            list = q2.ToList()
            Assert.IsNotNull(list)

            Dim q3 As IQueryable(Of Task) = db.Task.ByStartDate(DateTime.Now, ComparisonOperator.LessThan)
            list = q3.ToList()
            Assert.IsNotNull(list)

            Dim q4 As IQueryable(Of Task) = db.Task.ByStartDate(DateTime.Now, ComparisonOperator.LessThanOrEquals)
            list = q4.ToList()
            Assert.IsNotNull(list)

            Dim q5 As IQueryable(Of Task) = db.Task.ByStartDate(DateTime.Now, ComparisonOperator.NotEquals)
            list = q5.ToList()
            Assert.IsNotNull(list)

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(Nothing, ComparisonOperator.Equals)
            list = q6.ToList()
            Assert.IsNotNull(list)

            Dim q7 As IQueryable(Of Task) = db.Task.ByStartDate(Nothing, ComparisonOperator.NotEquals)
            list = q7.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonGreaterThan()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(Nothing, ComparisonOperator.GreaterThan)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonGreaterThanOrEquals()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(Nothing, ComparisonOperator.GreaterThanOrEquals)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonLessThan()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(Nothing, ComparisonOperator.LessThan)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub

        <Test()> _
        <ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ComparisonLessThanOrEquals()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim q6 As IQueryable(Of Task) = db.Task.ByStartDate(Nothing, ComparisonOperator.LessThanOrEquals)
            Dim list As List(Of Task) = q6.ToList()
            Assert.IsNotNull(list)
        End Sub
    End Class
End Namespace
