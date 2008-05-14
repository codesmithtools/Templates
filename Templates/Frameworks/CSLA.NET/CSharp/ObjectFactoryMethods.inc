
<%=Indent(i)%>#region Factory Methods
<% if(objInfo.IsSingle || objInfo.IsGeneratedPartial || objInfo.IsUserBase) {%>
<%=Indent(i)%>private <%=objInfo.Name%>()
<%=Indent(i)%>{ /* require use of factory method */ }

<% } %>
<% //all in generated class except when base class with authorization turned on
	if(objInfo.IsSingle || objInfo.IsGeneratedPartial 
	|| (objInfo.IsGeneratedBase && !objInfo.UseSecurity)
	|| (objInfo.IsUserBase && objInfo.UseSecurity)) { %>
<% if(!objInfo.IsReadOnly) {%>
<%=Indent(i)%>public static <%=objInfo.Type%> <%=objInfo.NewMethodName%>(<%=GetFactoryNewDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<% if(objInfo.UseSecurity) {%>
<%=Indent(i)%>if (!CanAddObject())
<%=Indent(i+1)%>throw new System.Security.SecurityException("User not authorized to add a <%=objInfo.Name%>");
<% } %>
<%=Indent(i)%>return DataPortal.Create<<%=objInfo.Type%>>(<% string cPars = GetFactoryNewCallArguments(objInfo); if(cPars.Length>0) {%>new Criteria(<%=cPars%>)<%}%>);
<%=Indent(--i)%>}

<% } //if(!objInfo.IsReadOnly)%>
<%=Indent(i)%>public static <%=objInfo.Type%> <%=objInfo.GetMethodName%>(<%=GetFactoryDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<% if(objInfo.UseSecurity) {%>
<%=Indent(i)%>if (!CanGetObject())
<%=Indent(i+1)%>throw new System.Security.SecurityException("User not authorized to view a <%=objInfo.Name%>");
<% } %>
<%=Indent(i)%>return DataPortal.Fetch<<%=objInfo.Type%>>(new Criteria(<%=GetFactoryCallArguments(objInfo)%>));
<%=Indent(--i)%>}
<% if(!objInfo.IsReadOnly) {%>

<%=Indent(i)%>public static void <%=objInfo.DeleteMethodName%>(<%=GetFactoryDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<% if(objInfo.UseSecurity) {%>
<%=Indent(i)%>if (!CanDeleteObject())
<%=Indent(i+1)%>throw new System.Security.SecurityException("User not authorized to remove a <%=objInfo.Name%>");
<% } %>
<%=Indent(i)%>DataPortal.Delete(new Criteria(<%=GetFactoryCallArguments(objInfo)%>));
<%=Indent(--i)%>}

<% if(objInfo.UseSecurity) {%>
<%=Indent(i)%>public override <%=objInfo.Type%> Save()
<%=Indent(i++)%>{
<%=Indent(i)%>if (IsDeleted && !CanDeleteObject())
<%=Indent(i+1)%>throw new System.Security.SecurityException("User not authorized to remove a <%=objInfo.Name%>");
<%=Indent(i)%>else if (IsNew && !CanAddObject())
<%=Indent(i+1)%>throw new System.Security.SecurityException("User not authorized to add a <%=objInfo.Name%>");
<%=Indent(i)%>else if (!CanEditObject())
<%=Indent(i+1)%>throw new System.Security.SecurityException("User not authorized to update a <%=objInfo.Name%>");

<%=Indent(i)%>return base.Save();
<%=Indent(--i)%>}

<% } %>
<% } //if(!objInfo.IsReadOnly)%>
<% } //all in generated class except when base class with authorization turned on%>
<%=Indent(i)%>#endregion //Factory Methods
