Imports System.Collections.Generic
Imports System.Data.Linq
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Imports CodeSmith.Data.Linq
Imports NUnit.Framework
Imports Tracker.Core.Data

Namespace Tracker.Tests

    <TestFixture()> _
    Public Class DebugTest
        <Test()> _
        Public Sub FutureLoadWith()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }

            Dim options As New DataLoadOptions()
            options.LoadWith(Of Task)(Function(t) t.CreatedUser)

            db.LoadOptions = options

            Dim q1 As FutureQuery(Of User) = db.User.ByEmailAddress("laura.roslin@battlestar.com").Future()

            Dim q2 As FutureQuery(Of Task) = db.Task.Where(Function(t) t.LastModifiedBy = "laura.roslin@battlestar.com").Future()

            Dim users As List(Of User) = q1.ToList()
            Assert.IsNotNull(users)

            Dim tasks As List(Of Task) = q2.ToList()
            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub ExecuteQueryLoadWith()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
            db.DeferredLoadingEnabled = False
            db.ObjectTrackingEnabled = False

            Dim options As New DataLoadOptions()
            options.LoadWith(Of Task)(Function(t) t.CreatedUser)

            db.LoadOptions = options

            Dim q1 As IQueryable(Of User) = db.User.ByEmailAddress("laura.roslin@battlestar.com")

            Dim q2 As IQueryable(Of Task) = db.Task.Where(Function(t) t.LastModifiedBy = "laura.roslin@battlestar.com")

            Dim result As IMultipleResults = db.ExecuteQuery(q1, q2)

            Assert.IsNotNull(result)

            Dim userResult As IEnumerable(Of User) = result.GetResult(Of User)()
            Assert.IsNotNull(userResult)

            Dim users As List(Of User) = userResult.ToList()
            Assert.IsNotNull(users)

            Dim taskResult As IEnumerable(Of Task) = result.GetResult(Of Task)()
            Assert.IsNotNull(taskResult)

            Dim tasks As List(Of Task) = taskResult.ToList()
            Assert.IsNotNull(tasks)
        End Sub

        <Test()> _
        Public Sub Reflection()
            Dim db As New TrackerDataContext() With { _
             .Log = Console.Out _
            }
            Dim source As IQueryable = db.Task.Where(Function(t) t.LastModifiedBy = "laura.roslin@battlestar.com").OrderBy(Function(t) t.CreatedId)


            ' can be static
            Dim queryableType As Type = GetType(Queryable)
            ' can be static
            Dim countMethod As MethodInfo = (From m In queryableType.GetMethods(BindingFlags.[Static] Or BindingFlags.[Public]) _
             Where m.Name = "Count" AndAlso m.IsGenericMethod AndAlso m.GetParameters().Length = 1 _
             Select m).FirstOrDefault()


            Assert.IsNotNull(countMethod)

            Dim genericMethod As MethodInfo = countMethod.MakeGenericMethod(New Type() {source.ElementType})
            Dim expression__1 As MethodCallExpression = Expression.[Call](Nothing, genericMethod, source.Expression)

            Assert.IsNotNull(expression__1)

            Dim dataContextType As Type = db.[GetType]()
            Dim providerProperty As PropertyInfo = dataContextType.GetProperty("Provider", BindingFlags.Instance Or BindingFlags.NonPublic)

            Dim provider As Object = providerProperty.GetValue(db, Nothing)

            Dim providerType As Type = provider.[GetType]().GetInterface("IProvider")
            Dim getCommandMethod As MethodInfo = providerType.GetMethod("GetCommand", BindingFlags.Instance Or BindingFlags.[Public])

            Dim commandObject As Object = getCommandMethod.Invoke(provider, New Object() {expression__1})

            Assert.IsNotNull(commandObject)
        End Sub
    End Class
End Namespace
