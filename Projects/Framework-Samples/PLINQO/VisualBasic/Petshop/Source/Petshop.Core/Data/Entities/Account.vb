    
Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.Data.Linq
Imports System.ComponentModel.DataAnnotations
Imports CodeSmith.Data.Attributes
Imports CodeSmith.Data.Rules

Namespace PetShop.Core.Data
    
    public Partial Class Account
        
        ' For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        ' http://code.google.com/p/codesmith/wiki/PLINQO
        ' Also, you can watch our Video Tutorials at...
        ' http://community.codesmithtools.com/
        
        #Region "Metadata"
        
        <CodeSmith.Data.Audit.Audit()> _
        Friend Class Metadata
            ' Only Attributes in the class will be preserved.
            
            Public Property AccountID() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property UniqueID() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            <DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)> _
            Public Property Email() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property FirstName() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property LastName() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property Address1() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Address2() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property City() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property State() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property Zip() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property Country() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)> _
            Public Property Phone() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Number() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Profiles() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
        End Class
        
        #End Region
    
    End Class

End Namespace
