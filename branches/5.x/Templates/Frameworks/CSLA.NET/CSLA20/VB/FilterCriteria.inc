
#Region " Filter Criteria "

<%=Indent(i)%><Serializable()> _
<%=Indent(i)%><%=objInfo.MemberAccess%> Class FilterCriteria
<% If objInfo.IsGeneratedBase Then %>
<%=Indent(i+1)%>Inherits Csla.CriteriaBase
<% End If %>
<% i+=1 %>
<% For Each prop As PropertyInfo In objInfo.FilterProperties %>
<%=Indent(i)%><%=GetCriteriaPropertyDeclaration(prop)%>
<% Next %>

<%=Indent(i)%>Public Sub New(<%=GetFactoryFilterDeclarationArguments(objInfo)%>)
<% If objInfo.IsGeneratedBase Then %> 
<%=Indent(i+1)%>MyBase.New(GetType(<%=objInfo.Type%>))
<% End If %>
<%=GetFactoryFilterAssignments(objInfo, i+1, false) %>
<%=Indent(i)%>End Sub
<% i-=1 %>
<%=Indent(i)%>End Class

#End Region
