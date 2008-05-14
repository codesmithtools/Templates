<%=Indent(i, true)%>#region Data Access - Fetch
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>private void DataPortal_Fetch(FilterCriteria criteria)
<%=Indent(i++)%>{
<%=Indent(i)%>RaiseListChangedEvents = false;
<% if(objInfo.IsReadOnly) {%>
<%=Indent(i)%>IsReadOnly = false;
<%}%>

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

<% if(objInfo.IsReadOnly) {%>
<%=Indent(i)%>IsReadOnly = true;
<%}%>
<%=Indent(i)%>RaiseListChangedEvents = true;
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteFetch(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>, FilterCriteria criteria)
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
<%=GetFilterParameters(objInfo, i)%>

<%=Indent(i)%>using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
<%=Indent(i++)%>{
<%=Indent(i)%>while (dr.Read())
<%=Indent(i+1)%>this.Add(<%=objInfo.Child%><%if(objInfo.IsGeneratedBase){%><<%=GenericTypeChildParameter%>><%}%>.<%=objInfo.GetChildMethodName%>(dr));
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteFetchComplete(cm, criteria);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Data Access - Fetch
