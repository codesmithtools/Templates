<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%>#region Data Access - DeleteSelf
<%=Indent(i)%>private void Child_DeleteSelf(<% if(objInfo.Parent.Length>0) {%><%=objInfo.Parent%> parent<% } %>)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>bool cancel = false;
<%=Indent(i)%>OnSelfDeleting(<% if(objInfo.Parent.Length>0) {%>parent , <% } %>ref cancel);
<%=Indent(i)%>if (cancel) return;

<%}%>
<%=Indent(i)%>using (var mgr = Csla.Data.ContextManager<<%=objInfo.LinqDataContext%>>
<%=Indent(i+3)%>.GetManager(<%=objInfo.DbConnection%>))
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>OnMemberSelfDeleting();
<%}%>
<%if(objInfo.ChildCollection.Count>0) {%>
<%=Indent(i)%>//Delete child object(s)
<%}%>
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%><%=prop.Name%>.Clear();
<%=Indent(i)%>DataPortal.UpdateChild(ReadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>), data.<%=prop.DbRefTableName%>);
<% } %>
<%=Indent(i)%>var data = new <%=objInfo.DalNamespace%>.<%=objInfo.EntityName%>() 
<%=Indent(i++)%>{
<% for(int j=0; j<objInfo.UniqueProperties.Count; j++) { 
	PropertyInfo prop = objInfo.UniqueProperties[j] as PropertyInfo; %>
<%=Indent(i)%><%=prop.DbColumnName%> = ReadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>)<%if(j<objInfo.UniqueProperties.Count-1){%>,<%}%>
<% } %>
<%=Indent(--i)%>};
<% foreach(PropertyInfo prop in objInfo.Properties) {
	if (!prop.IsTimestamp) continue; %>	 
<%=Indent(i)%>data.<%=prop.DbColumnName%> = <%=prop.MemberName%>;
<% } %>
<%=Indent(i)%>mgr.DataContext.<%=objInfo.EntitySetName%>.Attach(data);

<%=Indent(i)%>mgr.DataContext.<%=objInfo.EntitySetName%>.DeleteOnSubmit(data);
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>OnMemberSelfDeleted();
<%}%>
<%=Indent(--i)%>}//using
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%>SetProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, <%=prop.DefaultValue%>);
<% } %>
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>OnSelfDeleted();
<%}%>
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>partial void OnSelfDeleting(<% if(objInfo.Parent.Length>0) {%><%=objInfo.Parent%> parent, <% } %>ref bool cancel);
<%=Indent(i)%>partial void OnSelfDeleted();
<%=Indent(i)%>partial void OnMemberSelfDeleting();
<%=Indent(i)%>partial void OnMemberSelfDeleted();
<%}%>
<%=Indent(i)%>#endregion //Data Access - DeleteSelf
<%}%>