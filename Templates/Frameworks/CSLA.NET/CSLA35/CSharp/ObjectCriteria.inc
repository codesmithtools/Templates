<% if (objInfo.UniqueProperties.Count > 1) { %>
<%=Indent(i)%>#region Criteria

<%=Indent(i)%>[Serializable()]
<%=Indent(i)%><%=objInfo.MemberAccess%> class Criteria
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.UniqueProperties) {%>
<%=Indent(i)%><%=GetCriteriaPropertyDeclaration(prop)%>
<%}%>

<%=Indent(i)%>public Criteria(<%=GetCriteriaDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.UniqueProperties) {%>
<%=Indent(i)%><%=GetCriteriaMemberAssignment(prop)%>
<%}%>
<%=Indent(--i)%>}
<%=Indent(--i)%>}

<%=Indent(i)%>#endregion //Criteria
<% } %>