<%=Indent(i, true)%>#region Data Access - Create
<%=Indent(i)%>[RunLocal]
<%if(objInfo.HasIdentity || objInfo.HasObjectGeneratedKey || objInfo.IsCollection) {%>
<%=Indent(i)%>protected override void DataPortal_Create()
<%}else{%>
<%=Indent(i)%>private void DataPortal_Create(Criteria criteria)
<%}%>
<%=Indent(i++)%>{
<% if(!objInfo.IsCollection && !objInfo.HasIdentity && !objInfo.HasObjectGeneratedKey) { 
   foreach(PropertyInfo prop in objInfo.UniqueProperties) { %>
<%=Indent(i)%><%= string.Format("{0} = criteria.{1};", prop.MemberName, prop.Name)%>
<% }} %>
<% if(objInfo.IsChild && objInfo.CslaObjectType != ObjectType.EditableSwitchable){ %>
<%=Indent(i)%>MarkAsChild();
<% } %>
<% if(!objInfo.IsReadOnly && !objInfo.IsCollection){ %>
<%=Indent(i)%>ValidationRules.CheckRules();
<% } %>
<%=Indent(--i)%>}
<%=Indent(i, true)%>#endregion //Data Access - Create
