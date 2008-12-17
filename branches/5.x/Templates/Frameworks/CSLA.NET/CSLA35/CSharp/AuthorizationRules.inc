<% if(objInfo.IsUserClass && objInfo.UseSecurity) {%>

<%=Indent(i)%>#region Authorization Rules
<% if(!objInfo.IsCollection) {%>
<% if(objInfo.PropertyAuthorization != PropertyAccessSecurity.None) { %>
<%=Indent(i)%>protected override void AddAuthorizationRules()
<%=Indent(i++)%>{
<%=Indent(i)%>//TODO: Define authorization rules in <%=objInfo.Name%>
<% if(objInfo.PropertyAuthorization == PropertyAccessSecurity.Read 
	|| objInfo.PropertyAuthorization == PropertyAccessSecurity.Both) { %>
<% foreach(PropertyInfo prop in objInfo.Properties) { if(prop.IsTimestamp) continue; %>
<%=Indent(i)%>//AuthorizationRules.AllowRead(<%= prop.PropertyInfoName %>, "<%=objInfo.Name%>ReadGroup");
<%} //foreach%>
<%} //if(read)%>
<% if(objInfo.PropertyAuthorization == PropertyAccessSecurity.Write 
	|| objInfo.PropertyAuthorization == PropertyAccessSecurity.Both) { %>

<% foreach(PropertyInfo prop in objInfo.Properties) { if(!prop.IsReadOnly && !prop.IsDbComputed) {%>
<%=Indent(i)%>//AuthorizationRules.AllowWrite(<%= prop.PropertyInfoName %>, "<%=objInfo.Name%>WriteGroup");
<%}} //foreach, if%>
<%} //if(write)%>
<%=Indent(--i)%>}
<%} //if(!None)%>
<%}%>
<% if(!objInfo.IsChild || objInfo.CslaObjectType == ObjectType.EditableSwitchable) {%>

<%=Indent(i)%>protected static void AddObjectAuthorizationRules()
<%=Indent(i++)%>{
<%=Indent(i)%>//TODO: Define object-level authorization rules of <%=objInfo.Name%>
<%=Indent(i)%>//AuthorizationRules.AllowCreate(typeof(<%=objInfo.Name%>), "<%=objInfo.Name%>CreateGroup"));
<% if(!objInfo.IsReadOnly) {%>
<%=Indent(i)%>//AuthorizationRules.AllowEdit(typeof(<%=objInfo.Name%>), "<%=objInfo.Name%>EditGroup"));
<%=Indent(i)%>//AuthorizationRules.AllowDelete(typeof(<%=objInfo.Name%>), "<%=objInfo.Name%>DeleteGroup"));
<%} //if(!IsReadOnly)%>
<%=Indent(--i)%>}
<%} //if(!IsChild)%>
<%=Indent(i)%>#endregion //Authorization Rules
<%} //if(IsUserClass && UseSecurity)%>
