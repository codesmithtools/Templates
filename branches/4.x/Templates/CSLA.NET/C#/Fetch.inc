<%=Indent(i, true)%>#region Data Access - Fetch
<%=Indent(i)%>private void Fetch(SafeDataReader dr)
<%=Indent(i++)%>{
<%=Indent(i)%>FetchObject(dr);
<% if(!objInfo.IsReadOnly) {%>
<%=Indent(i)%>MarkOld();
<%=Indent(i)%>ValidationRules.CheckRules();
<%}%>

<%=Indent(i)%>//load child object(s)
<%=Indent(i)%>FetchChildren(dr);
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void FetchObject(SafeDataReader dr)
<%=Indent(i++)%>{
<% foreach(PropertyInfo prop in objInfo.Properties) { if (!prop.HasDbColumn) {continue;} %>
<%=Indent(i)%><%= GetReaderAssignmentStatement(prop) %>
<%}%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onFetchObjectComplete(dr);
<%}%>
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void FetchChildren(SafeDataReader dr)
<%=Indent(i++)%>{
<% if(objInfo.HasChild) { %>
<% foreach(PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%>dr.NextResult();
<%=Indent(i)%><%=string.Format("{0} = {1}.Get{1}(dr);", prop.MemberName, prop.Type)%>
<%}%>
<%}%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onFetchChildrenComplete(dr);
<%}%>
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Data Access - Fetch
