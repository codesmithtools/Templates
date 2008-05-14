<% if(objInfo.IsUserClass && objInfo.UseSecurity) {%>

<%=Indent(i)%>#region Authorization Rules
<% if(!objInfo.IsCollection) {%>
<% if(objInfo.PropertyAuthorization != PropertyAccessSecurity.None) { %>
<%=Indent(i)%>protected override void AddAuthorizationRules()
<%=Indent(i++)%>{
<% if(objInfo.PropertyAuthorization == PropertyAccessSecurity.Read 
	|| objInfo.PropertyAuthorization == PropertyAccessSecurity.Both) { %>
<%=Indent(i)%>//TODO: Define authorization rules in <%=objInfo.Name%>
<% foreach(PropertyInfo prop in objInfo.Properties) { if(prop.IsTimestamp) continue; %>
<%=Indent(i)%>//AuthorizationRules.AllowRead("<%= prop.Name %>", "<%=objInfo.Name%>ReadGroup");
<% if(prop.Type == "SmartDate") { %>
<%=Indent(i)%>//AuthorizationRules.AllowRead("<%= prop.Name %>String", "<%=objInfo.Name%>ReadGroup");
<%}%>
<%} //foreach%>
<%} //if(read)%>
<% if(objInfo.PropertyAuthorization == PropertyAccessSecurity.Write 
	|| objInfo.PropertyAuthorization == PropertyAccessSecurity.Both) { %>

<% foreach(PropertyInfo prop in objInfo.Properties) { if(!prop.IsReadOnly && !prop.IsDbComputed) {%>
<% if(prop.Type == "SmartDate") { %>
<%=Indent(i)%>//AuthorizationRules.AllowWrite("<%= prop.Name %>String", "<%=objInfo.Name%>WriteGroup");
<%} else {%>
<%=Indent(i)%>//AuthorizationRules.AllowWrite("<%= prop.Name %>", "<%=objInfo.Name%>WriteGroup");
<%}%>
<%}} //foreach, if%>
<%} //if(write)%>
<%=Indent(--i)%>}

<%} //if(!None)%>
<%}%>
<% if(!objInfo.IsChild || objInfo.CslaObjectType == ObjectType.EditableSwitchable) {%>

<%=Indent(i)%>public static bool CanGetObject()
<%=Indent(i++)%>{
<%=Indent(i)%>//TODO: Define CanGetObject permission in <%=objInfo.Name%>
<%=Indent(i)%>return true;
<%=Indent(i)%>//if (Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>ViewGroup"))
<%=Indent(i)%>//<%=Indent(1)%>return true;
<%=Indent(i)%>//return false;
<%=Indent(--i)%>}
<% if(!objInfo.IsReadOnly) {%>

<%=Indent(i)%>public static bool CanAddObject()
<%=Indent(i++)%>{
<%=Indent(i)%>//TODO: Define CanAddObject permission in <%=objInfo.Name%>
<%=Indent(i)%>return true;
<%=Indent(i)%>//if (Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>AddGroup"))
<%=Indent(i)%>//<%=Indent(1)%>return true;
<%=Indent(i)%>//return false;
<%=Indent(--i)%>}

<%=Indent(i)%>public static bool CanEditObject()
<%=Indent(i++)%>{
<%=Indent(i)%>//TODO: Define CanEditObject permission in <%=objInfo.Name%>
<%=Indent(i)%>return true;
<%=Indent(i)%>//if (Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>EditGroup"))
<%=Indent(i)%>//<%=Indent(1)%>return true;
<%=Indent(i)%>//return false;
<%=Indent(--i)%>}

<%=Indent(i)%>public static bool CanDeleteObject()
<%=Indent(i++)%>{
<%=Indent(i)%>//TODO: Define CanDeleteObject permission in <%=objInfo.Name%>
<%=Indent(i)%>return true;
<%=Indent(i)%>//if (Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>DeleteGroup"))
<%=Indent(i)%>//<%=Indent(1)%>return true;
<%=Indent(i)%>//return false;
<%=Indent(--i)%>}
<%} //if(!IsReadOnly)%>
<%} //if(!IsChild)%>
<%=Indent(i)%>#endregion //Authorization Rules
<%} //if(IsUserClass && UseSecurity)%>
