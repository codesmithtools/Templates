<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%>#region Data Access - Fetch
<%=Indent(i)%>private void Child_Fetch(<%=objInfo.DalNamespace%>.<%=objInfo.EntityName%> data)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>bool cancel = false;
<%=Indent(i)%>OnLoading(data, ref cancel);
<%=Indent(i)%>if (cancel) return;

<%}%>
<% foreach(PropertyInfo prop in objInfo.Properties) {
	if (!prop.HasDbColumn) continue; %>
<%  if (prop.IsTimestamp) { %>        
	 
<%=Indent(i)%><%=prop.MemberName%> = data.<%=prop.DbColumnName%>.ToArray();
<%  } else if(prop.Type != prop.NativeType) { %>
<%=Indent(i)%>LoadProperty<<%=prop.Type%>, <%=prop.NativeType%>>(<%=prop.PropertyInfoName%>, data.<%=prop.DbColumnName%>);
<%  } else { %>
<%=Indent(i)%>LoadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, data.<%=prop.DbColumnName%>);
<%  } %>
<% } %>
<%if(objInfo.ChildCollection.Count>0) {%>

<%=Indent(i)%>//Load Child object
<%}%>
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%>LoadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, <%=prop.Type%>.Get<%=prop.Type%>(data.<%=prop.DbRefTableName%>.ToArray()));
<% } %>
<% if(!objInfo.IsReadOnly) {%>

<%=Indent(i)%>ValidationRules.CheckRules();
<%}%>
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>OnLoaded();
<%}%>
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>partial void OnLoading(<%=objInfo.DalNamespace%>.<%=objInfo.EntityName%> data, ref bool cancel);
<%=Indent(i)%>partial void OnLoaded();
<% } %>
<%=Indent(i)%>#endregion //Data Access - Fetch
<% } %>
