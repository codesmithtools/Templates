<%=Indent(i, true)%>#region Data Access - Fetch
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>private void DataPortal_Fetch(Criteria criteria)
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>SqlTransaction tr;

<%}%>
<%=Indent(i)%>using (SqlConnection cn = new SqlConnection(<%=objInfo.DbConnection%>))
<%=Indent(i++)%>{
<%=Indent(i)%>cn.Open();

<% if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>tr = cn.BeginTransaction();
<%=Indent(i)%>try
<%=Indent(i++)%>{
<%}%>
<%=Indent(i)%>ExecuteFetch(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, criteria);
<% if(objInfo.UseAdoTransaction) {%>

<%=Indent(i)%>tr.Commit();
<%=Indent(--i)%>}
<%=Indent(i)%>catch
<%=Indent(i++)%>{
<%=Indent(i)%>tr.Rollback();
<%=Indent(i)%>throw;
<%=Indent(--i)%>}
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteFetch(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>, Criteria criteria)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteFetch(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, criteria)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.FetchCommandText%>";

<%=GetFetchParameters(objInfo, i)%>

<%=Indent(i)%>using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
<%=Indent(i++)%>{
<% if(objInfo.CslaObjectType == ObjectType.EditableSwitchable) {%>
<%=Indent(i)%>dr.Read();
<%}%>
<%=Indent(i)%>FetchObject(dr);
<% if(!objInfo.IsReadOnly) {%>
<%=Indent(i)%>ValidationRules.CheckRules();
<%}%>

<%=Indent(i)%>//load child object(s)
<%=Indent(i)%>FetchChildren(dr);
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteFetchComplete(cm, criteria);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}

<% if(objInfo.CslaObjectType != ObjectType.EditableSwitchable) {%>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void FetchObject(SafeDataReader dr)
<%=Indent(i++)%>{
<%=Indent(i)%>dr.Read();
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
<%}%>
<%=Indent(i)%>#endregion //Data Access - Fetch
