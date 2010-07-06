Imports System.Web.Profile
Imports System.Configuration

Imports PetShop.Business

Public NotInheritable Class PetShopProfileProvider
    Inherits ProfileProvider

#Region "Private Memebers"

    Private Const _ERR_INVALID_PARAMETER As String = "Invalid Profile parameter:"
    Private Const _PROFILE_ACCOUNT As String = "Account"
    Private Const _PROFILE_SHOPPINGCART As String = "ShoppingCart"
    Private Const _PROFILE_WISHLIST As String = "WishList"
    Private Shared _applicationName As String = ".NET Pet Shop 4.0"
#End Region

#Region "Overrides of SettingsProvider"

    ''' <summary>
    ''' Gets or sets the name of the currently running application.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.String"/> that contains the application's shortened name, which does not contain a full path or extension, for example, SimpleAppSettings.
    ''' </returns>
    ''' <filterpriority>2</filterpriority>
    Public Overloads Overrides Property ApplicationName() As String
        Get
            Return _applicationName
        End Get
        Set(ByVal value As String)
            _applicationName = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the collection of settings property values for the specified application instance and settings property group.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.Configuration.SettingsPropertyValueCollection"/> containing the values for the specified settings property group.
    ''' </returns>
    ''' <param name="context">A <see cref="T:System.Configuration.SettingsContext"/> describing the current application use.
    '''                 </param><param name="collection">A <see cref="T:System.Configuration.SettingsPropertyCollection"/> containing the settings property group whose values are to be retrieved.
    '''                 </param><filterpriority>2</filterpriority>
    Public Overloads Overrides Function GetPropertyValues(ByVal context As SettingsContext, ByVal collection As SettingsPropertyCollection) As SettingsPropertyValueCollection
        Dim username As String = DirectCast(context("UserName"), String)
        Dim isAuthenticated As Boolean = DirectCast(context("IsAuthenticated"), Boolean)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(username)

        Dim svc As New SettingsPropertyValueCollection

        For Each prop As SettingsProperty In collection
            Dim pv As New SettingsPropertyValue(prop)

            Select Case pv.[Property].Name
                Case _PROFILE_SHOPPINGCART
                    pv.PropertyValue = CartList.GetCart(profile.UniqueID, True)
                    Exit Select
                Case _PROFILE_WISHLIST
                    pv.PropertyValue = CartList.GetCart(profile.UniqueID, False)
                    Exit Select
                Case _PROFILE_ACCOUNT
                    If isAuthenticated Then
                        pv.PropertyValue = New Address(profile)
                    End If
                    Exit Select
                Case Else
                    Throw New ApplicationException(String.Format("{0} name.", _ERR_INVALID_PARAMETER))
            End Select

            svc.Add(pv)
        Next
        Return svc
    End Function

    ''' <summary>
    ''' Sets the values of the specified group of property settings.
    ''' </summary>
    ''' <param name="context">A <see cref="T:System.Configuration.SettingsContext"/> describing the current application usage.
    '''                 </param><param name="collection">A <see cref="T:System.Configuration.SettingsPropertyValueCollection"/> representing the group of property settings to set.
    '''                 </param><filterpriority>2</filterpriority>
    Public Overloads Overrides Sub SetPropertyValues(ByVal context As SettingsContext, ByVal collection As SettingsPropertyValueCollection)
        Dim username As String = DirectCast(context("UserName"), String)
        If String.IsNullOrEmpty(username) OrElse username.Length > 256 OrElse username.IndexOf(",") > 0 Then
            Throw New ApplicationException(String.Format("{0} user name.", _ERR_INVALID_PARAMETER))
        End If

        Dim isAuthenticated As Boolean = DirectCast(context("IsAuthenticated"), Boolean)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(username)

        For Each pv As SettingsPropertyValue In collection
            If pv.PropertyValue <> Nothing Then
                Select Case pv.[Property].Name
                    Case _PROFILE_SHOPPINGCART
                        profile.ShoppingCart.Add(DirectCast(pv.PropertyValue, Cart))
                        Exit Select
                    Case _PROFILE_WISHLIST
                        profile.WishList.Add(DirectCast(pv.PropertyValue, Cart))
                        Exit Select
                    Case _PROFILE_ACCOUNT
                        If isAuthenticated Then
                            If profile.Accounts.Count > 0 Then
                                Dim account As Account = profile.Accounts(0)
                                UpdateAccount(account, DirectCast(pv.PropertyValue, Address))
                            Else
                                Dim account As Account = profile.Accounts.AddNew()
                                account.UniqueID = profile.UniqueID

                                UpdateAccount(account, DirectCast(pv.PropertyValue, Address))
                            End If
                        End If

                        Exit Select
                    Case Else
                        Throw New ApplicationException(_ERR_INVALID_PARAMETER + " name.")
                End Select
            End If
        Next

        profile.LastActivityDate = DateTime.Now
        profile.LastUpdatedDate = DateTime.Now

        profile = profile.Save(True)
    End Sub
