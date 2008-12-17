
<%=Indent(i)%>#region Criteria

<%=Indent(i)%>[Serializable()]
<%=Indent(i)%><%=objInfo.MemberAccess%> class Criteria<%if(objInfo.IsGeneratedBase) {%> : Csla.CriteriaBase<%}%>
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.UniqueProperties) {%>
<%=Indent(i)%><%=GetCriteriaPropertyDeclaration(prop)%>
<%}%>

<%=Indent(i)%>public Criteria(<%=GetCriteriaDeclarationArguments(objInfo)%>)<%if(objInfo.IsGeneratedBase) {%> : base(typeof(<%=objInfo.Type%>))<%}%>
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.UniqueProperties) {%>
<%=Indent(i)%><%=GetCriteriaMemberAssignment(prop)%>
<%}%>
<%=Indent(--i)%>}
<%=Indent(--i)%>}

<%=Indent(i)%>#endregion //Criteria
