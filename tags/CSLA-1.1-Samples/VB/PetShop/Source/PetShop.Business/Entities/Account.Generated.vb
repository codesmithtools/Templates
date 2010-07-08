'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated Imports CSLA 3.7.X CodeSmith Templates.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'Account.vb.
'
'     Template: EditableChild.Generated.cst
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
Public Partial Class Account 
    Inherits BusinessBase(Of Account)

    #Region "Contructor(s)"

	Private Sub New()
    	' require use of factory method 
End Sub
    
    Friend Sub New(Byval reader As SafeDataReader)
        Fetch(reader)
	End Sub
    
	#End Region
    
	#Region "Validation Rules"
	
	Protected Overrides Sub AddBusinessRules()
	
        If AddBusinessValidationRules() Then Exit Sub
       
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "Email")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("Email", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "FirstName")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("FirstName", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "LastName")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("LastName", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "Address1")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("Address1", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("Address2", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "City")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("City", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "State")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("State", 80))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "Zip")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("Zip", 20))
		ValidationRules.AddRule(AddressOf CommonRules.StringRequired, "Country")
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("Country", 20))
		ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs("Phone", 20))
	End Sub
	
	#End Region
    
    #Region "Business Methods"


	
	Private Shared ReadOnly _accountIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As Account) p.AccountId)
		<System.ComponentModel.DataObjectField(true, true)> _
	Public ReadOnly Property AccountId() As Integer
		Get 
			Return GetProperty(_accountIdProperty)
		End Get
	End Property
	
	
	Private Shared ReadOnly _emailProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.Email)
	Public Property Email() As String
		Get 
			Return GetProperty(_emailProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("Email")
            SetProperty(_emailProperty, value)
            OnPropertyChanged("Email")
        End Set
	End Property
	
	
	Private Shared ReadOnly _firstNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.FirstName)
	Public Property FirstName() As String
		Get 
			Return GetProperty(_firstNameProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("FirstName")
            SetProperty(_firstNameProperty, value)
            OnPropertyChanged("FirstName")
        End Set
	End Property
	
	
	Private Shared ReadOnly _lastNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.LastName)
	Public Property LastName() As String
		Get 
			Return GetProperty(_lastNameProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("LastName")
            SetProperty(_lastNameProperty, value)
            OnPropertyChanged("LastName")
        End Set
	End Property
	
	
	Private Shared ReadOnly _address1Property As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.Address1)
	Public Property Address1() As String
		Get 
			Return GetProperty(_address1Property)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("Address1")
            SetProperty(_address1Property, value)
            OnPropertyChanged("Address1")
        End Set
	End Property
	
	
	Private Shared ReadOnly _address2Property As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.Address2)
	Public Property Address2() As String
		Get 
			Return GetProperty(_address2Property)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("Address2")
            SetProperty(_address2Property, value)
            OnPropertyChanged("Address2")
        End Set
	End Property
	
	
	Private Shared ReadOnly _cityProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.City)
	Public Property City() As String
		Get 
			Return GetProperty(_cityProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("City")
            SetProperty(_cityProperty, value)
            OnPropertyChanged("City")
        End Set
	End Property
	
	
	Private Shared ReadOnly _stateProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.State)
	Public Property State() As String
		Get 
			Return GetProperty(_stateProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("State")
            SetProperty(_stateProperty, value)
            OnPropertyChanged("State")
        End Set
	End Property
	
	
	Private Shared ReadOnly _zipProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.Zip)
	Public Property Zip() As String
		Get 
			Return GetProperty(_zipProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("Zip")
            SetProperty(_zipProperty, value)
            OnPropertyChanged("Zip")
        End Set
	End Property
	
	
	Private Shared ReadOnly _countryProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.Country)
	Public Property Country() As String
		Get 
			Return GetProperty(_countryProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("Country")
            SetProperty(_countryProperty, value)
            OnPropertyChanged("Country")
        End Set
	End Property
	
	
	Private Shared ReadOnly _phoneProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p As Account) p.Phone)
	Public Property Phone() As String
		Get 
			Return GetProperty(_phoneProperty)
		End Get
        Set (ByVal value As String)
            OnPropertyChanging("Phone")
            SetProperty(_phoneProperty, value)
            OnPropertyChanged("Phone")
        End Set
	End Property
	
    Private Shared _uniqueIDProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p As Account) p.UniqueID)
	Public Property UniqueID() As Integer 
		Get  
			Return GetProperty(_uniqueIDProperty)				
		End Get
        Set (ByVal value As Integer)
            OnPropertyChanging("UniqueID")
            SetProperty(_uniqueIDProperty, value) 
            OnPropertyChanged("UniqueID")
        End Set
	End Property
	
	Private Shared ReadOnly _profileProperty As PropertyInfo(Of Profile) = RegisterProperty(Of Profile)(Function(p As Account) p.Profile, Csla.RelationshipTypes.LazyLoad)
	Public ReadOnly Property Profile() As Profile
		Get
        
            If Not(FieldManager.FieldExists(_profileProperty))
                SetProperty(_profileProperty, Profile.GetProfile(UniqueID))
            End If
            
            Return GetProperty(_profileProperty) 
        End Get
    End Property
    
	#End Region
			
	#Region "Factory Methods"
	
	Friend Shared Function NewAccount() As Account 
		Return DataPortal.Create(Of Account)()
	End Function
	
	Friend Shared Function GetAccount(ByVal accountId As Integer) As Account 		
		Return DataPortal.Fetch(Of Account)(New AccountCriteria(accountId))
	End Function
	
	#End Region
	

End Class