#End Region

#Region "Overrides of ProfileProvider"

    ''' <summary>
    ''' When overridden in a derived class, deletes profile properties and information for the supplied list of profiles.
    ''' </summary>
    ''' <returns>
    ''' The number of profiles deleted from the data source.
    ''' </returns>
    ''' <param name="profiles">A <see cref="T:System.Web.Profile.ProfileInfoCollection"/>  of information about profiles that are to be deleted.
    '''                 </param>
    Public Overloads Overrides Function DeleteProfiles(ByVal profiles As ProfileInfoCollection) As Integer
        Dim deleteCount As Integer = 0

        For Each p As ProfileInfo In profiles
            If DeleteProfile(p.UserName) Then
                System.Math.Max(System.Threading.Interlocked.Increment(deleteCount), deleteCount - 1)
            End If
        Next

        Return deleteCount
    End Function

    ''' <summary>
    ''' When overridden in a derived class, deletes profile properties and information for profiles that match the supplied list of user names.
    ''' </summary>
    ''' <returns>
    ''' The number of profiles deleted from the data source.
    ''' </returns>
    ''' <param name="usernames">A string array of user names for profiles to be deleted.
    '''                 </param>
    Public Overloads Overrides Function DeleteProfiles(ByVal usernames As String()) As Integer
        Dim deleteCount As Integer = 0

        For Each user As String In usernames
            If DeleteProfile(user) Then
                System.Math.Max(System.Threading.Interlocked.Increment(deleteCount), deleteCount - 1)
            End If
        Next

        Return deleteCount
    End Function

    ''' <summary>
    ''' When overridden in a derived class, deletes all user-profile data for profiles in which the last activity date occurred before the specified date.
    ''' </summary>
    ''' <returns>
    ''' The number of profiles deleted from the data source.
    ''' </returns>
    ''' <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are deleted.
    '''                 </param><param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which user profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  value of a user profile occurs on or before this date and time, the profile is considered inactive.
    '''                 </param>
    Public Overloads Overrides Function DeleteInactiveProfiles(ByVal authenticationOption As ProfileAuthenticationOption, ByVal userInactiveSinceDate As DateTime) As Integer
        Dim count As Integer = 0
        For Each profile As Profile In ProfileList.GetAll()
            If profile.LastActivityDate.Value < userInactiveSinceDate Then
                DeleteProfile(profile.Username)
                System.Math.Max(System.Threading.Interlocked.Increment(count), count - 1)
            End If
        Next

        Return count
    End Function

    ''' <summary>
    ''' When overridden in a derived class, returns the number of profiles in which the last activity date occurred on or before the specified date.
    ''' </summary>
    ''' <returns>
    ''' The number of profiles in which the last activity date occurred on or before the specified date.
    ''' </returns>
    ''' <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.
    '''                 </param><param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which user profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  of a user profile occurs on or before this date and time, the profile is considered inactive.
    '''                 </param>
    Public Overloads Overrides Function GetNumberOfInactiveProfiles(ByVal authenticationOption As ProfileAuthenticationOption, ByVal userInactiveSinceDate As DateTime) As Integer
        Dim profileList As ProfileList = profileList.NewList()
        For Each item As Profile In profileList.GetAll()
            If item.LastActivityDate.Value < userInactiveSinceDate Then
                profileList.Add(item)
            End If
        Next

        Return profileList.Count
    End Function

    ''' <summary>
    ''' When overridden in a derived class, retrieves user profile data for all profiles in the data source.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing user-profile information for all profiles in the data source.
    ''' </returns>
    ''' <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.
    '''                 </param><param name="pageIndex">The index of the page of results to return.
    '''                 </param><param name="pageSize">The size of the page of results to return.
    '''                 </param><param name="totalRecords">When this method returns, contains the total number of profiles.
    '''                 </param>
    Public Overloads Overrides Function GetAllProfiles(ByVal authenticationOption As ProfileAuthenticationOption, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ProfileInfoCollection
        If pageIndex < 1 OrElse pageSize < 1 Then
            Throw New ApplicationException(String.Format("{0} page index.", _ERR_INVALID_PARAMETER))
        End If

        Dim profiles As New ProfileInfoCollection()

        Dim counter As Integer = 0
        Dim startIndex As Integer = pageSize * (pageIndex - 1)
        Dim endIndex As Integer = startIndex + pageSize - 1

        Dim profileList As ProfileList = profileList.GetAll()
        totalRecords = profileList.Count

        For Each profile As Profile In profileList
            If counter >= startIndex Then
                profiles.Add(New ProfileInfo(profile.Username, profile.IsAnonymous.Value, profile.LastActivityDate.Value, profile.LastUpdatedDate.Value, 0))
            End If

            If counter >= endIndex Then
                Exit For
            End If

            System.Math.Max(System.Threading.Interlocked.Increment(counter), counter - 1)
        Next

        Return profiles
    End Function

    ''' <summary>
    ''' When overridden in a derived class, retrieves user-profile data from the data source for profiles in which the last activity date occurred on or before the specified date.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing user-profile information about the inactive profiles.
    ''' </returns>
    ''' <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.
    '''                 </param><param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which user profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  of a user profile occurs on or before this date and time, the profile is considered inactive.
    '''                 </param><param name="pageIndex">The index of the page of results to return.
    '''                 </param><param name="pageSize">The size of the page of results to return.
    '''                 </param><param name="totalRecords">When this method returns, contains the total number of profiles.
    '''                 </param>
    Public Overloads Overrides Function GetAllInactiveProfiles(ByVal authenticationOption As ProfileAuthenticationOption, ByVal userInactiveSinceDate As DateTime, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ProfileInfoCollection
        If pageIndex < 1 OrElse pageSize < 1 Then
            Throw New ApplicationException(String.Format("{0} page index.", _ERR_INVALID_PARAMETER))
        End If

        Dim profiles As New ProfileInfoCollection()

        Dim counter As Integer = 0
        Dim startIndex As Integer = pageSize * (pageIndex - 1)
        Dim endIndex As Integer = startIndex + pageSize - 1

        Dim profileList As ProfileList = profileList.NewList()
        For Each item As Profile In profileList.GetAll()
            If item.LastActivityDate.Value < userInactiveSinceDate Then
                profileList.Add(item)
            End If
        Next

        totalRecords = profileList.Count

        For Each profile As Profile In profileList
            If counter >= startIndex Then
                profiles.Add(New ProfileInfo(profile.Username, profile.IsAnonymous.Value, profile.LastActivityDate.Value, profile.LastUpdatedDate.Value, 0))
            End If

            If counter >= endIndex Then
                Exit For
            End If

            System.Math.Max(System.Threading.Interlocked.Increment(counter), counter - 1)
        Next

        Return profiles
    End Function

    ''' <summary>
    ''' When overridden in a derived class, retrieves profile information for profiles in which the user name matches the specified user names.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing user-profile information for profiles where the user name matches the supplied <paramref name="usernameToMatch"/> parameter.
    ''' </returns>
    ''' <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.
    '''                 </param><param name="usernameToMatch">The user name to search for.
    '''                 </param><param name="pageIndex">The index of the page of results to return.
    '''                 </param><param name="pageSize">The size of the page of results to return.
    '''                 </param><param name="totalRecords">When this method returns, contains the total number of profiles.
    '''                 </param>
    Public Overloads Overrides Function FindProfilesByUserName(ByVal authenticationOption As ProfileAuthenticationOption, ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ProfileInfoCollection
        If pageIndex < 1 OrElse pageSize < 1 Then
            Throw New ApplicationException(String.Format("{0} page index.", _ERR_INVALID_PARAMETER))
        End If

        Dim profiles As New ProfileInfoCollection()

        Dim counter As Integer = 0
        Dim startIndex As Integer = pageSize * (pageIndex - 1)
        Dim endIndex As Integer = startIndex + pageSize - 1

        Dim profileList As ProfileList = profileList.GetByUsername(usernameToMatch)
        totalRecords = profileList.Count

        For Each profile As Profile In profileList
            If counter >= startIndex Then
                profiles.Add(New ProfileInfo(profile.Username, profile.IsAnonymous.Value, profile.LastActivityDate.Value, profile.LastUpdatedDate.Value, 0))
            End If

            If counter >= endIndex Then
                Exit For
            End If

            System.Math.Max(System.Threading.Interlocked.Increment(counter), counter - 1)
        Next

        Return profiles
    End Function

    ''' <summary>
    ''' When overridden in a derived class, retrieves profile information for profiles in which the last activity date occurred on or before the specified date and the user name matches the specified user name.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing user profile information for inactive profiles where the user name matches the supplied <paramref name="usernameToMatch"/> parameter.
    ''' </returns>
    ''' <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.
    '''                 </param><param name="usernameToMatch">The user name to search for.
    '''                 </param><param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which user profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/> value of a user profile occurs on or before this date and time, the profile is considered inactive.
    '''                 </param><param name="pageIndex">The index of the page of results to return.
    '''                 </param><param name="pageSize">The size of the page of results to return.
    '''                 </param><param name="totalRecords">When this method returns, contains the total number of profiles.
    '''                 </param>
    Public Overloads Overrides Function FindInactiveProfilesByUserName(ByVal authenticationOption As ProfileAuthenticationOption, ByVal usernameToMatch As String, ByVal userInactiveSinceDate As DateTime, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ProfileInfoCollection
        Throw New NotImplementedException()
    End Function
#End Region

#Region "Private Method(s)"

    ''' <summary>
    ''' Updartes an account from an address class.
    ''' </summary>
    ''' <param name="account">The account.</param>
    ''' <param name="address">The address.</param>
    Private Shared Sub UpdateAccount(ByRef account As Account, ByVal address As Address)
        account.FirstName = address.FirstName
        account.LastName = address.LastName
        account.Address1 = address.Address1
        account.Address2 = address.Address2
        account.City = address.City
        account.State = address.State
        account.Zip = address.Zip
        account.Country = address.Country
        account.Phone = address.Phone
        account.Email = address.Email
    End Sub

    ''' <summary>
    ''' Deletes profile data from the database for the specified user name.
    ''' </summary>
    ''' <param name="username">username.</param>
    ''' <returns>true if it was deleted.</returns>
    Private Shared Function DeleteProfile(ByVal username As String) As Boolean
        If String.IsNullOrEmpty(username) OrElse username.Length > 256 OrElse username.IndexOf(",") > 0 Then
            Throw New ApplicationException(_ERR_INVALID_PARAMETER + " user name.")
        End If

        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(username)
        profile.Delete()

        profile = profile.Save(True)

        Return profile.IsDeleted
    End Function

#End Region

End Class