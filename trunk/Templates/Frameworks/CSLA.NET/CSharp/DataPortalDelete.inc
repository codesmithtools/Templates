<%=Indent(i, true)%>#region Data Access - Delete
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>protected override void DataPortal_DeleteSelf()
<%=Indent(i++)%>{
<%=Indent(i)%>DataPortal_Delete(new Criteria(<%=GetDeleteSelfCriteriaCallArguments(objInfo)%>));
<%=Indent(--i)%>}

<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>private void DataPortal_Delete(Criteria criteria)
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
<%=Indent(i)%>ExecuteDelete(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, criteria);

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

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteDelete(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>, Criteria criteria)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteDelete(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, criteria)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.DeleteCommandText%>";

<%=GetFetchParameters(objInfo, i)%>

<%=Indent(i)%>cm.ExecuteNonQuery();
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteDeleteComplete(cm, criteria);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Data Access - Delete
