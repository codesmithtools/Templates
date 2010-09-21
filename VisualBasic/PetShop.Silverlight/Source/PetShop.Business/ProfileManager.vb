Imports PetShop.Business

Public Class ProfileManager

#Region "Private Members"

    Private Const _ANONYMOUS_USERNAME As String = "Anonymous"

#End Region

#Region "Instance"

    Public Shared ReadOnly Property Instance() As ProfileManager
        Get
            Return Nested.Current
        End Get
    End Property

    Private Class Nested
        Shared Sub New()
            Current = New ProfileManager()
        End Sub

        ''' <summary>
        ''' Current singleton instance.
        ''' </summary>
        Friend Shared ReadOnly Current As ProfileManager
    End Class

#End Region

    ''' <summary>
    ''' Gets the Anonymous user.
    ''' </summary>
    ''' <returns>The Anonymous user</returns>
    Public Function GetAnonymousUser() As Profile
        Return GetCurrentUser(_ANONYMOUS_USERNAME)
    End Function

    ''' <summary>
    ''' Gets a user by the username.
    ''' </summary>
    ''' <param name="username">the username.</param>
    ''' <returns>The user if it is found, otherwise returns the anonymous user.</returns>
    Public Function GetCurrentUser(ByVal username As String) As Profile
        'Make sure the username is not empty.
        If String.IsNullOrEmpty(username.Trim()) Then
            username = _ANONYMOUS_USERNAME
        End If

        'Get the profile.
        Dim profile As Profile = profile.GetProfile(username)

        'Check to see if the profile exists.
        If String.IsNullOrEmpty(profile.Username) Then
            'If the username is the Anonymous user then create it.
            If username = _ANONYMOUS_USERNAME Then
                Return CreateUser(username, True)
            End If

            Return CreateUser(username, False)
        End If

        'return profile.
        Return profile
    End Function

    ''' <summary>
    ''' Creates a new user.
    ''' </summary>
    ''' <param name="username">The username.</param>
    ''' <param name="isAnonymous">True if the the user anonymous.</param>
    ''' <returns>A newly created user.</returns>
    Public Function CreateUser(ByVal username As String, ByVal isAnonymous As Boolean) As Profile
        Dim profile As Profile = profile.NewProfile()
        profile.Username = username
        profile.ApplicationName = ".NET Pet Shop 4.0"
        profile.IsAnonymous = isAnonymous
        profile.LastActivityDate = DateTime.Now
        profile.LastUpdatedDate = DateTime.Now

        profile = profile.Save()

        Return profile
    End Function

End Class
