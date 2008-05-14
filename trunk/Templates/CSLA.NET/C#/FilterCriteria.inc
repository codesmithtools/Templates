
<%=Indent(i)%>#region Filter Criteria
<%=Indent(i)%>[Serializable()]
<%=Indent(i)%><%=objInfo.MemberAccess%> class FilterCriteria<%if(objInfo.IsGeneratedBase) {%> : Csla.CriteriaBase<%}%>
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.FilterProperties) {%>
<%=Indent(i)%><%=GetCriteriaPropertyDeclaration(prop)%>
<%}%>

<%=Indent(i)%>public FilterCriteria(<%=GetFactoryFilterDeclarationArguments(objInfo)%>)<%if(objInfo.IsGeneratedBase) {%> : base(typeof(<%=objInfo.Type%>))<%}%>
<%=Indent(i++)%>{
<%=GetFactoryFilterAssignments(objInfo, i, false)%>
<%=Indent(--i)%>}
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Filter Criteria
