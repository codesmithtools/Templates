Imports System.Web.SessionState
Imports PetShop.Business

Public Class Global_asax
    Inherits System.Web.HttpApplication

    ' Carry over profile property values from an anonymous to an authenticated state
    Protected Sub Profile_MigrateAnonymous(ByVal sender As [Object], ByVal e As ProfileMigrateEventArgs)
        Dim anonymousProfile As Profile = ProfileManager.Instance.GetAnonymousUser()
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(e.Context.User.Identity.Name)

        'Merge anonymous shopping cart items to the authenticated shopping cart items
        For Each item As Cart In anonymousProfile.ShoppingCart
            profile.ShoppingCart.Add(item.ItemId, profile.UniqueID, True, item.Quantity)
        Next

        'Merge anonymous wishlist items to the authenticated wishlist items
        For Each item As Cart In anonymousProfile.WishList
            profile.WishList.Add(item.ItemId, profile.UniqueID, False, item.Quantity)
        Next

        profile = profile.Save()

        'Clear the cart.
        anonymousProfile.ShoppingCart.Clear()
        anonymousProfile.WishList.Clear()
        anonymousProfile = anonymousProfile.Save()

        ' Clean up anonymous profile
        'System.Web.Profile.ProfileManager.DeleteProfile(e.AnonymousID)
        'AnonymousIdentificationModule.ClearAnonymousIdentifier()
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        'Dim exception As Exception = Server.GetLastError().GetBaseException()
        'EventLog.WriteEntry(".NET Pet Shop 4.0", exception.ToString(), EventLogEntryType.[Error])
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class