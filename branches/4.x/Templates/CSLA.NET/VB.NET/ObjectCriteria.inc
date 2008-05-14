
<%=Indent(0)%>#Region " Criteria "

<%=Indent(i)%><Serializable()> _
<%=Indent(i)%><%=objInfo.MemberAccess%> Class Criteria
<% If objInfo.IsGeneratedBase Then %> 
<%=Indent(i+1)%>Inherits Csla.CriteriaBase<% end if %>
<% For Each prop As PropertyInfo In objInfo.UniqueProperties %>
<%=Indent(i+1)%><%=GetCriteriaPropertyDeclaration(prop)%>
<% Next %>

<%=Indent(i+1)%>Public Sub New(<%=GetCriteriaDeclarationArguments(objInfo)%>)<% If objInfo.IsGeneratedBase Then %><%=Indent(i+2, True)%>Mybase.New(GetType(<%=objInfo.Type%>))<% End If %>
<% For Each prop As PropertyInfo In objInfo.UniqueProperties %>
<%=Indent(i+2)%><%=GetCriteriaMemberAssignment(prop)%>
<% Next %>
<%=Indent(i+1)%>End Sub
<%=Indent(i)%>End Class

<%=Indent(0)%>#End Region 'Criteria
