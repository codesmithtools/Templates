#Region " Exists "

<%=Indent(i)%>Public Shared Function Exists(<%=GetFactoryNewDeclarationArguments(objInfo)%>) As Boolean
<% i+=1 %>
<%=Indent(i)%>Dim result As ExistsCommand
<%=Indent(i)%>result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(<%=GetFactoryNewCallArguments(objInfo)%>))
<%=Indent(i)%>Return result.Exists
<% i-=1 %>
<%=Indent(i)%>End Function

<%=Indent(i)%><Serializable()> _
<%=Indent(i)%>Private Class ExistsCommand
<% i+=1 %>
<%=Indent(i)%>Inherits CommandBase

<%=Indent(i)%>Private mId As Guid
<%=Indent(i)%>Private mExists As Boolean

<%=Indent(i)%>Public ReadOnly Property Exists() As Boolean
<%=Indent(i+1)%>Get
<%=Indent(i+2)%>Return mExists
<%=Indent(i+1)%>End Get
<%=Indent(i)%>End Property

<%=Indent(i)%>Public Sub New(ByVal id As Guid)
<%=Indent(i+1)%>mId = id
<%=Indent(i)%>End Sub

<%=Indent(i)%>Protected Overrides Sub DataPortal_Execute()
<% i+=1 %>
<%=Indent(i)%>Using cn As New SqlConnection(Database.PTrackerConnection)
<% i+=1 %>
<%=Indent(i)%>cn.Open()
<%=Indent(i)%>Using cm As SqlCommand = cn.CreateCommand
<% i+=1 %>
<%=Indent(i)%>cm.CommandType = CommandType.Text
<%=Indent(i)%>cm.CommandText = "SELECT Id FROM Projects WHERE Id=@id"
<%=Indent(i)%>cm.Parameters.AddWithValue("@id", mId)

<%=Indent(i)%>Dim count As Integer = CInt(cm.ExecuteScalar)
<%=Indent(i)%>mExists = (count > 0)
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
    End Sub
<% i-=1 %>

<%=Indent(i)%>End Class

#End Region