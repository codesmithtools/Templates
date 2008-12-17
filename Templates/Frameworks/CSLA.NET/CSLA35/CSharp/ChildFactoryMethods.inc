<% if(objInfo.IsGeneratedClass) {%>

<%=Indent(i)%>#region Factory Methods
<% if(!objInfo.IsReadOnly) {%>
<%=Indent(i)%>internal static <%=objInfo.Type%> <%=objInfo.NewMethodName%>(<%=GetFactoryNewDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%=Indent(i)%>return DataPortal.CreateChild<<%=objInfo.Type%>>(<%=GetFactoryNewCallArguments(objInfo)%>);
<%=Indent(--i)%>}

<% } //if(!objInfo.IsReadOnly)%>
<%=Indent(i)%>internal static <%=objInfo.Type%> <%=objInfo.GetMethodName%>(<%=objInfo.DalNamespace%>.<%=CsHelper.ConvertToSingular(objInfo.EntityName)%> data)
<%=Indent(i++)%>{
<%=Indent(i)%>return DataPortal.FetchChild<<%=objInfo.Type%>>(data);
<%=Indent(--i)%>}
<% if(objInfo.IsSingle || objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>private <%=objInfo.Name%>()
<%=Indent(i)%>{ /* require use of factory method */ }
<% } %>
<%=Indent(i)%>#endregion //Factory Methods
<% } //if(objInfo.IsGeneratedClass) %>
