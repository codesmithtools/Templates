Imports System
Imports System.Linq
Imports System.Data.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel.DataAnnotations
Imports CodeSmith.Data.Attributes
Imports CodeSmith.Data.Rules

Namespace Tracker.Core.Data
    
    Public Partial Class GetUsersWithRolesResult2
        
        ' Place custom code here.
        
        #Region "Metadata"
        ' For more information about how to use the metadata class visit:
        ' http://www.plinqo.com/metadata.ashx
        <CodeSmith.Data.Audit.Audit()> _
        Friend Class Metadata
            ' WARNING: Only attributes inside of this class will be preserved.
            
            Public Property UserId() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property RoleId() As Object
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
