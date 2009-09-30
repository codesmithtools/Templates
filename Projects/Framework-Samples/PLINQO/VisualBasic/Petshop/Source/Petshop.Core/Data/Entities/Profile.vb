    
Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.Data.Linq
Imports System.ComponentModel.DataAnnotations
Imports CodeSmith.Data.Attributes
Imports CodeSmith.Data.Rules

Namespace PetShop.Core.Data
    
    public Partial Class Profile
        
        ' For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        ' http://code.google.com/p/codesmith/wiki/PLINQO
        ' Also, you can watch our Video Tutorials at...
        ' http://community.codesmithtools.com/
        
        #Region "Metadata"
        
        <CodeSmith.Data.Audit.Audit()> _
        Friend Class Metadata
            ' Only Attributes in the class will be preserved.
            
            Public Property UniqueID() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property Username() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property ApplicationName() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property IsAnonymous() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property LastActivityDate() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property LastUpdatedDate() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property AccountList() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property CartList() As Object
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
