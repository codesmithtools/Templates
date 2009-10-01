Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel.DataAnnotations
Imports CodeSmith.Data.Attributes
Imports CodeSmith.Data.Rules

Namespace Petshop.Data
    
    public Partial Class Supplier
        
        ' For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        ' http://code.google.com/p/codesmith/wiki/PLINQO
        ' Also, you can watch our Video Tutorials at...
        ' http://community.codesmithtools.com/
        
        #Region "Metadata"
        
        Protected Friend Class Metadata
            ' Only Attributes in the class will be preserved.
            
            Public Property SuppId() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Name() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <Required> _
            Public Property Status() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Addr1() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Addr2() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property City() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property State() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            Public Property Zip() As Object
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object)
                End Set
            End Property
            
            <DataType(DataType.PhoneNumber)> _
            Public Property Phone() As Object
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

