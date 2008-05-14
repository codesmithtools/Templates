
<%=Indent(i)%>#region Validation Rules
<% if(objInfo.IsUserBase) {%>
<%=Indent(i)%>protected override void AddCustomRules()
<%=Indent(i++)%>{
<%=Indent(i)%>//add custom/non-generated rules here...
<%=Indent(--i)%>}
<% } %>
<% if(objInfo.IsSingle || objInfo.IsGeneratedBase) {%>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddCustomRules()
<%=Indent(i++)%>{
<% if(objInfo.IsGeneratedBase) {%>
<%=Indent(i)%>//hook for adding custom/non-generated rules
<% } else { %>
<%=Indent(i)%>//add custom/non-generated rules here...
<% } %>
<%=Indent(--i)%>}
<% } %>
<% if(objInfo.IsGeneratedClass) {%>

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddCommonRules()
<%=Indent(i++)%>{
<%= GetCommonValidationRules(objInfo, i)%>
<%=Indent(--i)%>}

<%=Indent(i)%>protected override void AddBusinessRules()
<%=Indent(i++)%>{
<%=Indent(i)%>AddCommonRules();
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onAddCustomRules();
<%} else {%>
<%=Indent(i)%>AddCustomRules();
<%}%>
<%=Indent(--i)%>}
<% } //if(IsGeneratedClass)%>
<%=Indent(i)%>#endregion //Validation Rules
