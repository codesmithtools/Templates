'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated Imports CSLA 3.8.X CodeSmith Templates.
'     Changes to this file will be lost after each regeneration.
'     To extend the functionality of this class, please modify the partial class 'CategoryList.vb.
'
'     Template: EditableRootList.Generated.cst
'     Template website: http://code.google.com/p/codesmith/
' </autogenerated>
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports System

Imports Csla
Imports Csla.Data

<Serializable()> _
Public Partial Class CategoryList 
    Inherits BusinessListBase(Of CategoryList, Category)

    #Region "Contructor(s)"
    
    Private Sub New()
        AllowNew = true
    End Sub
    
    #End Region
    
    #Region "Factory Methods"
    
    Public Shared Function NewList() As CategoryList
        Return DataPortal.Create(Of CategoryList)()
    End Function
    
    Public Shared Function GetCategoryList(ByVal categoryId As String) As CategoryList
        Return DataPortal.Fetch(Of CategoryList)(new CategoryCriteria(categoryId))
    End Function
    
    Public Shared Function GetAll() As CategoryList
        Return DataPortal.Fetch(Of CategoryList)(new CategoryCriteria())
    End Function
    
    #End Region
    
    #Region "Business Methods"
    
    Protected Overrides Function AddNewCore() As Object
        Dim item As Category = PetShop.Business.Category.NewCategory()
        Me.Add(item)
        Return item
    End Function
    
    #End Region

End Class