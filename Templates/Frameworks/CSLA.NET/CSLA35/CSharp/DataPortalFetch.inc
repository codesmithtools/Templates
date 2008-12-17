<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%>#region Data Access - Fetch
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<% } %>
<%=Indent(i)%>private void DataPortal_Fetch(<%=GetFactoryDPDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>bool cancel = false;
<%=Indent(i)%>OnFetching(criteria, ref cancel);
<%=Indent(i)%>if (cancel) return;

<%}%>
<%=Indent(i)%>using (var mgr = ContextManager<<%=objInfo.LinqDataContext%>>
<%=Indent(i+3)%>.GetManager(<%=objInfo.DbConnection%>))
<%=Indent(i++)%>{
<%if(objInfo.ChildCollection.Count>0) {%>
<%=Indent(i)%>//set option to eager load child object(s)
<%=Indent(i)%>var opts = new System.Data.Linq.DataLoadOptions();
<%}%>
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%>opts.LoadWith<<%=objInfo.DalNamespace%>.<%=objInfo.EntityName%>>(p => p.<%=prop.DbRefTableName%>);
<% } %>
<%if(objInfo.ChildCollection.Count>0) {%>
<%=Indent(i)%>mgr.DataContext.LoadOptions = opts;

<%}%>
<%=Indent(i)%>var data = mgr.DataContext.<%=objInfo.EntitySetName%>
<%=Indent(i+2)%>.Single(r => <% for(int j=0; j<objInfo.UniqueProperties.Count; j++) { 
	PropertyInfo prop = objInfo.UniqueProperties[j] as PropertyInfo; %><%if(j>0){%>
<%=Indent(i+4)%>&& <%}%>r.<%=prop.DbColumnName%> == criteria.<%if(objInfo.UniqueProperties.Count==1){%>Value<%}else{%><%=prop.Name%><%}%><% } %>);

<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>OnMemberLoading(data);
<%}%>
<% foreach(PropertyInfo prop in objInfo.Properties) {
	if (!prop.HasDbColumn) continue; if (prop.IsTimestamp && objInfo.IsReadOnly) continue; %>
<% if (objInfo.IsReadOnly) { %>
<%=Indent(i)%><%=prop.Name%> = data.<%=prop.DbColumnName%>;
<% } else if (prop.IsTimestamp) { %>        
	 
<%=Indent(i)%><%=prop.MemberName%> = data.<%=prop.DbColumnName%>.ToArray();
<%  } else if(prop.Type != prop.NativeType) { %>
<%=Indent(i)%>LoadProperty<<%=prop.Type%>, <%=prop.NativeType%>>(<%=prop.PropertyInfoName%>, data.<%=prop.DbColumnName%>);
<%  } else { %>
<%=Indent(i)%>LoadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, data.<%=prop.DbColumnName%>);
<%  } %>
<% } %>
<%if(objInfo.ChildCollection.Count>0) {%>

<%=Indent(i)%>//Load child object(s)
<%}%>
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<% if (objInfo.IsReadOnly) { %>
<%=Indent(i)%><%=prop.Name%> = <%=prop.Type%>.Get<%=prop.Type%>(data.<%=prop.DbRefTableName%>);
<%  } else { %>
<%=Indent(i)%>LoadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, <%=prop.Type%>.Get<%=prop.Type%>(data.<%=prop.DbRefTableName%>));
<%  } %>
<% } %>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>OnMemberLoaded();
<%}%>
<% if(!objInfo.IsReadOnly) {%>

<%=Indent(i)%>ValidationRules.CheckRules();
<%}%>
<%=Indent(--i)%>}//using
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>OnFetched();
<%}%>
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>partial void OnFetching(<%=GetFactoryDPDeclarationArguments(objInfo, "ref bool cancel")%>);
<%=Indent(i)%>partial void OnFetched();
<%=Indent(i)%>partial void OnMemberLoading(<%=objInfo.DalNamespace%>.<%=objInfo.EntityName%> data);
<%=Indent(i)%>partial void OnMemberLoaded();
<%}%>
<%=Indent(i)%>#endregion //Data Access - Fetch
<%}%>