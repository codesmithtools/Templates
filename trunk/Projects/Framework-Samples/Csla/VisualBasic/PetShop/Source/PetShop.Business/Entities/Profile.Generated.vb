'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated Imports CSLA 3.8.X CodeSmith Templates.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'Profile.vb.
'
'     Template: SwitchableObject.Generated.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports System

Imports Csla
Imports Csla.Data
Imports Csla.Validation

<Serializable()> _
Public Partial Class Profile 
    Inherits BusinessBase(Of Profile)
    
    #Region "Contructor(s)"

    Private Sub New()
        ' require use of factory method 
    End Sub

    Friend Sub New(Byval reader As SafeDataReader)
        Map(reader)
    End Sub

    #End Region
    
    #Region "Validation Rules"
    
    Protected Overrides Sub AddBusinessRules()
    
        If AddBusinessValidationRules() Then Exit Sub
       
        ValidationRules.AddRule(AddressOf CommonRules.StringRequired, _usernameProperty)
        ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs(_usernameProperty, 256))
        ValidationRules.AddRule(AddressOf CommonRules.StringRequired, _applicationNameProperty)
        ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs(_applicationNameProperty, 256))
    End Sub
    
    #End Region
    
    #Region "Business Methods"


    
    Private Shared ReadOnly _uniqueIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As Profile) p.UniqueID)
		<System.ComponentModel.DataObjectField(true, true)> _
    Public ReadOnly Property UniqueID() As Integer
        Get 
            Return GetProperty(_uniqueIDProperty)
        End Get
    End Property
    
    
    Private Shared ReadOnly _usernameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Profile) p.Username)
    Public Property Username() As String
        Get 
            Return GetProperty(_usernameProperty)
        End Get
        Set (ByVal value As String)
            SetProperty(_usernameProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _applicationNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Profile) p.ApplicationName)
    Public Property ApplicationName() As String
        Get 
            Return GetProperty(_applicationNameProperty)
        End Get
        Set (ByVal value As String)
            SetProperty(_applicationNameProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _isAnonymousProperty As PropertyInfo(Of System.Nullable(Of Boolean)) = RegisterProperty(Of System.Nullable(Of Boolean))(Function(p As Profile) p.IsAnonymous)
    Public Property IsAnonymous() As System.Nullable(Of Boolean)
        Get 
            Return GetProperty(_isAnonymousProperty)
        End Get
        Set (ByVal value As System.Nullable(Of Boolean))
            SetProperty(_isAnonymousProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _lastActivityDateProperty As PropertyInfo(Of System.Nullable(Of SmartDate)) = RegisterProperty(Of System.Nullable(Of SmartDate))(Function(p As Profile) p.LastActivityDate)
    Public Property LastActivityDate() As System.Nullable(Of SmartDate)
        Get 
            Return GetProperty(_lastActivityDateProperty)
        End Get
        Set (ByVal value As System.Nullable(Of SmartDate))
            SetProperty(_lastActivityDateProperty, value)
        End Set
    End Property
    
    
    Private Shared ReadOnly _lastUpdatedDateProperty As PropertyInfo(Of System.Nullable(Of SmartDate)) = RegisterProperty(Of System.Nullable(Of SmartDate))(Function(p As Profile) p.LastUpdatedDate)
    Public Property LastUpdatedDate() As System.Nullable(Of SmartDate)
        Get 
            Return GetProperty(_lastUpdatedDateProperty)
        End Get
        Set (ByVal value As System.Nullable(Of SmartDate))
            SetProperty(_lastUpdatedDateProperty, value)
        End Set
    End Property
    
    Private Shared ReadOnly _accountsProperty As PropertyInfo(Of AccountList) = RegisterProperty(Of AccountList)(Function(p As Profile) p.Accounts, Csla.RelationshipTypes.LazyLoad)
    Public ReadOnly Property Accounts() As AccountList 
        Get
            If Not (FieldManager.FieldExists(_accountsProperty)) Then
                If (Me.IsNew) Then
                    LoadProperty(_accountsProperty, AccountList.NewList())
                Else
                    LoadProperty(_accountsProperty, AccountList.GetByUniqueID(UniqueID))
                End If
            End If
            
            Return GetProperty(_accountsProperty) 
        End Get
    End Property
    
    Private Shared ReadOnly _cartsProperty As PropertyInfo(Of CartList) = RegisterProperty(Of CartList)(Function(p As Profile) p.Carts, Csla.RelationshipTypes.LazyLoad)
    Public ReadOnly Property Carts() As CartList 
        Get
            If Not (FieldManager.FieldExists(_cartsProperty)) Then
                If (Me.IsNew) Then
                    LoadProperty(_cartsProperty, CartList.NewList())
                Else
                    LoadProperty(_cartsProperty, CartList.GetByUniqueID(UniqueID))
                End If
            End If
            
            Return GetProperty(_cartsProperty) 
        End Get
    End Property
    
    #End Region
            
    #Region "Root Factory Methods"
    
    Public Shared Function NewProfile() As Profile 
        Return DataPortal.Create(Of Profile)()
    End Function
    
    Public Shared Function GetProfile(ByVal uniqueID As Integer) As Profile         
        Return DataPortal.Fetch(Of Profile)(New ProfileCriteria(uniqueID))
    End Function

    Public Shared Sub DeleteProfile(ByVal uniqueID As Integer)
        DataPortal.Delete(New ProfileCriteria(uniqueID))
    End Sub
    Public Shared Sub DeleteProfile(ByVal username As String, ByVal applicationName As String, ByVal uniqueID As Integer)
        Dim criteria As New ProfileCriteria()
        criteria.Username = username
        criteria.ApplicationName = applicationName
        criteria.UniqueID = uniqueID

        DataPortal.Delete(criteria)
    End Sub
    
    #End Region
    
    #Region "Child Factory Methods"
    
    Friend Shared Function NewProfileChild() As Profile
        Return DataPortal.CreateChild(Of Profile)()
    End Function
    
    Friend Shared Function GetProfileChild(ByVal uniqueID As Integer) As Profile         
        Return DataPortal.FetchChild(Of Profile)(New ProfileCriteria(uniqueID))
    End Function

    #End Region
    

End Class