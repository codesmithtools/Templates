<% if(objInfo.FilterProperties.Count > 1) {%>
<%=Indent(i)%>#region Filter Criteria
<%=Indent(i)%>[Serializable()]
<%=Indent(i)%><%=objInfo.MemberAccess%> class FilterCriteria
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.FilterProperties) {%>
<%=Indent(i)%><%=GetCriteriaPropertyDeclaration(prop)%>
<%}%>

<%=Indent(i)%>public FilterCriteria(<%=GetFactoryFilterDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%=GetFactoryFilterAssignments(objInfo, i, false)%>
<%=Indent(--i)%>}
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Filter Criteria
<%}%>
