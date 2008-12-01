<% if(objInfo.IsGeneratedClass) {%>

<%=Indent(i)%>#region Validation Rules
<%=Indent(i)%>protected override void AddBusinessRules()
<%=Indent(i++)%>{
<%= GetCommonValidationRules(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>AddCustomRules();
<% } %>
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>partial void AddCustomRules();
<% } %>
<%=Indent(i)%>#endregion //Validation Rules
<% } //if(IsGeneratedClass)%>
