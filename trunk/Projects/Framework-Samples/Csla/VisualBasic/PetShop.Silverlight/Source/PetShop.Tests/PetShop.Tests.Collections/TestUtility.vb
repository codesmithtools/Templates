Imports System
Imports System.Text

Public Class TestUtility
    Private Shared _instance As TestUtility

    'Random class used when generating strings.  Using these classes will
    'guarantee that multiple calls to these functions should return 
    'random values.
    Private ReadOnly _stringRandom As New Random()
    Private ReadOnly _dateRandom As New Random()
    Private ReadOnly _numberRandom As New Random()

    Private ReadOnly _minDate As New DateTime(1990, 1, 1)
    Private ReadOnly _maxDate As New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)

    ''' <summary>
    ''' Private constructor for Singleton
    ''' </summary>
    Private Sub New()

    End Sub

    ''' <summary>
    ''' Get the instance of the TestUtility class
    ''' </summary>
    Public Shared ReadOnly Property Instance() As TestUtility
        Get
            If _instance Is Nothing Then
                _instance = New TestUtility()
            End If

            Return _instance
        End Get
    End Property

    ''' <summary>
    ''' Generates a random number between 0 and int.MaxValue.
    ''' </summary>
    ''' <returns>Random Number</returns>
    Public Function RandomNumber() As Integer
        Return RandomNumber(0, Integer.MaxValue)
    End Function

    ''' <summary>
    ''' Generates a random number between the given bounds.
    ''' </summary>
    ''' <param name="min">lowest bound</param>
    ''' <param name="max">highest bound</param>
    ''' <returns>Random Number</returns>
    Public Function RandomNumber(ByVal min As Integer, ByVal max As Integer) As Integer
        Return _numberRandom.[Next](min, max)
    End Function

    ''' <summary>
    ''' Generates a random string with the given length
    ''' </summary>
    ''' <param name="size">Size of the string</param>
    ''' <param name="lowerCase">If true, generate lowercase string</param>
    ''' <returns>Random string</returns>
    ''' <remarks>Mahesh Chand  - http://www.c-sharpcorner.com/Code/2004/Oct/RandomNumber.asp</remarks>
    Public Function RandomString(ByVal size As Integer, ByVal lowerCase As Boolean) As String
        Dim builder As New StringBuilder()

        For i As Integer = 0 To size - 1
            Dim ch As Char = Convert.ToChar(Convert.ToInt32(26 * _stringRandom.NextDouble() + 65))
            builder.Append(ch)
        Next

        Dim retVal = If(lowerCase, builder.ToString().ToLower(), builder.ToString())

        Return retVal
    End Function

    ''' <summary>
    ''' Returns a random date between the default min and max dates
    ''' </summary>
    ''' <returns></returns>
    Public Function RandomDate() As DateTime
        Return RandomDate(_minDate, _maxDate)
    End Function

    ''' <summary>
    ''' Returns a random date between the dates you pass in
    ''' </summary>
    ''' <param name="minDate">Min date to return</param>
    ''' <param name="maxDate">Max date to return</param>
    ''' <returns></returns>
    Public Function RandomDate(ByVal minDate As DateTime, ByVal maxDate As DateTime) As DateTime
        'Get the total days between the 2 dates
        Dim totalDays As Integer = CInt(maxDate.Subtract(minDate).TotalDays)

        'Pick a random date in between
        Dim randomDays As Integer = _dateRandom.[Next](0, totalDays)

        'Return the random day.
        Return minDate.AddDays(randomDays)
    End Function

    ''' <summary>
    ''' Returns a random date and time between the default datess
    ''' </summary>
    ''' <returns></returns>
    Public Function RandomDateTime() As DateTime
        Return RandomDateTime(_minDate, _maxDate)
    End Function

    ''' <summary>
    ''' Get a random DateTime with a random Time
    ''' </summary>
    ''' <param name="minDate">Min datetime to return</param>
    ''' <param name="maxDate">Max datetime to return</param>
    ''' <returns></returns>
    Public Function RandomDateTime(ByVal minDate As DateTime, ByVal maxDate As DateTime) As DateTime
        'Get the total seconds between the 2 dates
        'Careful of overflow here
        Dim totalSeconds As Integer = CInt(maxDate.Subtract(minDate).TotalSeconds)

        'Pick a random date in between
        Dim randomSeconds As Integer = _dateRandom.[Next](0, totalSeconds)

        'Return the random date.
        Return minDate.AddSeconds(randomSeconds)
    End Function

    ''' <summary>
    ''' Returns a random boolean value
    ''' </summary>
    ''' <returns>Random Boolean</returns>
    Public Function RandomBoolean() As Boolean
        'if the second is odd, return True
        Return ((DateTime.Now.Second Mod 2) > 0)
    End Function

    ''' <summary>
    ''' Returns a random character
    ''' </summary>
    ''' <returns>Random Character</returns>
    Public Function RandomChar() As Char
        Return Convert.ToChar(Convert.ToInt32(26 * _stringRandom.NextDouble() + 65))
    End Function

    ''' <summary>
    ''' Return a random byte between 0 and byte.MaxValue;
    ''' </summary>
    ''' <returns></returns>
    Public Function RandomByte() As Byte
        Return RandomByte(0, Byte.MaxValue)
    End Function

    ''' <summary>
    ''' Return a random byte between the values specified
    ''' </summary>
    ''' <param name="min">Min value</param>
    ''' <param name="max">Max value</param>
    ''' <returns>Random Byte</returns>
    Public Function RandomByte(ByVal min As Byte, ByVal max As Byte) As Byte
        Return CByte(RandomNumber(min, max))
    End Function

    ''' <summary>
    ''' Return a random short between 0 and byte.MaxValue;
    ''' </summary>
    ''' <returns></returns>
    Public Function RandomShort() As Short
        Return RandomShort(0, Short.MaxValue)
    End Function

    ''' <summary>
    ''' Return a random short between the values specified
    ''' </summary>
    ''' <param name="min">Min value</param>
    ''' <param name="max">Max value</param>
    ''' <returns>Random short</returns>
    Public Function RandomShort(ByVal min As Short, ByVal max As Short) As Short
        Return CShort(RandomNumber(min, max))
    End Function
End Class
