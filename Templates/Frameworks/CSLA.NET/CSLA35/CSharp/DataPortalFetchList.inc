<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%>#region Data Access - Fetch
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>private void DataPortal_Fetch(<%=GetFactoryDPFilterDeclarationArguments(objInfo)%>)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>bool cancel = false;
<%=Indent(i)%>OnFetching(<%if(objInfo.FilterProperties.Count>0){%>criteria, <%}%>ref cancel);
<%=Indent(i)%>if (cancel) return;

<%}%>
<%=Indent(i)%>using (var mgr = ContextManager<<%=objInfo.LinqDataContext%>>
<%=Indent(i+3)%>.GetManager(<%=objInfo.DbConnection%>))
<%=Indent(i++)%>{
<%=Indent(i)%>RaiseListChangedEvents = false;
<% if(objInfo.IsReadOnly) {%>
<%=Indent(i)%>IsReadOnly = false;
<%}%>

<%=Indent(i)%>this.AddRange(
<%=Indent(i+1)%>from row in mgr.DataContext.<%=objInfo.EntitySetName%>
<% if(objInfo.FilterProperties.Count>0) {%>
<%=Indent(i+1)%>where <% for(int j=0; j<objInfo.FilterProperties.Count; j++) { 
	PropertyInfo prop = objInfo.FilterProperties[j] as PropertyInfo; %><%if(j>0){%>
<%=Indent(i+2)%>&& <%}%>row.<%=prop.DbColumnName%> == criteria.<%if(objInfo.FilterProperties.Count==1){%>Value<%}else{%><%=prop.Name%><%}%><% } %>
<%}%>
<%=Indent(i+1)%>select <%=objInfo.Child%>.<%=objInfo.GetChildMethodName%>(row)
<%=Indent(i)%>);

<% if(objInfo.IsReadOnly) {%>
<%=Indent(i)%>IsReadOnly = true;
<%}%>
<%=Indent(i)%>RaiseListChangedEvents = true;
<%=Indent(--i)%>}//using

<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>OnFetched();
<%}%>
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>partial void OnFetching(<%=GetFactoryDPFilterDeclarationArguments(objInfo, "ref bool cancel")%>);
<%=Indent(i)%>partial void OnFetched();
<%}%>
<%=Indent(i)%>#endregion //Data Access - Fetch
<%}%>