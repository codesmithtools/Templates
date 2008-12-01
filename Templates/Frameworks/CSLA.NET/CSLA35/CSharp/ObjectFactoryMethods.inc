<% if(objInfo.IsGeneratedClass) {%>

<%=Indent(i)%>#region Factory Methods
<% if(!objInfo.IsReadOnly) {%>
<%=Indent(i)%>public static <%=objInfo.Type%> <%=objInfo.NewMethodName%>(<%=GetFactoryNewDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%=Indent(i)%>return DataPortal.Create<<%=objInfo.Type%>>(<%=GetFactoryNewDPCallArguments(objInfo)%>);
<%=Indent(--i)%>}

<% } //if(!objInfo.IsReadOnly)%>
<%=Indent(i)%>public static <%=objInfo.Type%> <%=objInfo.GetMethodName%>(<%=GetFactoryDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%=Indent(i)%>return DataPortal.Fetch<<%=objInfo.Type%>>(<%=GetFactoryDPCallArguments(objInfo)%>);
<%=Indent(--i)%>}
<% if(!objInfo.IsReadOnly) {%>

<%=Indent(i)%>public static void <%=objInfo.DeleteMethodName%>(<%=GetFactoryDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%=Indent(i)%>DataPortal.Delete(<%=GetFactoryDPCallArguments(objInfo)%>);
<%=Indent(--i)%>}

<% } //if(!objInfo.IsReadOnly)%>
<% if(objInfo.IsSingle || objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>private <%=objInfo.Name%>()
<%=Indent(i)%>{ /* require use of factory method */ }

<% } %>
<%=Indent(i)%>#endregion //Factory Methods
<% } //if(objInfo.IsGeneratedClass) %>
